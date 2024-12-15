using Mobi.Data.Domain;
using Mobi.Repository;
using Mobi.Service.LocationBeacons;

namespace Mobi.Service.LocationBeaconMappings
{
    public class LocationBeaconService : ILocationBeaconService
    {
        private readonly IRepository<LocationBeaconMapping> _locationRepository;

        public LocationBeaconService(IRepository<LocationBeaconMapping> locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public IEnumerable<LocationBeaconMapping> GetAllLocationBeaconMappings()
        {
            return _locationRepository.GetAll();
        }

        public LocationBeaconMapping GetLocationBeaconMappingById(int id)
        {
            return _locationRepository.GetById(id);
        }

        public void AddLocationBeaconMapping(LocationBeaconMapping location)
        {
            _locationRepository.Insert(location);
        }

        public void UpdateLocationBeaconMapping(LocationBeaconMapping location)
        {
            _locationRepository.Update(location);
        }

        public void RemoveLocationBeaconMapping(LocationBeaconMapping location)
        {
            _locationRepository.Delete(location);
        }
    }
}
