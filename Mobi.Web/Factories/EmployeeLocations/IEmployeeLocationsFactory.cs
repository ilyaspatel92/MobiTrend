using Mobi.Data.Domain;
using Mobi.Web.Models.EmployeeLocations;

namespace Mobi.Web.Factories.EmployeeLocations
{
    public interface IEmployeeLocationsFactory
    {
        public EmployeeLocationViewModel PrepareEmployeeLocationModel(EmployeeLocation employeeLocation);
        public IEnumerable<EmployeeLocationViewModel> PrepareEmployeeLocationModels(IEnumerable<EmployeeLocation> employees);
    }
}
