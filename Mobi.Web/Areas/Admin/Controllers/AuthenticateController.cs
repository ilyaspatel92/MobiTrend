using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Service.SystemUser;
using Mobi.Web.Areas.Admin.Utilities;
using Mobi.Web.Models;

namespace Mobi.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/admin/auth")]
    public class AuthenticateController : ControllerBase
    {
        private readonly ISystemUserService _systemUserService;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public AuthenticateController(ISystemUserService systemUserService, JwtTokenHelper jwtTokenHelper)
        {
            _systemUserService = systemUserService;
            _jwtTokenHelper = jwtTokenHelper;
        }

        /// <summary>
        /// Login endpoint to authenticate a user.
        /// </summary>
        /// <param name="request">Login request with username and password.</param>
        /// <returns>JWT token or unauthorized response.</returns>
        [HttpPost("gettoken")]
        public IActionResult GetToken([FromBody] LoginModel request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest(new { message = "Username and password are required" });

            SystemUsers user = null;

            // Replace with actual user validation logic
            if (request.Email == "admin@mobi.com" && request.Password == "admin")
            {
                user = new SystemUsers() 
                {
                    Id=1,
                    UserName = "admin@mobi.com",
                    Password = "admin"
                };
            }

            //    var user = _systemUserService.Authenticate(request.UserName, request.Password);

            if (user == null)
                return Unauthorized(new { message = "Invalid username or password" });

            // Generate JWT token
            var token = _jwtTokenHelper.GenerateJwtToken(user);

            return Ok(new { message = "Login successful", userId = user.Id, token = token });
        }

    }

}
