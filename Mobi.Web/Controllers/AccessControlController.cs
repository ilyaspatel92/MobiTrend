using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Service.SystemUser;
using Mobi.Service.SystemUserAuthoritys;

namespace Mobi.Web.Controllers
{
    public class AccessControlController : BasePublicController
    {
        private readonly ISystemUserService _systemUserService;
        private readonly ISystemUserAuthorityService _systemUserAuthorityService;

        public AccessControlController(
            ISystemUserService systemUserService,
            ISystemUserAuthorityService systemUserAuthorityService)
        {
            _systemUserService = systemUserService;
            _systemUserAuthorityService = systemUserAuthorityService;
        }

        public IActionResult Index()
        {
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
                .Select(x => new { x.Id, x.EmployeeName })
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
        public IActionResult SaveAccess([FromForm] int userId, [FromForm] List<string> authorities)
        {
            if (userId == 0)
            {
                return BadRequest("Invalid data.");
            }

            _systemUserAuthorityService.DeleteByUserId(userId);
            if (authorities != null && authorities.Any())
            {
                foreach (var authority in authorities)
                {
                    var mapping = new SystemUserAuthorityMapping
                    {
                        SystemUserID = userId,
                        ScreenAuthority = authority,
                        ScreenAuthoritySystemName = authority.Replace(" ", "").ToLower()
                    };
                    _systemUserAuthorityService.Insert(mapping);
                }
            }

            return Ok("Access saved successfully.");
        }

    }
}
