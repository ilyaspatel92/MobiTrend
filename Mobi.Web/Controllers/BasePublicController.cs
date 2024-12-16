using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mobi.Web.Controllers
{
    [Authorize(Policy = "WebPolicy")]
    public abstract partial class BasePublicController : Controller
    {
    }
}
