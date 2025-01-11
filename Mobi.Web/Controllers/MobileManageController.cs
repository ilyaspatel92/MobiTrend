using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.Employees;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models;
using Mobi.Web.Models.Employees;

namespace Mobi.Web.Controllers
{
    public class MobileManageController : BasePublicController
    {

        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly IAccessControlService _accessControlService;
        public MobileManageController(IEmployeeFactory employeeFactory, IEmployeeService employeeService, IAccessControlService accessControlService)
        {
            _employeeFactory = employeeFactory;
            _employeeService = employeeService;
            _accessControlService = accessControlService;
        }

        [HttpGet]
        public IActionResult MobileManage(string name, int? id, int page = 1, int pageSize = 10)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.MobileManage));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");
            // Pass query string values to the view
            ViewData["Name"] = name;
            ViewData["Id"] = id?.ToString();

            // Retrieve all employees
            var query = _employeeService.GetAllEmployees();

            // Apply filters if the parameters are provided
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.NameEng.Contains(name, StringComparison.OrdinalIgnoreCase) || e.NameArabic.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (id.HasValue)
            {
                query = query.Where(e => e.Id == id.Value);
            }

            // Pagination
            var totalItems = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            // Convert to ViewModels and pass to the view
            var employeeViewModels = _employeeFactory.PrepareEmployeeViewModels(items);

            var model = new PaginatedList<EmployeeModel>
            {
                Items = employeeViewModels.ToList(),
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(model);
        }
    }
}
