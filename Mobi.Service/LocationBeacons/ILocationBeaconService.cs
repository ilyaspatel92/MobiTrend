using Mobi.Data.Domain;

namespace Mobi.Service.LocationBeacons
{
    public interface ILocationBeaconService
    {
        IEnumerable<LocationBeaconMapping> GetAllLocationBeaconMappings();
        LocationBeaconMapping GetLocationBeaconMappingById(int id);
        void AddLocationBeaconMapping(LocationBeaconMapping location);
        void UpdateLocationBeaconMapping(LocationBeaconMapping location);
        void RemoveLocationBeaconMapping(LocationBeaconMapping location);
    }
}
