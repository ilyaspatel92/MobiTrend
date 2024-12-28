using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
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

        public IList<Employee> GetEmployeeByName(string name)
        {
            if(name == null)
                return new List<Employee>();

            return _employeeRepository
                .GetAll()
                .Where(e => e.NameEng.Contains(name, StringComparison.OrdinalIgnoreCase) || e.NameArabic.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public Employee GetCurrentEmployee(string token)
        {
            var employeeId = GetEmployeeIdFromToken(token);

            if (string.IsNullOrEmpty(employeeId))
                return null;

            var employee = GetEmployeeById(Convert.ToInt32(employeeId));

            return employee;

        }

        public string GetEmployeeIdFromToken(string token)
        {
            // Create a new instance of JwtSecurityTokenHandler to read the token
            var tokenHandler = new JwtSecurityTokenHandler();

            // Read the token
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Retrieve the 'EmployeeId' claim
            var employeeIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "EmployeeId");

            // Return the value of 'EmployeeId', or null if not found
            return employeeIdClaim?.Value;
        }
    }
}
