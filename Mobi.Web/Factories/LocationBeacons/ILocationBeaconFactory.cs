using Mobi.Data.Domain;
using Mobi.Web.Models.LocationBeacons;
using Mobi.Web.Models.Locations;

namespace Mobi.Web.Factories.LocationBeacons
{
    public interface ILocationBeaconFactory
    {
        LocationBeaconModel PrepareLocationBeaconViewModel(LocationBeaconMapping location);
        IEnumerable<LocationBeaconModel> PrepareLocationBeaconViewModels(IEnumerable<LocationBeaconMapping> locations);
    }
}
