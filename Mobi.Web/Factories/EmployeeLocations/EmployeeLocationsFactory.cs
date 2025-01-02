using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Service.EmployeeLocationServices;
using Mobi.Service.Employees;
using Mobi.Service.Locations;
using Mobi.Web.Models.EmployeeLocations;

namespace Mobi.Web.Factories.EmployeeLocations
{
    public class EmployeeLocationsFactory : IEmployeeLocationsFactory
    {
        private readonly ILocationService _locationService;
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeLocationService _employeeLocationService;
        public EmployeeLocationsFactory(ILocationService locationService,
                                        IEmployeeService employeeService,
                                        IEmployeeLocationService employeeLocationService)
        {
            _locationService = locationService;
            _employeeService = employeeService;
            _employeeLocationService = employeeLocationService;
        }

        public EmployeeLocationViewModel PrepareEmployeeLocationModel(EmployeeLocation employeeLocation)
        {
            var emp = new EmployeeLocationViewModel
            {
                EmployeeId = employeeLocation.Id,
                EmployeeName = _employeeService.GetEmployeeById(employeeLocation.EmployeeId)?.NameArabic,
            };

            var selectedLocations = _employeeLocationService.GetSelectedLocationsByEmployeeId(employeeLocation.EmployeeId);

            // Fetch locations for the employee
            var locations = _locationService.GetAllLocations().Where(x=> selectedLocations.Contains(x.Id))
                .Select(l => l.LocationNameEnglish).ToList();

            emp.LocationNames = string.Join(",", locations);

            return emp;
        }

        public IEnumerable<EmployeeLocationViewModel> PrepareEmployeeLocationModels(IEnumerable<EmployeeLocation> employees)
        {
            return employees.Select(emp => PrepareEmployeeLocationModel(emp));
        }
    }

}
