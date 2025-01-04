using Mobi.Data.Domain;

namespace Mobi.Service.Locations
{
    public interface ILocationService
    {
        IEnumerable<Location> GetAllLocations();
        Location GetLocationById(int id);
        void AddLocation(Location location);
        void UpdateLocation(Location location);
        void RemoveLocation(Location location);
        bool IsLocationNameExists(string locationNameEnglish);

        IList<Location> GetAllEmployeeLocations(int employeeId); 
    }
}
