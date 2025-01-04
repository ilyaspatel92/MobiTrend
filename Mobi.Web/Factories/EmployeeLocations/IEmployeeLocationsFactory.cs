using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Web.Models.EmployeeLocations;

namespace Mobi.Web.Factories.EmployeeLocations
{
    public interface IEmployeeLocationsFactory
    {
        public EmployeeLocationViewModel PrepareEmployeeLocationModel(Employee employeeLocation);
        public IEnumerable<EmployeeLocationViewModel> PrepareEmployeeLocationModels(IEnumerable<Employee> employees);
    }
}
