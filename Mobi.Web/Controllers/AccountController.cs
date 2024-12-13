using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Mobi.Web.Controllers
{
    //[Authorize(Policy = "WebPolicy")]
    public class AccountController : Controller
    {
        // Login GET
        public IActionResult Login()
        {
            return View();
        }

        // Login POST
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Replace with actual user validation logic
            if (email == "admin@mobi.com" && password == "admin")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
