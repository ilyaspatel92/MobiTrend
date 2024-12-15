using Mobi.Data.Domain;

namespace Mobi.Service.ResourceService
{
    public partial interface IResourceService
    {
        IList<LocaleStringResource> GetAllResource();
        LocaleStringResource GetResourcesById(int id);
        LocaleStringResource GetResourceByName(string resourceName);
        bool InsertResource(LocaleStringResource resourceString);
        bool UpdateResource(LocaleStringResource resourceString);
        bool Delete(int id);
        string GetResource(string resourceKey);

        void ImportResourcesFromXml(StreamReader xmlStreamReader, bool updateExistingResources = true);
    }
}
