using System.IdentityModel.Tokens.Jwt;
using Azure.Core;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Repository;

namespace Mobi.Service.Employees
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<SystemUsers> _systemUserRepository;

        public EmployeeService(IRepository<Employee> employeeRepository, IRepository<SystemUsers> systemUserRepository)
        {
            _employeeRepository = employeeRepository;
            _systemUserRepository= systemUserRepository;
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
            return _employeeRepository.GetAll().Any(e => e.Email is not null && e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsDeviceIdExists(string deviceId)
        {
            return _employeeRepository.GetAll().Any(e => e.DeviceId is not null && e.DeviceId.Equals(deviceId, StringComparison.OrdinalIgnoreCase));
        }


        public Employee GetEmployeeByEmail(string email)
        {
            return _employeeRepository
                .GetAll()
                .FirstOrDefault(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public Employee GetEmployeeByEmail(string email, int companyId)
        {
            return _employeeRepository
                .GetAll()
                .FirstOrDefault(e => e.Email.Equals(email, StringComparison.OrdinalIgnoreCase) && e.CompanyId == companyId);
        }

        public Employee GetEmployeeBySearchText(string searchText, int companyId)
        {
            if(searchText.ToLower().Contains("superadmin"))
            {
                return _employeeRepository
                .GetAll()
                .FirstOrDefault(e => e.Email.Equals(searchText, StringComparison.OrdinalIgnoreCase) || e.UserName.Equals(searchText, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return _employeeRepository
                .GetAll()
                .FirstOrDefault(e => e.Email.Equals(searchText, StringComparison.OrdinalIgnoreCase)||e.FileNumber.Equals(searchText, StringComparison.OrdinalIgnoreCase) || e.UserName.Equals(searchText, StringComparison.OrdinalIgnoreCase) && e.CompanyId == companyId);
            }
        }
        
        public IList<Employee> GetEmployeeByName(string name)
        {
            if (name == null)
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

        public bool IsFileNumberExists(string filenumber, int empId = 0)
        {
            if (empId > 0)
                return _employeeRepository.GetAll().Any(e => e.FileNumber is not null && e.FileNumber.Equals(filenumber, StringComparison.OrdinalIgnoreCase) && e.Id != empId);

            return _employeeRepository.GetAll().Any(e => e.FileNumber is not null && e.FileNumber.Equals(filenumber, StringComparison.OrdinalIgnoreCase));
        }

    }
}
