using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mobi.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Route("Admin/[controller]/[action]")]
    //[Authorize(Policy = "ApiPolicy")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract partial class BaseAPIController : Controller
    {
    }
}
