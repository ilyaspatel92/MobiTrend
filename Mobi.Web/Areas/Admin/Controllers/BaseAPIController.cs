using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mobi.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Route("Admin/[controller]/[action]")]
    [Authorize(Policy = "ApiPolicy")]   
    public abstract partial class BaseAPIController : Controller
    {
    }
}
