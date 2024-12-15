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
        Employee GetEmployeeByEmailOrUserName(string email);
        Employee GetEmployeeByEmail(string email);
    }
}

