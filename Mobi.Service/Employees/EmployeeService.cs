using Mobi.Data.Domain.Employees;
using Mobi.Repository;

namespace Mobi.Service.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;

        public EmployeeService(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public IEnumerable<Employee> GetAllEmployees()
        {
            return _employeeRepository.GetAll();
        }

        public Employee GetEmployeeById(int id)
        {
            return _employeeRepository.GetById(id);
        }

        public Employee GetEmployeeByEmailOrUserName(string email)
        {
            return _employeeRepository
                .GetAll()
                .FirstOrDefault(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase) ||
                                     e.UserName.Equals(email, StringComparison.OrdinalIgnoreCase));
        }


        public void AddEmployee(Employee employee)
        {
            _employeeRepository.Insert(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            _employeeRepository.Update(employee);
        }

        public void RemoveEmployee(Employee employee)
        {
            _employeeRepository.Delete(employee);
        }

        public bool IsEmailExists(string email)
        {
            return _employeeRepository.GetAll().Any(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public Employee GetEmployeeByEmail(string email) 
        {
            return _employeeRepository
                .GetAll()
                .FirstOrDefault(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
    }
}
