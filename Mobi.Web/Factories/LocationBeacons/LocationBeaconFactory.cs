using Mobi.Data.Domain;
using Mobi.Web.Factories.Locations;
using Mobi.Web.Models.LocationBeacons;
using Mobi.Web.Models.Locations;

namespace Mobi.Web.Factories.LocationBeacons
{
    public class LocationBeaconFactory : ILocationBeaconFactory
    {
        /// <summary>
        /// Prepares the LocationModel ViewModel from the Location domain object.
        /// </summary>
        /// <param name="location">The domain Location object.</param>
        /// <returns>The ViewModel LocationModel.</returns>
        public LocationBeaconModel PrepareLocationBeaconViewModel(LocationBeaconMapping locationBeaconMapping)
        {
            return new LocationBeaconModel
            {
                Id = locationBeaconMapping.Id,
                BeaconName = locationBeaconMapping.BeaconName,
                LocationId = locationBeaconMapping.LocationId,
                Status = locationBeaconMapping.Status,
                UUID = locationBeaconMapping.UUID   
            };
        }

        /// <summary>
        /// Prepares a collection of LocationModel ViewModels from a collection of Location domain objects.
        /// </summary>
        /// <param name="locations">The collection of Location domain objects.</param>
        /// <returns>The collection of ViewModel LocationModels.</returns>
        public IEnumerable<LocationBeaconModel> PrepareLocationBeaconViewModels(IEnumerable<LocationBeaconMapping> locations)
        {
            return locations.Select(PrepareLocationBeaconViewModel);
        }
    }
}
