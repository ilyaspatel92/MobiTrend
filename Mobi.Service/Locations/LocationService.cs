using Mobi.Data.Domain;
using Mobi.Repository;

namespace Mobi.Service.Locations
{
    public class LocationService : ILocationService
    {
        private readonly IRepository<Location> _locationRepository;

        public LocationService(IRepository<Location> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public IEnumerable<Location> GetAllLocations()
        {
            return _locationRepository.GetAll();
        }

        public Location GetLocationById(int id)
        {
            return _locationRepository.GetById(id);
        }

        public void AddLocation(Location location)
        {
            _locationRepository.Insert(location);
        }

        public void UpdateLocation(Location location)
        {
            _locationRepository.Update(location);
        }

        public void RemoveLocation(Location location)
        {
            _locationRepository.Delete(location);
        }

        public bool IsLocationNameExists(string locationNameEnglish)
        {
            return _locationRepository.GetAll()
                .Any(loc => loc.LocationNameEnglish.Equals(locationNameEnglish, StringComparison.OrdinalIgnoreCase));
        }
    }
}
