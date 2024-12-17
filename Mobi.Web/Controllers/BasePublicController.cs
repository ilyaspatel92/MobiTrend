using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mobi.Web.Controllers
{
    //[Authorize(Policy = "WebPolicy")]
    //[Authorize]
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public abstract partial class BasePublicController : Controller
    {
    }
}
