using Mobi.Data.Domain.Employees;
using Mobi.Web.Models.Employees;

namespace Mobi.Web.Factories.Employees
{
    public class EmployeeFactory : IEmployeeFactory
    {
        public EmployeeModel PrepareEmployeeViewModel(Employee employee)
        {
            return new EmployeeModel
            {
                Id = employee.Id,
                NameEng = employee.NameEng,
                NameArabic = employee.NameArabic,
                Status = employee.Status,
                CompanyId = employee.CompanyId,
                FileNumber = employee.FileNumber,
                MobileNumber = employee.MobileNumber,
                Email = employee.Email,
                PhotoPath = employee.PhotoPath,
                UserName = employee.UserName,
                MobileType = employee.MobileType,
                DeviceId = employee.DeviceId,
                RegistrationVia = employee.RegistrationType,
                RegisterStatus = employee.RegisterStatus,
                CreatedDate = employee.CreatedDate
            };
        }

        public IEnumerable<EmployeeModel> PrepareEmployeeViewModels(IEnumerable<Employee> employees)
        {
            return employees.Select(PrepareEmployeeViewModel);
        }
    }
}
