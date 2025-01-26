using AutoMapper.Configuration.Annotations;
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

        public EmployeeLocationViewModel PrepareEmployeeLocationModel(Employee employee)
        {
            var emp = new EmployeeLocationViewModel
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.NameEng,
                FileNumber= employee.FileNumber
            };

            var selectedLocations = _employeeLocationService.GetSelectedLocationsByEmployeeId(employee.Id);

            // Fetch locations for the employee
            var locations = _locationService.GetAllLocations().Where(x=> selectedLocations.Contains(x.Id))
                .Select(l => l.LocationNameEnglish).ToList();

            emp.LocationNames = string.Join(",", locations);

            if (_employeeLocationService.IsFreeLocationSelected(employee.Id))
            {
                emp.LocationNames = "Free Location";
            }

            return emp;
        }

        public IEnumerable<EmployeeLocationViewModel> PrepareEmployeeLocationModels(IEnumerable<Employee> employees)
        {
            return employees.Select(emp => PrepareEmployeeLocationModel(emp));
        }
    }

}
