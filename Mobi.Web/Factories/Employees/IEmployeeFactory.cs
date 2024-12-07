using Mobi.Data.Domain.Employees;
using Mobi.Web.Models.Employees;

namespace Mobi.Web.Factories.Employees
{
    public interface IEmployeeFactory
    {
        EmployeeModel PrepareEmployeeViewModel(Employee employee);
        IEnumerable<EmployeeModel> PrepareEmployeeViewModels(IEnumerable<Employee> employees);
    }
}
