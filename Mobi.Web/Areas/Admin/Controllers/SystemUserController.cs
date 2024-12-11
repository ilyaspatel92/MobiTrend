using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mobi.Service.Factories;
using Mobi.Service.SystemUser;
using Mobi.Web.Areas.Admin.Utilities;
using Mobi.Web.Models;
using Mobi.Web.Models.SystemUser;

namespace Mobi.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/admin/systemusers")]
    [Authorize]
    public class SystemUserController : ControllerBase
    {
        #region Fields

        private readonly ISystemUserService _systemUserService;
        private readonly IConfiguration _configuration;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly ISystemUserFactory _systemUserFactory;


        #endregion

        #region Ctor

        public SystemUserController(IConfiguration configuration,
                                    JwtTokenHelper jwtTokenHelper,
                                    ISystemUserService systemUserService,
                                    ISystemUserFactory systemUserFactory)
        {
            _configuration = configuration;
            _jwtTokenHelper = jwtTokenHelper;
            _systemUserService = systemUserService;
            _systemUserFactory = systemUserFactory;
        }

        #endregion

        #region Utility

        #endregion


        #region Methods

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username already exists
            var existingUser = _systemUserService.GetSystemUserByUserName(model.UserName);
            if (existingUser != null)
            {
                return Conflict("A user with this username already exists.");
            }

            // Prepare the entity
            var newUser = _systemUserFactory.PrepareSystemUser(model);

            // Save to database
            _systemUserService.InsertSystemUser(newUser);

            return Ok("User registered successfully.");
        }

        #endregion


    }
}
