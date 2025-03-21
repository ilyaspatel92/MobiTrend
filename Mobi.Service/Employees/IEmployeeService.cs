using Mobi.Data.Domain.Employees;

namespace Mobi.Service.Employees
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetAllEmployees();
        Employee GetEmployeeById(int id);
        void AddEmployee(Employee employee);
        void UpdateEmployee(Employee employee);
        void RemoveEmployee(Employee employee);
        bool IsEmailExists(string email);
        bool IsDeviceIdExists(string deviceId);
        Employee GetEmployeeByEmail(string email);

        Employee GetEmployeeByEmail(string email, int companyId);
        Employee GetCurrentEmployee(string token);
        IList<Employee> GetEmployeeByName(string name);

        bool IsFileNumberExists(string filenumber, int empId = 0);

        Employee GetEmployeeBySearchText(string searchText, int companyId);
    }
}

