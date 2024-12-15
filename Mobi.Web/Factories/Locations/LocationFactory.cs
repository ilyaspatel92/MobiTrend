using Mobi.Data.Domain;
using Mobi.Web.Models.Locations;

namespace Mobi.Web.Factories.Locations
{
    public class LocationFactory : ILocationFactory
    {
        /// <summary>
        /// Prepares the LocationModel ViewModel from the Location domain object.
        /// </summary>
        /// <param name="location">The domain Location object.</param>
        /// <returns>The ViewModel LocationModel.</returns>
        public LocationModel PrepareLocationViewModel(Location location)
        {
            return new LocationModel
            {
                Id = location.Id,
                LocationNameEnglish = location.LocationNameEnglish,
                LocationNameArabic = location.LocationNameArabic,
                Status = location.Status,
                IsBeaconProofSelected = location.BeaconProof,
                IsGPSProofSelected = location.GPSProof,
                CreatedDate = location.CreatedDate,
                GPSLocationAddress = location.GPSLocationAddress,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                SetRadius = location.SetRadius,
                CompanyId = location.CompanyId
            };
        }

        /// <summary>
        /// Prepares a collection of LocationModel ViewModels from a collection of Location domain objects.
        /// </summary>
        /// <param name="locations">The collection of Location domain objects.</param>
        /// <returns>The collection of ViewModel LocationModels.</returns>
        public IEnumerable<LocationModel> PrepareLocationViewModels(IEnumerable<Location> locations)
        {
            return locations.Select(PrepareLocationViewModel);
        }
    }
}
