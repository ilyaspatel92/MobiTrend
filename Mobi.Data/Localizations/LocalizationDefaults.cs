using Mobi.Data.Domain.Caching;

namespace Mobi.Data.Localizations
{
    public static partial class LocalizationDefaults
    {
        #region Locales

        public static string AdminLocaleStringResourcesPrefix => "Admin.";
        public static string LocaleStringResourcesPrefixCacheKey => "Mobi.lsr.";

        public static CacheKey LocaleStringResourcesAllCacheKey => new CacheKey("Mobi.lsr.all-{0}", LocaleStringResourcesPrefixCacheKey);

        public static CacheKey LocaleStringResourcesAllPublicCacheKey => new CacheKey("Mobi.lsr.all.public-{0}", LocaleStringResourcesPrefixCacheKey);

        public static CacheKey LocaleStringResourcesAllAdminCacheKey => new CacheKey("Mobi.lsr.all.admin-{0}", LocaleStringResourcesPrefixCacheKey);

        #endregion
    }
}
