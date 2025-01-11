using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.EmployeeLocationServices;
using Mobi.Service.Employees;
using Mobi.Service.Locations;
using Mobi.Web.Factories.EmployeeLocations;
using Mobi.Web.Models;
using Mobi.Web.Models.EmployeeLocations;

namespace Mobi.Web.Controllers
{
    public class EmployeeLocationController : BasePublicController
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILocationService _locationService;
        private readonly IEmployeeLocationService _employeeLocationService;
        private readonly IEmployeeLocationsFactory _employeeLocationsFactory;
        private readonly IAccessControlService _accessControlService;
        public EmployeeLocationController(IEmployeeService employeeService,
                                          ILocationService locationService,
                                          IEmployeeLocationService employeeLocationService,
                                          IEmployeeLocationsFactory employeeLocationsFactory,
                                          IAccessControlService accessControlService)
        {
            _employeeService = employeeService;
            _locationService = locationService;
            _employeeLocationService = employeeLocationService;
            _employeeLocationsFactory = employeeLocationsFactory;
            _accessControlService = accessControlService;
        }


        #region Set Employee Location
        [HttpGet]
        public IActionResult SetLocation(int? employeeId)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.EmployeeLocation));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            if (employeeId.HasValue)
            {
                // Fetch employee details
                var employee = _employeeService.GetEmployeeById(employeeId.Value);

                if (employee == null)
                    return RedirectToAction("EmployeeLocation"); // Redirect back if the employee doesn't exist

                // Fetch selected locations for the employee
                var selectedLocations = _employeeLocationService.GetSelectedLocationsByEmployeeId(employeeId.Value);

                // Fetch all locations and set "IsSelected" for the selected ones
                var locations = _locationService.GetAllLocations()
                    .Select(l => new LocationViewModel
                    {
                        Id = l.Id,
                        Name = l.LocationNameEnglish,
                        IsSelected = selectedLocations.Contains(l.Id)
                    })
                    .ToList();

                // Prepare the model
                var model = new EmployeeLocationViewModel
                {
                    EmployeeId = employee.Id,
                    EmployeeName = employee.NameEng,
                    Locations = locations
                };

                return View(model); // Pass the model to the view
            }

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
                .Select(l => new LocationViewModel { Id = l.Id, Name = l.LocationNameEnglish, IsSelected = selectedLocations.Contains(l.Id) })
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

        #endregion

        #region Employee location


        [HttpGet]
        public IActionResult EmployeeLocation(string employeeName, int? employeeId, string siteStatus, int page = 1, int pageSize = 10)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.EmployeeLocation));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            // Pass query string values to the view
            ViewData["EmployeeName"] = employeeName;
            ViewData["EmployeeId"] = employeeId?.ToString();
            ViewData["SiteStatus"] = siteStatus;

            var query = _employeeService.GetAllEmployees();

            //// Apply search filters
            if (!string.IsNullOrEmpty(employeeName))
                query = query.Where(e => e.NameEng.Contains(employeeName, StringComparison.OrdinalIgnoreCase) || e.NameArabic.Contains(employeeName, StringComparison.OrdinalIgnoreCase));

            if (employeeId.HasValue)
                query = query.Where(e => e.Id == employeeId);

            if (siteStatus =="set")
            {
                var alreadySetLocationEmp = _employeeLocationService.GetAllEmployeeLocations().DistinctBy(x=>x.EmployeeId).Select(x=>x.EmployeeId).ToList();

                if (alreadySetLocationEmp.Any())
                    query = query.Where(e => !alreadySetLocationEmp.Contains(e.Id));
            }

            // Pagination
            var totalItems = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var employeeLocationViewModel = _employeeLocationsFactory.PrepareEmployeeLocationModels(items).ToList();

            var model = new PaginatedList<EmployeeLocationViewModel>
            {
                Items = employeeLocationViewModel,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(model);
        }


        //[HttpGet]
        //public IActionResult EmployeeLocation1(string employeeName, int? employeeId, string siteStatus, int page = 1, int pageSize = 5)
        //{
        //    var query = _employeeLocationService.GetAllEmployeeLocations();

        //    //// Apply search filters
        //    //if (!string.IsNullOrEmpty(employeeName))
        //    //    query = query.Where(e => e.EmployeeName.Contains(employeeName, StringComparison.OrdinalIgnoreCase));

        //    if (employeeId.HasValue)
        //        query = query.Where(e => e.Id == employeeId);

        //    //if (!string.IsNullOrEmpty(siteStatus))
        //    //    query = query.Where(e => e.SiteStatus.Equals(siteStatus, StringComparison.OrdinalIgnoreCase));

        //    query = query.DistinctBy(x => x.EmployeeId);

        //    // Pagination
        //    var totalItems = query.Count();
        //    var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        //    var employeeLocationViewModel = _employeeLocationsFactory.PrepareEmployeeLocationModels(items).ToList();

        //    var model = new PaginatedList<EmployeeLocationViewModel>
        //    {
        //        Items = employeeLocationViewModel,
        //        TotalItems = totalItems,
        //        CurrentPage = page,
        //        PageSize = pageSize
        //    };

        //    return View(model);
        //}

        #endregion



    }
}
