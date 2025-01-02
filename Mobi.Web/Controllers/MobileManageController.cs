using Microsoft.AspNetCore.Mvc;

namespace Mobi.Web.Controllers
{
    public class MobileManageController : BasePublicController
    {
        [HttpGet]
        public IActionResult MobileManage()
        {
            return View();
        }
    }
}
