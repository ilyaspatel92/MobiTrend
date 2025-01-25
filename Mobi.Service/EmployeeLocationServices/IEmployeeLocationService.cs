using Mobi.Data.Domain;

namespace Mobi.Service.EmployeeLocationServices
{
    public interface IEmployeeLocationService
    {
        bool SaveLocationsForEmployee(int empId, List<int> locationIds, bool isFreeLocation);
        IList<int> GetSelectedLocationsByEmployeeId(int empId);
        IEnumerable<EmployeeLocation> GetAllEmployeeLocations();
        bool IsFreeLocationSelected(int empId);
    }
}
