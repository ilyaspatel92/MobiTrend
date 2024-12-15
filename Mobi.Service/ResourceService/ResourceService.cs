using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Mobi.Data;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Caching;
using Mobi.Data.Infrastructure;
using Mobi.Data.Localizations;
using Mobi.Repository;
using NLog;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.Caching;
using System.Text;
using System.Xml;
using LogManager = log4net.LogManager;

namespace Mobi.Service.ResourceService
{
    public partial class ResourceService : IResourceService
    {
        #region fields

        private readonly IRepository<LocaleStringResource> _resourceStringRepository;
        private static readonly ILog logs = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static CancellationTokenSource _clearToken = new CancellationTokenSource();
        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _prefixes = new ConcurrentDictionary<string, CancellationTokenSource>();
        private readonly IMemoryCache _memoryCache;
        private const string HASH_ALGORITHM = "SHA1";
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region ctor

        public ResourceService(IRepository<LocaleStringResource> resourceStringrepository,
             IMemoryCache memoryCache,
             IHttpContextAccessor httpContextAccessor)
        {
            _resourceStringRepository = resourceStringrepository;
            _memoryCache = memoryCache;
            _httpContextAccessor = httpContextAccessor;
        }

        #endregion

        #region Property

        public int DefaultCacheTime { get; set; }

        #endregion

        #region Utility


        private MemoryCacheEntryOptions PrepareEntryOptions(CacheKey key)
        {
            //set expiration time for the passed cache key
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(key.CacheTime)
            };

            //add tokens to clear cache entries
            options.AddExpirationToken(new CancellationChangeToken(_clearToken.Token));
            foreach (var keyPrefix in key.Prefixes.ToList())
            {
                var tokenSource = _prefixes.GetOrAdd(keyPrefix, new CancellationTokenSource());
                options.AddExpirationToken(new CancellationChangeToken(tokenSource.Token));
            }

            return options;
        }

        public void Remove(CacheKey key)
        {
            _memoryCache.Remove(key.Key);
        }

        public T Get<T>(CacheKey key, Func<T> acquire)
        {
            if (key.CacheTime <= 0)
                return acquire();

            var result = _memoryCache.GetOrCreate(key.Key, entry =>
            {
                entry.SetOptions(PrepareEntryOptions(key));

                return acquire();
            });

            //do not cache null value
            if (result == null)
                Remove(key);

            return result;
        }

        public bool IsSet(CacheKey key)
        {
            return _memoryCache.TryGetValue(key.Key, out _);
        }

        protected virtual string CreateIdsHash(IEnumerable<int> ids)
        {
            var identifiers = ids.ToList();

            if (!identifiers.Any())
                return string.Empty;

            return HashHelper.CreateHash(Encoding.UTF8.GetBytes(string.Join(", ", identifiers.OrderBy(id => id))), HASH_ALGORITHM);
        }

        protected virtual object CreateCacheKeyParameters(object parameter)
        {
            return parameter switch
            {
                null => "null",
                IEnumerable<int> ids => CreateIdsHash(ids),
                IEnumerable<BaseEntity> entities => CreateIdsHash(entities.Select(e => e.Id)),
                BaseEntity entity => entity.Id,
                decimal param => param.ToString(CultureInfo.InvariantCulture),
                _ => parameter
            };
        }

        protected virtual CacheKey FillCacheKey(CacheKey cacheKey, params object[] keyObjects)
        {
            return new CacheKey(cacheKey, CreateCacheKeyParameters, keyObjects);
        }

