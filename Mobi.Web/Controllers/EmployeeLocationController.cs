using Microsoft.AspNetCore.Mvc;
using Mobi.Service.EmployeeLocationServices;
using Mobi.Service.Employees;
using Mobi.Service.Locations;
using Mobi.Web.Models.EmployeeLocations;

namespace Mobi.Web.Controllers
{
    public class EmployeeLocationController : BasePublicController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILocationService _locationService;
        private readonly IEmployeeLocationService _employeeLocationService;

        public EmployeeLocationController(IEmployeeService employeeService,
                                          ILocationService locationService,
                                          IEmployeeLocationService employeeLocationService)
        {
            _employeeService = employeeService;
            _locationService = locationService;
            _employeeLocationService = employeeLocationService;
        }

        [HttpGet]
        public IActionResult SetLocation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetEmployeeSuggestions(string term)
        {
            // Fetch matching employees from the service based on the term
            var employeeDomains = _employeeService.GetEmployeeByName(term);

            // Map employee domain objects to the desired JSON structure
            var employees = employeeDomains
                .Where(e => e.Status) // Optional: Filter only active employees
                .Select(e => new
                {
                    id = e.Id,                  // Use the employee ID as the identifier
                    label = e.NameEng,          // Display name in English
                    nameArabic = e.NameArabic,  // Include Arabic name in the response if needed
                    email = e.Email,            // Include email if relevant for autocomplete UI
                    mobileNumber = e.MobileNumber // Include mobile number if needed
                })
                .ToList();

            return Json(employees);
        }

        [HttpGet]
        public IActionResult GetLocationsByEmployeeId(int employeeId)
        {
            var selectedLocations = _employeeLocationService.GetSelectedLocationsByEmployeeId(employeeId);

            // Fetch locations for the employee
            var locations = _locationService.GetAllLocations()
                .Select(l => new LocationViewModel { Id = l.Id, Name = l.LocationNameEnglish,IsSelected = selectedLocations.Contains(l.Id)})
                .ToList();

            return Json(locations);
            //return Json(null);
        }

        [HttpPost]
        public IActionResult SaveEmployeeLocations([FromBody] SaveEmployeeLocationsModel model)
        {
            if (model == null || model.LocationIds == null || !model.LocationIds.Any())
                return Json(new { success = false, message = "No locations selected." });            

            var success = _employeeLocationService.SaveLocationsForEmployee(model.EmployeeId, model.LocationIds);

            if (success)
                return Json(new { success = true, message = "Locations saved successfully!" });
            else
                return Json(new { success = false, message = "Error saving locations." });
        }


    }
}
