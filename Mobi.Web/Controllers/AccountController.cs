using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mobi.Service.EmailServices;
using Mobi.Service.Helpers;
using Mobi.Service.SystemUser;
using System.Security.Claims;
using System.Text;

namespace Mobi.Web.Controllers
{
    public class AccountController : BasePublicController
    {
        #region Fields

        private readonly ISystemUserService _systemUserService;

        #endregion

        #region Ctor

        public AccountController(ISystemUserService systemUserService)
        {
            _systemUserService = systemUserService;
        }
        #endregion

        #region Methods

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            // Validate ReturnUrl to avoid recursive redirects
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                ViewData["ReturnUrl"] = returnUrl;
            }
            else
            {
                ViewData["ReturnUrl"] = "/";
            }

            return View();
        }

        // Login POST
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string userName, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            var systemUser = _systemUserService.Authenticate(userName, password);

            // Replace with actual user validation logic
            if (systemUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, systemUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, userName)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties { IsPersistent = true });

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToAction("Index", "Home");
            }

            // Invalid login
            ModelState.AddModelError("", "Invalid username or password.");

            return View();
        }


        // Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }


        // Forgot Password GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Forgot Password POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email is required.");
                return View();
            }

            var user = _systemUserService.GetSystemUserByEmail(email);
            if (user == null)
            {
                ModelState.AddModelError("", "No account found with this email.");
                return View();
            }

            var token = Guid.NewGuid().ToString(); // Replace with your token generation logic
            var resetLink = Url.Action("ResetPassword", "Account", new { token = token, email = email }, Request.Scheme);

            _systemUserService.SavePasswordResetToken(email, token,DateTime.Now.AddHours(1));

            // Send reset link via email
            var subject = "Password Reset Request";
            var message = new StringBuilder();
            message.AppendLine($"Hi {user.EmployeeName},");
            message.AppendLine("You requested to reset your password. Click the link below to reset it:");
            message.AppendLine($"{resetLink}");
            message.AppendLine("If you did not request this, please ignore this email.");

            // Configure your email service with SMTP details
            var emailService = new EmailService("smtp.gmail.com", 587, "ppatel14151617@gmail.com", "pP@telTest");

            try
            {
                // Send an email
                emailService.SendEmail(email, subject, message.ToString());
                Console.WriteLine("Email sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }

            ViewBag.Message = "Password reset link has been sent to your email.";

            return View("ForgotPasswordConfirmation");
        }

        // Reset Password GET
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Invalid password reset request.");
            }

            // Verify token (example implementation)
            //var isValid = _systemUserService.ValidatePasswordResetToken(email, token);
            //if (!isValid)
            //{
            //    return BadRequest("Invalid or expired token.");
            //}

            ViewBag.Token = token;
            ViewBag.Email = email;

            return View();
        }

        // Reset Password POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string token, string email, string newPassword)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("", "Invalid input.");
                return View();
            }

            //Verify token
            var isValid = _systemUserService.ValidatePasswordResetToken(email, token);
            if (!isValid)
            {
                ModelState.AddModelError("", "Invalid or expired token.");
                return View();
            }

            // Update password
            var hashedPassword = PasswordHelper.HashPassword(newPassword); // Use your hashing method
            //_systemUserService.UpdatePassword(email, hashedPassword);

            ViewBag.Message = "Password reset successfully!";
            return RedirectToAction("Login");
        }

        #endregion
    }
}
