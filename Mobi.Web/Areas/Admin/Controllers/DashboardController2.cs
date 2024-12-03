using Microsoft.AspNetCore.Mvc;

namespace Mobi.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ApiController]
    [Route("Admin/[controller]/[action]")]
    public class DashboardController2 : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok(new { Message = "Welcome to the Admin Dashboard" });
        }
    }
}
