using Mobi.Data.Domain.Employees;
using Mobi.Web.Models.Employees;

namespace Mobi.Web.Factories.Employees
{
    public interface IEmployeeFactory
    {
        EmployeeModel PrepareEmployeeViewModel(Employee employee, bool isMobileManage = false);
        IEnumerable<EmployeeModel> PrepareEmployeeViewModels(IEnumerable<Employee> employees, bool isMobileManage = false);
    }
}
