using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.SystemUser;
using Mobi.Service.SystemUserAuthoritys;
using Mobi.Web.Models.AccessControl;

namespace Mobi.Web.Controllers
{
    public class AccessControlController : BasePublicController
    {
        private readonly ISystemUserService _systemUserService;
        private readonly ISystemUserAuthorityService _systemUserAuthorityService;
        private readonly IAccessControlService _accessControlService;

        public AccessControlController(ISystemUserService systemUserService,
                                       ISystemUserAuthorityService systemUserAuthorityService,
                                       IAccessControlService accessControlService)
        {
            _systemUserService = systemUserService;
            _systemUserAuthorityService = systemUserAuthorityService;
            _accessControlService = accessControlService;
        }

        public IActionResult Index()
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.ControlACL));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            return View();
        }

        [HttpGet]
        public JsonResult GetEmployeeNames(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 3)
                return Json(new List<string>());

            var employeeNames = _systemUserService
                .GetAllUsers()
                .Where(x => x.EmployeeName.ToLower().Contains(term.ToLower()))
                .Select(x => new { x.Id, x.EmployeeName,x.UserName })
                .ToList();

            return Json(employeeNames);
        }

        [HttpGet]
        public JsonResult GetAccessForEmployee(int userId)
        {
            var accessList = _systemUserAuthorityService
                .GetAuthoritiesByUserId(userId)
                .Select(x => new
                {
                    x.ScreenAuthority,
                    x.ScreenAuthoritySystemName
                })
                .ToList();

            return Json(accessList);
        }

        [HttpPost]
        public IActionResult SaveAccess([FromBody] SaveAccessRequest request)
        {
            if (request.UserId == 0)
            {
                TempData["ErrorMessage"] = "Invalid user selection.";
                return RedirectToAction("Index");
            }

            try
            {
                _systemUserAuthorityService.DeleteByUserId(request.UserId);
                if (request.Authorities != null && request.Authorities.Any())
                {
                    foreach (var authority in request.Authorities)
                    {
                        var mapping = new SystemUserAuthorityMapping
                        {
                            SystemUserID = request.UserId,
                            ScreenAuthority = authority,
                            ScreenAuthoritySystemName = authority.Replace(" ", "").ToLower()
                        };
                        _systemUserAuthorityService.Insert(mapping);
                    }
                }
                TempData["SuccessMessage"] = "Access control updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        public IActionResult AccessDenied()
        {
            return View();  
        }

    }
}
