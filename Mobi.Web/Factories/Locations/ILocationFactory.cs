using Mobi.Data.Domain;
using Mobi.Web.Models.Locations;

namespace Mobi.Web.Factories.Locations
{
    public interface ILocationFactory
    {
        LocationModel PrepareLocationViewModel(Location location);
        IEnumerable<LocationModel> PrepareLocationViewModels(IEnumerable<Location> locations);
    }
}
