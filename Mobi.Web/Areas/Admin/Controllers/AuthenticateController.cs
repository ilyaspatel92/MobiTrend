//using Microsoft.AspNetCore.Mvc;
//using Mobi.Data.Domain;
//using Mobi.Service.Employees;
//using Mobi.Service.Helpers;
//using Mobi.Service.SystemUser;
//using Mobi.Web.Areas.Admin.Utilities;
//using Mobi.Web.Models;
//using Mobi.Web.Models.APIModels;

//namespace Mobi.Web.Areas.Admin.Controllers
//{
//    //[ApiController]
//    //[Route("api/admin/auth")]
//    public class AuthenticateController : ControllerBase
//    {
//        private readonly ISystemUserService _systemUserService;
//        private readonly JwtTokenHelper _jwtTokenHelper;
//        private readonly IEmployeeService _employeeService;

//        public AuthenticateController(ISystemUserService systemUserService,
//                                      JwtTokenHelper jwtTokenHelper,
//                                      IEmployeeService employeeService)
//        {
//            _systemUserService = systemUserService;
//            _jwtTokenHelper = jwtTokenHelper;
//            _employeeService = employeeService;
//        }

//        [HttpPost("gettoken")]
//        public virtual ActionResult GetToken([FromBody] LoginModel queryModel)
//        {
//            var response = new ResponseModel<TokenResponseModel>();

//            try
//            {
//                if (ModelState.IsValid)
//                {
//                    if (string.IsNullOrEmpty(queryModel.Email) || string.IsNullOrEmpty(queryModel.Email))
//                    {
//                        response.Success = false;
//                        response.Message = "username or password is missing";
//                        return BadRequest(response);
//                    }

//                    queryModel.Email = queryModel.Email.Trim();
//                    var employee = _employeeService.GetEmployeeByEmail(queryModel.Email);
//                    if (employee is null)
//                    {
//                        response.Success = false;
//                        response.Message = "Account Login Wrong Credentials CustomerNotExist";
//                        return BadRequest(response);  //OR return response
//                    }

//                    if (!PasswordHelper.VerifyPassword(queryModel.Password, employee.Password))
//                    {
//                        response.Success = false;
//                        response.Message = "Account Login Wrong Credentials";
//                        return BadRequest(response);  //OR return response
//                    }


//                    if (employee == null)
//                        return Unauthorized(new { message = "Invalid username or password" });

//                    // Generate JWT token
//                    var token = _jwtTokenHelper.GenerateJwtTokenEmployee(employee);


//                    response.Success = true;
//                    response.Message = "Login Successfully";
//                    response.Data = new TokenResponseModel { Token = token };
//                    return Ok(response);
//                }

//                foreach (var modelState in ModelState.Values)
//                    foreach (var error in modelState.Errors)
//                        response.ErrorList.Add(error.ErrorMessage);

//                response.Success = false;
//                response.Message = "Failure";
//                return BadRequest(response);
//            }
//            catch (Exception ex)
//            {
//                response.Success = false;
//                response.Message = "Failure";
//                response.Exception = ex;
//                return StatusCode(500, response);  //OR return response
//            }
//        }

//        /// <summary>
//        /// Login endpoint to authenticate a user.
//        /// </summary>
//        /// <param name="request">Login request with username and password.</param>
//        /// <returns>JWT token or unauthorized response.</returns>
//        [HttpPost("gettokentest")]
//        public IActionResult GetTokenTest([FromBody] LoginModel request)
//        {
//            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
//                return BadRequest(new { message = "Username and password are required" });

//            SystemUsers user = null;

//            // Replace with actual user validation logic
//            if (request.Email == "admin@mobi.com" && request.Password == "admin")
//            {
//                user = new SystemUsers() 
//                {
//                    Id=1,
//                    UserName = "admin@mobi.com",
//                    Password = "admin"
//                };
//            }

//            //    var user = _systemUserService.Authenticate(request.UserName, request.Password);

//            if (user == null)
//                return Unauthorized(new { message = "Invalid username or password" });

//            // Generate JWT token
//            var token = _jwtTokenHelper.GenerateJwtToken(user);

//            return Ok(new { message = "Login successful", userId = user.Id, token = token });
//        }

//    }

//}