        public virtual CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] keyObjects)
        {
            var key = FillCacheKey(cacheKey, keyObjects);

            key.CacheTime = DefaultCacheTime;

            return key;
        }

        private static Dictionary<string, KeyValuePair<int, string>> ResourceValuesToDictionary(IEnumerable<LocaleStringResource> locales)
        {
            //format: <name, <id, value>>
            var dictionary = new Dictionary<string, KeyValuePair<int, string>>();
            foreach (var locale in locales)
            {
                var resourceName = locale.ResourceName.ToLowerInvariant();
                if (!dictionary.ContainsKey(resourceName))
                    dictionary.Add(resourceName, new KeyValuePair<int, string>(locale.Id, locale.ResourceValue));
            }

            return dictionary;
        }

        public virtual string GetResource(string resourceKey)
        {
            // Default to English (1)
            int languageId = 1;

            // Retrieve the language preference from cookies
            var languageCookie = _httpContextAccessor.HttpContext?.Request.Cookies["language"];
            if (languageCookie != null && int.TryParse(languageCookie, out int parsedLang))
            {
                languageId = parsedLang; // Use the language ID from the cookie
            }

            return GetResources(resourceKey, languageId);
        }

        public virtual string GetResources(string resourceKey, int languageId,
            bool logIfNotFound = true, string defaultValue = "", bool returnEmptyIfNotFound = false)
        {
            var result = string.Empty;
            if (resourceKey == null)
                resourceKey = string.Empty;
            resourceKey = resourceKey.Trim().ToLowerInvariant();

            var resources = GetAllResourceValues(languageId, !resourceKey.StartsWith(LocalizationDefaults.AdminLocaleStringResourcesPrefix, StringComparison.InvariantCultureIgnoreCase));
            if (resources.ContainsKey(resourceKey))
            {
                result = resources[resourceKey].Value;
            }

            if (!string.IsNullOrEmpty(result))
                return result;

            if (logIfNotFound)
                logs.Info($"Resource string ({resourceKey}) is not found. Language ID = {languageId}");

            if (!string.IsNullOrEmpty(defaultValue))
            {
                result = defaultValue;
            }

            else
            {
                if (!returnEmptyIfNotFound)
                    result = resourceKey;
            }

            return result;
        }

        public virtual Dictionary<string, KeyValuePair<int, string>> GetAllResourceValues(int languageId, bool? loadPublicLocales)
        {
            var key = PrepareKeyForDefaultCache(LocalizationDefaults.LocaleStringResourcesAllCacheKey, languageId);

            //get all locale string resources by language identifier
            if (!loadPublicLocales.HasValue || IsSet(key))
            {
                var rez = Get(key, () =>
                {
                    var query = from l in _resourceStringRepository.GetAll()
                                orderby l.ResourceName
                                where l.LanguageId == languageId
                                select l;

                    return ResourceValuesToDictionary(query);
                });

                //remove separated resource 
                Remove(PrepareKeyForDefaultCache(LocalizationDefaults.LocaleStringResourcesAllPublicCacheKey));
                Remove(PrepareKeyForDefaultCache(LocalizationDefaults.LocaleStringResourcesAllAdminCacheKey));

                return rez;
            }

            //performance optimization of the site startup
            key = PrepareKeyForDefaultCache(
                loadPublicLocales.Value ? LocalizationDefaults.LocaleStringResourcesAllPublicCacheKey : LocalizationDefaults.LocaleStringResourcesAllAdminCacheKey,
                languageId);

            return Get(key, () =>
            {
                var query = from l in _resourceStringRepository.GetAll()
                            orderby l.ResourceName
                            where l.LanguageId == languageId
                            select l;
                query = loadPublicLocales.Value ? query.Where(r => !r.ResourceName.StartsWith(LocalizationDefaults.AdminLocaleStringResourcesPrefix)) : query.Where(r => r.ResourceName.StartsWith(LocalizationDefaults.AdminLocaleStringResourcesPrefix));
                return ResourceValuesToDictionary(query);
            });
        }

        #endregion

        #region methods

        public virtual IList<LocaleStringResource> GetAllResource()
        {
            //pass album id in store procedure on the basis of face detection true 
            var cache = System.Runtime.Caching.MemoryCache.Default;
            var cachepolicy = new CacheItemPolicy();

            IList<LocaleStringResource> resources = new List<LocaleStringResource>();
            if (cache.Get("Resources") == null)
            {
                resources = _resourceStringRepository.GetAll().ToList();
                cache.Set("Resources", resources, cachepolicy);
            }
            else
            {
                resources = cache.Get("Resources") as IList<LocaleStringResource>;

            }

            if (resources.Count() > 0)
                return resources.ToList();
            else
                return resources;

        }

        public LocaleStringResource GetResourcesById(int id)
        {


            var resource = new LocaleStringResource();

            if (id > 0)
            {
                resource = _resourceStringRepository.GetById(id);

                if (resource != null)
                    return resource;
                else
                    return resource;
            }

            else

                return resource;
        }

        public virtual LocaleStringResource GetResourceByName(string resourceName)
        {
            var resource = new LocaleStringResource();
            if (!string.IsNullOrEmpty(resourceName))
            {
                resource = GetAllResource().Where(x => x.ResourceName == resourceName.Trim()).FirstOrDefault();
                return resource;
            }
            else
                return resource;
        }

        public bool InsertResource(LocaleStringResource resourceString)
        {
            if (resourceString != null)
            {
                _resourceStringRepository.Insert(resourceString);
                var cache = System.Runtime.Caching.MemoryCache.Default;
                cache.Remove("Resources", 0, null);
                return true;
            }
            else
                return false;
        }

        public bool UpdateResource(LocaleStringResource resourceString)
        {
            if (resourceString != null)
            {
                _resourceStringRepository.Update(resourceString);
                var cache = System.Runtime.Caching.MemoryCache.Default;
                cache.Remove("Resources", 0, null);
                return true;
            }
            else
                return false;
        }

        public bool Delete(int id)
        {
            if (id > 0)
            {
                var resource = _resourceStringRepository.GetById(id);
                if (resource != null)
                {
                    _resourceStringRepository.Delete(resource);
                    var cache = System.Runtime.Caching.MemoryCache.Default;
                    cache.Remove("Resources", 0, null);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public virtual void ImportResourcesFromXml(StreamReader xmlStreamReader, bool updateExistingResources = true)
        {
            if (xmlStreamReader.EndOfStream)
                return;

            //var lsNamesList = _resourceStringRepository.GetAll()
            //    .ToDictionary(x => x.ResourceName);

            var lsNamesList = _resourceStringRepository.GetAll();

            //updateList
            var lrsToUpdateList = new List<LocaleStringResource>();

            //insertList
            var lrsToInsertList = new Dictionary<string, LocaleStringResource>();

            foreach (var (name, value) in LoadLocaleResourcesFromStream(xmlStreamReader))
            {
                if (lsNamesList.Any(l => l.ResourceName == name))
                {
                    if (!updateExistingResources)
                        continue;

                    var lsr = lsNamesList.FirstOrDefault(l => l.ResourceName == name);
                    lsr.ResourceValue = value;
                    lrsToUpdateList.Add(lsr);
                }
                else
                {
                    var lsr = new LocaleStringResource { ResourceName = name, ResourceValue = value };
                    if (lrsToInsertList.ContainsKey(name))
                        lrsToInsertList[name] = lsr;
                    else
                        lrsToInsertList.Add(name, lsr);
                }
            }

            foreach (var lrsToUpdate in lrsToUpdateList)
                _resourceStringRepository.Update(lrsToUpdate);

            //insert
            InsertLocaleStringResources(lrsToInsertList.Values);
        }

        protected virtual HashSet<(string name, string value)> LoadLocaleResourcesFromStream(StreamReader xmlStreamReader)
        {
            var result = new HashSet<(string name, string value)>();

            using (var xmlReader = XmlReader.Create(xmlStreamReader))
                while (xmlReader.ReadToFollowing("Language"))
                {
                    if (xmlReader.NodeType != XmlNodeType.Element)
                        continue;

                    using var languageReader = xmlReader.ReadSubtree();
                    while (languageReader.ReadToFollowing("LocaleResource"))
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.GetAttribute("Name") is string name)
                        {
                            using var lrReader = languageReader.ReadSubtree();
                            if (lrReader.ReadToFollowing("Value") && lrReader.NodeType == XmlNodeType.Element)
                                result.Add((name, lrReader.ReadString()));
                        }
                    break;
                }
            return result;
        }

        public bool InsertLocaleStringResources(IEnumerable<LocaleStringResource> resources)
        {
            if (resources.Count() > 0)
            {
                foreach (var resource in resources)
                {
                    //insert
                    _resourceStringRepository.Insert(resource);
                }
                return true;
            }
            else
                return false;
        }

        #endregion

    }
}
