using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain.Employees;
using Mobi.Service.Employees;
using Mobi.Service.Helpers;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models;
using Mobi.Web.Models.APIModels;
using Mobi.Web.Models.Employees;

namespace Mobi.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [ApiController]
    [Route("Admin/[controller]/[action]")]
    [Authorize] // Ensure this controller requires authentication
    public class EmployeeAPIController : Controller
    {

        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;

        public EmployeeAPIController(IEmployeeService employeeService, IEmployeeFactory employeeFactory)
        {
            _employeeService = employeeService;
            _employeeFactory = employeeFactory;
        }

        [HttpPost]
        public virtual  ActionResult Login([FromBody] LoginModel queryModel)
        {
            var response = new ResponseModel<Employee>();

            try
            {
                if (ModelState.IsValid)
                {
                    if (string.IsNullOrEmpty(queryModel.Email) || string.IsNullOrEmpty(queryModel.Email))
                    {
                        response.Success = false;
                        response.Message = "username or password is missing";
                        return BadRequest(response);
                    }

                    queryModel.Email = queryModel.Email.Trim();
                    var employee = _employeeService.GetEmployeeByEmailOrUserName(queryModel.Email);
                    if (employee is null)
                    {
                        response.Success = false;
                        response.Message = "Account Login WrongCredentials CustomerNotExist";
                        return BadRequest(response);  //OR return response
                    }

                    if (!PasswordHelper.VerifyPassword(queryModel.Password, employee.Password))
                    {
                        response.Success = false;
                        response.Message = "Account Login WrongCredentials";
                        return BadRequest(response);  //OR return response
                    }

                    if (employee.DeviceId != queryModel.DeviceId && employee.MobileType== queryModel.MobiTypeId)
                    {
                        response.Success = false;
                        response.Message = "Account Device Id Not Match";
                        return BadRequest(response);  //OR return response
                    }

                    response.Success = true;
                    response.Message = "Login Successfully";
                    response.Data = employee;
                    return Ok(response);
                }

                foreach (var modelState in ModelState.Values)
                    foreach (var error in modelState.Errors)
                        response.ErrorList.Add(error.ErrorMessage);

                response.Success = false;
                response.Message = "Failure";
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }
        }

        [HttpGet]
        public IActionResult VerifyQrCode(int langId, string qrCode)
        {
            var response = new ResponseModel<List<Employee>>();
            try
            {
                if (string.IsNullOrEmpty(qrCode))
                {
                    response.Success = false;
                    response.Message = "Qr code text is required";
                    return StatusCode(500, response);
                }

                var employee = _employeeService.GetEmployeeByEmail(qrCode);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return NotFound(response);  //OR return response
                }

                // update the employee for verify the QR code 
                employee.IsQrVerify = true;
                _employeeService.UpdateEmployee(employee);

                dynamic empObject = new object();
                empObject.Id = employee.Id;
                empObject.CompanyId=employee.CompanyId;
                empObject.UserName=employee.UserName;
                empObject.Email=employee.Email;

                response.Success = true;
                response.Message = "Item retrieved successfully.";
                response.Data = empObject;
                return Ok(response);  
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }
        }

        [HttpGet]
        public IActionResult UploadPhoto(int langId, string Profilebase64)
        {
            var response = new ResponseModel<List<Employee>>();
            try
            {
                if (string.IsNullOrEmpty(Profilebase64))
                {
                    response.Success = false;
                    response.Message = "Base 64 is required";
                    return StatusCode(500, response);
                }

                var employee = _employeeService.GetEmployeeByEmailOrUserName(Profilebase64);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return NotFound(response);  //OR return response
                }

                dynamic empObject = new object();

                empObject.Id = employee.Id;
                empObject.CompanyId = employee.CompanyId;
                empObject.UserName = employee.UserName;
                empObject.Email = employee.Email;

                response.Success = true;
                response.Message = "Item retrieved successfully.";
                response.Data = empObject;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }

        }

        [HttpPost]
        public IActionResult EmployeeSignUp(int langId, [FromBody] EmployeeAPIModel empModel)
        {
            var response = new ResponseModel<List<Employee>>();
            try
            {
                //var employee = _employeeService.GetEmployeeByEmail(Profilebase64);
                //if (employee is null)
                //{
                //    response.Success = false;
                //    response.Message = "Using QR code record are not found";
                //    return NotFound(response);  //OR return response
                //}

                //dynamic empObject = new object();

                //empObject.Id = employee.Id;
                //empObject.CompanyId = employee.CompanyId;
                //empObject.UserName = employee.UserName;
                //empObject.Email = employee.Email;

                response.Success = true;
                response.Message = "Item retrieved successfully.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }

        }

        [HttpGet]
        public IActionResult SaveDeviceDetail(int langId,string employeeEmail,string deviceId, int MobileTypeId )
        {
            var response = new ResponseModel<List<Employee>>();
            try
            {
                if (string.IsNullOrEmpty(employeeEmail))
                {
                    response.Success = false;
                    response.Message = "Employee text is required";
                    return StatusCode(500, response);
                }

                var employee = _employeeService.GetEmployeeByEmail(employeeEmail);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using employeeEmail record are not found";
                    return NotFound(response);  //OR return response
                }

                // update the employee 
                employee.IsQrVerify = true;
                employee.DeviceId = deviceId;
                employee.MobileType = MobileTypeId;
                employee.MobRegistrationDate = DateTime.UtcNow;
                _employeeService.UpdateEmployee(employee);

                dynamic empObject = new object();
                empObject.Id = employee.Id;
                empObject.CompanyId = employee.CompanyId;
                empObject.UserName = employee.UserName;
                empObject.Email = employee.Email;

                response.Success = true;
                response.Message = "Employee detail Updated successfully.";
                response.Data = empObject;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            ResponseModel<string> response = new ResponseModel<string>();
            try
            {

                response.Success = true;
                response.Message = "success";
                response.Data = "User created successfully!!";
                return Ok(response);              //OR return response
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Data = null;
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }
        }


        #region My account / Change password

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        [HttpPost]
        public virtual IActionResult ChangePassword([FromBody] ChangePasswordModel queryModel, string email)
        {
            //if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            //    return Unauthorized();
            var response = new ResponseModel<List<Employee>>();
            try
            {
                var employee = _employeeService.GetEmployeeByEmail(email);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return NotFound(response);  //OR return response
                }

                if (!PasswordHelper.VerifyPassword(queryModel.OldPassword, employee.Password))
                {
                    response.Success = false;
                    response.Message = "Invalid Old Password";
                    return NotFound(response);  //OR return response
                }

                employee.Password = PasswordHelper.HashPassword(queryModel.NewPassword);
                _employeeService.UpdateEmployee(employee);

                response.Success = true;
                response.Message = "Password cahnge successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }
        }


        [HttpGet]
        public virtual IActionResult PasswordRecoverySend(string email)
        {
            var response = new ResponseModel<List<Employee>>();
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    response.Success = false;
                    response.Message = "email is required";
                    return StatusCode(500, response);
                }

                var employee = _employeeService.GetEmployeeByEmail(email);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "email are not found";
                    return NotFound(response);  //OR return response
                }

                var passwordRecoveryToken = Guid.NewGuid();
                DateTime? generatedDateTime = DateTime.UtcNow;

                var passwordResetLink = Url.Action("ResetPassword", "Account",
                                                    new { Email = email, Token = passwordRecoveryToken }, protocol: HttpContext.Request.Scheme);
                //send email
                // we will send an email to the customer 


                response.Success = true;
                response.Message = "PasswordRecovery.EmailHasBeenSent";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }
        }


        [HttpGet]
        public virtual IActionResult PasswordRecoveryConfirm(string token, string email)
        {
            var response = new ResponseModel<List<Employee>>();
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    response.Success = false;
                    response.Message = "email is required";
                    return StatusCode(500, response);
                }

                var employee = _employeeService.GetEmployeeByEmail(email);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "email are not found";
                    return NotFound(response);  //OR return response
                }

                var passwordRecoveryToken = Guid.NewGuid();

                DateTime? generatedDateTime = DateTime.UtcNow;

                //send email
                // we will send an email to the customer 

                response.Success = true;
                response.Message = "PasswordRecovery.EmailHasBeenSent";
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Failure";
                response.Exception = ex;
                return StatusCode(500, response);  //OR return response
            }
        }

        #endregion

    }

}
