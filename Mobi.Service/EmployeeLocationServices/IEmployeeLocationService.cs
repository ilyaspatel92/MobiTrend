namespace Mobi.Service.EmployeeLocationServices
{
    public interface IEmployeeLocationService
    {
        bool SaveLocationsForEmployee(int empId,List<int> locationIds);
        IList<int> GetSelectedLocationsByEmployeeId(int empId);
    }
}
