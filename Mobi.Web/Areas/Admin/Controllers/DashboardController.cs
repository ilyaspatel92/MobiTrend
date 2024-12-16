using Microsoft.AspNetCore.Mvc;

namespace Mobi.Web.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
