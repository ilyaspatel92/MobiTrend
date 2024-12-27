using System.Dynamic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain.Employees;
using Mobi.Service.Employees;
using Mobi.Service.Helpers;
using Mobi.Web.Areas.Admin.Utilities;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models;
using Mobi.Web.Models.APIModels;
using Mobi.Web.Models.Employees;

namespace Mobi.Web.Areas.Admin.Controllers
{
    public class EmployeeAPIController : BaseAPIController
    {

        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly JwtTokenHelper _jwtTokenHelper;

        public EmployeeAPIController(IEmployeeService employeeService, IEmployeeFactory employeeFactory,
            JwtTokenHelper jwtTokenHelper)
        {
            _employeeService = employeeService;
            _employeeFactory = employeeFactory;
            _jwtTokenHelper = jwtTokenHelper;
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Login(int langId, [FromBody] LoginModel queryModel)
        {
            var response = new ResponseModel<ExpandoObject>();
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

                    var employee = _employeeService.GetEmployeeByEmail(queryModel.Email.Trim());
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
                        return BadRequest(response);
                    }

                    //if (employee.CompanyId== queryModel.CompanyId)
                    //{
                    //    response.Success = false;
                    //    response.Message = "Company Id is not matched";
                    //    return BadRequest(response);  //OR return response
                    //}

                    if(queryModel.RequestType.ToLower() != "web")
                    {
                        if (employee.DeviceId is null)
                        {
                            response.Success = false;
                            response.Message = "Device Id is required";
                            return BadRequest(response);  //OR return response
                        }

                        if (employee.DeviceId != queryModel.DeviceId)
                        {
                            response.Success = false;
                            response.Message = "Account Device Id Not Match";
                            return BadRequest(response);  //OR return response
                        }
                    }

                    // Generate JWT token
                    var token = _jwtTokenHelper.GenerateJwtTokenEmployee(employee);

                    dynamic empObject = new ExpandoObject();
                    empObject.Id = employee.Id;
                    empObject.CompanyId = employee.CompanyId;
                    empObject.UserName = employee.UserName;
                    empObject.NameEng = employee.NameEng;
                    empObject.NameArabic = employee.NameArabic;
                    empObject.Status = employee.Status;
                    empObject.FileNumber = employee.FileNumber;
                    empObject.MobileNumber = employee.MobileNumber;
                    empObject.Email = employee.Email;
                    empObject.PhotoPath = employee.PhotoPath;
                    empObject.UserName = employee.UserName;
                    empObject.MobileType = employee.UserName;
                    empObject.RegistrationType = employee.RegistrationType;
                    empObject.DeviceId = employee.DeviceId;
                    empObject.RegisterStatus=employee.RegisterStatus;
                    empObject.CID=employee.CID;
                    empObject.MobRegistrationDate=employee.MobRegistrationDate;
                    empObject.Token = token;

                    response.Success = true;
                    response.Message = "Login Successfully";
                    response.Data = empObject;
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
                response.Message = ex.Message;
                response.Exception = ex;
                return BadRequest( response);  //OR return response
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult VerifyQrCode(int langId,[FromBody] QrCodeModel queryModel)
        {
            var response = new ResponseModel<ExpandoObject>();
            try
            {
                if (string.IsNullOrEmpty(queryModel.QrCode))
                {
                    response.Success = false;
                    response.Message = "Qr code text is required";
                    return BadRequest( response);
                }

                var employee = _employeeService.GetEmployeeByEmail(queryModel.QrCode);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return BadRequest(response);  //OR return response
                }

                // update the employee for verify the QR code 
                employee.IsQrVerify = true;
                _employeeService.UpdateEmployee(employee);

                dynamic empObject = new ExpandoObject();
                empObject.Id = employee.Id;
                empObject.CompanyId = employee.CompanyId;
                empObject.UserName = employee.UserName;
                empObject.FullName = employee.NameEng;

                response.Success = true;
                response.Message = "Item retrieved successfully.";
                response.Data = empObject;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Exception = ex;
                return BadRequest(response);  //OR return response
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult UploadPhoto(int langId, [FromBody] PhotoModel queryModel)
        {
            var response = new ResponseModel<List<Employee>>();
            try
            {
                if (string.IsNullOrEmpty(queryModel.Profilebase64))
                {
                    response.Success = false;
                    response.Message = "Base 64 is required";
                    return BadRequest(response);
                }

                var employee = GetTokenEmployeeDetails;
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return NotFound(response);  //OR return response
                }

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

        [HttpPost]
        [AllowAnonymous]
        public IActionResult EmployeeSignUp(int langId, [FromBody] EmployeeAPIModel empModel)
        {
            var response = new ResponseModel<ExpandoObject>();
            try
            {

                if (string.IsNullOrEmpty(empModel.Password))
                {
                    response.Success = false;
                    response.Message = "Password is required ";
                    return BadRequest(response);  //OR return response
                }

                var employee = _employeeService.GetEmployeeByEmail(empModel.QrCode);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return BadRequest(response);  //OR return response
                }

                employee.Password = PasswordHelper.HashPassword(empModel.Password);
                _employeeService.UpdateEmployee(employee);

                //employee.PhotoPath

                dynamic empObject = new ExpandoObject();
                empObject.Id = employee.Id;
                empObject.CompanyId = employee.CompanyId;
                empObject.UserName = employee.UserName;

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
        [AllowAnonymous]
        public IActionResult SaveDeviceDetail(int langId, [FromBody] DeviceDetailModel queryModel)
        {
            var response = new ResponseModel<ExpandoObject>();
            try
            {
                var employee = GetTokenEmployeeDetails();
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using employeeEmail record are not found";
                    return BadRequest(response);  //OR return response
                }

                // update the employee 
                employee.DeviceId = queryModel.DeviceId;
                employee.MobileType = queryModel.MobileTypeId;
                employee.MobRegistrationDate = DateTime.UtcNow;
                _employeeService.UpdateEmployee(employee);

                dynamic empObject = new ExpandoObject();
                empObject.Id = employee.Id;
                empObject.CompanyId = employee.CompanyId;
                empObject.UserName = employee.UserName;

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
                return BadRequest( response);  //OR return response
            }
        }

        [HttpGet]
        public IActionResult GetCurrentEmployeeDetails()
        {
            ResponseModel<Employee> response = new ResponseModel<Employee>();
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var employee = _employeeService.GetCurrentEmployee(token);
            try
            {

                response.Success = true;
                response.Message = "success";
                response.Data = employee;
                return Ok(response);
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

        private Employee GetTokenEmployeeDetails()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var employee = _employeeService.GetCurrentEmployee(token);
            return employee;
        }


        #region My account / Change password

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        [HttpPost]
        public virtual IActionResult ChangePassword(int langId,[FromBody] ChangePasswordModel queryModel)
        {
            //if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            //    return Unauthorized();
            var response = new ResponseModel<List<Employee>>();
            try
            {
                var employee = GetTokenEmployeeDetails();
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return BadRequest(response);  //OR return response
                }

                if (!PasswordHelper.VerifyPassword(queryModel.OldPassword, employee.Password))
                {
                    response.Success = false;
                    response.Message = "Invalid Old Password";
                    return BadRequest(response);  //OR return response
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
                return BadRequest(response);  //OR return response
            }
        }


        [HttpGet]
        [AllowAnonymous]
        public virtual IActionResult ForgotPassword(string email)
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
