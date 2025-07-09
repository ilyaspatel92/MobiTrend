using System.Dynamic;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Data.Enums;
using Mobi.Service.Compnay;
using Mobi.Service.EmployeeAttendances;
using Mobi.Service.EmployeeLocationServices;
using Mobi.Service.Employees;
using Mobi.Service.Helpers;
using Mobi.Service.Locations;
using Mobi.Service.Pictures;
using Mobi.Web.Areas.Admin.Utilities;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models;
using Mobi.Web.Models.APIModels;
using Mobi.Web.Models.Employees;
using Newtonsoft.Json.Linq;

namespace Mobi.Web.Areas.Admin.Controllers
{
    public class EmployeeAPIController : BaseAPIController
    {

        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly IPictureService _pictureService;
        private readonly IEmployeeAttendanceService _employeeAttendanceService;
        private readonly ILocationService _locationService;
        private readonly IEmployeeLocationService _employeeLocationService;
        private readonly ICompanyService _companyService;

        public EmployeeAPIController(IEmployeeService employeeService,
                                     IEmployeeFactory employeeFactory,
                                     JwtTokenHelper jwtTokenHelper,
                                     IPictureService pictureService,
                                     IEmployeeAttendanceService employeeAttendanceService,
                                     ILocationService locationService,
                                     IEmployeeLocationService employeeLocationService,
                                     ICompanyService companyService)
        {
            _employeeService = employeeService;
            _employeeFactory = employeeFactory;
            _jwtTokenHelper = jwtTokenHelper;
            _pictureService = pictureService;
            _employeeAttendanceService = employeeAttendanceService;
            _locationService = locationService;
            _employeeLocationService = employeeLocationService;
            _companyService = companyService;
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
                    if (string.IsNullOrEmpty(queryModel.Username) || string.IsNullOrEmpty(queryModel.Username))
                    {
                        if (string.IsNullOrEmpty(queryModel.Email) || string.IsNullOrEmpty(queryModel.Email))
                        {
                            response.Success = false;
                            response.Message = "username or password is missing";
                            return BadRequest(response);
                        }
                    }

                    if (queryModel.CompanyId <= 0)
                    {
                        response.Success = false;
                        response.Message = "Company ID is required";
                        return BadRequest(response);
                    }

                    var searchText = queryModel.Username.Trim();

                    if (!string.IsNullOrEmpty(queryModel.Email))
                        searchText = queryModel.Email.Trim();


                    var employee = _employeeService.GetEmployeeBySearchText(searchText, queryModel.CompanyId);
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

                    if (queryModel.RequestType.ToLower() != "web")
                    {
                        if (employee.DeviceId is null)
                        {
                            response.Success = false;
                            response.Message = "Device Id is required";
                            return BadRequest(response);  //OR return response
                        }

                        if (!employee.DeviceId.Equals(queryModel.DeviceId, StringComparison.OrdinalIgnoreCase))
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
                    empObject.NameEng = employee.NameEng;
                    empObject.NameArabic = employee.NameArabic;
                    empObject.Status = employee.Status;
                    empObject.CompanyId = employee.CompanyId;
                    empObject.UserName = employee.UserName;
                    empObject.FileNumber = employee.FileNumber;
                    empObject.MobileNumber = employee.MobileNumber;
                    empObject.Email = employee.Email;
                    empObject.MobileType = Enum.GetName((MobileType)employee.MobileType);
                    empObject.RegistrationType = Enum.GetName((RegistrationType)employee.MobileType);
                    empObject.DeviceId = employee.DeviceId;
                    empObject.RegisterStatus = employee.RegisterStatus;
                    empObject.CID = employee.CID;
                    empObject.MobRegistrationDate = employee.MobRegistrationDate;
                    empObject.Token = token;
                    empObject.IsFreeLocation = _employeeLocationService.IsFreeLocationSelected(employee.Id);

                    var company = _companyService.GetCompanyById(employee.CompanyId);
                    if (company is not null)
                    {
                        empObject.CompanyName = company.CompanyName;
                        empObject.IsAllowedPhoto = company.IsAllowedPhoto;
                    }

                    var picture = _pictureService.GetPictureById(employee?.PictureId ?? 0);
                    if (picture is not null)
                    {
                        var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/";
                        empObject.Path = url + picture.Path;
                    }

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
                return BadRequest(response);  //OR return response
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult VerifyQrCode(int langId, [FromBody] QrCodeModel queryModel)
        {
            var response = new ResponseModel<ExpandoObject>();
            try
            {
                if (string.IsNullOrEmpty(queryModel.QrCode))
                {
                    response.Success = false;
                    response.Message = "Qr code text is required";
                    return BadRequest(response);
                }

                var parts = queryModel.QrCode.Split(':');
                if (parts.Length != 2)
                {
                    response.Success = false;
                    response.Message = "Invalid QR code format.";
                    return BadRequest(response);
                }

                string email = parts[0];
                string timestampString = parts[1];

                if (!long.TryParse(timestampString, out var qrTimestamp))
                {
                    response.Success = false;
                    response.Message = "Invalid timestamp in QR code.";
                    return BadRequest(response);
                }

                var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (currentTimestamp - qrTimestamp > 600)
                {
                    response.Success = false;
                    response.Message = "QR code has expired.";
                    return BadRequest(response);
                }

                var employee = _employeeService.GetEmployeeByEmail(email);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return BadRequest(response);
                }

                dynamic empObject = new ExpandoObject();
                empObject.Id = employee.Id;
                empObject.CompanyId = employee.CompanyId;
                empObject.UserName = employee.UserName;
                empObject.FullName = employee.NameEng;
                empObject.email = employee.Email;

                var company = _companyService.GetCompanyById(employee.CompanyId);
                if (company is not null)
                {
                    empObject.CompanyName = company.CompanyName;
                    empObject.IsAllowedPhoto = company.IsAllowedPhoto;
                }

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

        //[HttpPost]
        //[AllowAnonymous]
        //public IActionResult UploadPhoto(int langId, [FromBody] PhotoModel queryModel)
        //{
        //    var response = new ResponseModel<List<Employee>>();
        //    try
        //    {
        //        if (string.IsNullOrEmpty(queryModel.Profilebase64))
        //        {
        //            response.Success = false;
        //            response.Message = "Base 64 is required";
        //            return BadRequest(response);
        //        }

        //        var employee = GetTokenEmployeeDetails;
        //        if (employee is null)
        //        {
        //            response.Success = false;
        //            response.Message = "Using QR code record are not found";
        //            return NotFound(response);  //OR return response
        //        }

        //        //dynamic empObject = new object();

        //        //empObject.Id = employee.Id;
        //        //empObject.CompanyId = employee.CompanyId;
        //        //empObject.UserName = employee.UserName;
        //        //empObject.Email = employee.Email;

        //        response.Success = true;
        //        response.Message = "Item retrieved successfully.";
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = "Failure";
        //        response.Exception = ex;
        //        return StatusCode(500, response);  //OR return response
        //    }
        //}

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

                var parts = empModel.QrCode.Split(':');
                if (parts.Length != 2)
                {
                    response.Success = false;
                    response.Message = "Invalid QR code format.";
                    return BadRequest(response);
                }

                string email = parts[0];

                var employee = _employeeService.GetEmployeeByEmail(email);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return BadRequest(response);  //OR return response
                }

                employee.Password = PasswordHelper.HashPassword(empModel.Password);

                dynamic empObject = new ExpandoObject();
                //employee.PhotoPath
                var picture = _pictureService.GetPictureById(empModel.PictureId);
                if (picture is not null)
                {
                    employee.PictureId = empModel.PictureId;
                    var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/";
                    empObject.Path = url + picture.Path;
                }

                employee.MobRegistrationDate = DateTime.UtcNow;

                _employeeService.UpdateEmployee(employee);

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
                var employee = _employeeService.GetEmployeeById(queryModel.EmpId);
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using QR code record are not found";
                    return BadRequest(response);  //OR return response
                }

                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using employeeEmail record are not found";
                    return BadRequest(response);  //OR return response
                }

                if (string.IsNullOrEmpty(employee.DeviceId))
                {
                    var deviceExist = _employeeService.IsDeviceIdExists(queryModel.DeviceId);
                    if (deviceExist)
                    {
                        response.Success = false;
                        response.Message = "the given Device Id is already Exist with other employee";
                        return BadRequest(response);
                    }
                }
                else
                {
                    if (!employee.DeviceId.Equals(queryModel.DeviceId, StringComparison.OrdinalIgnoreCase))
                    {
                        response.Success = false;
                        response.Message = "the given Device Id is not mapped with current employee ";
                        return BadRequest(response);
                    }
                }

                // update the employee 
                employee.DeviceId = queryModel.DeviceId;
                employee.MobileType = queryModel.MobileTypeId;
                employee.MobRegistrationDate = DateTime.UtcNow;
                employee.RegisterStatus = true;
                employee.RegistrationType = (int)RegistrationType.Mobile;
                // update the employee for verify the QR code 
                employee.IsQrVerify = true;
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
                return BadRequest(response);  //OR return response
            }
        }

        [HttpGet]
        public IActionResult GetCurrentEmployeeDetails()
        {
            ResponseModel<ExpandoObject> response = new ResponseModel<ExpandoObject>();
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var employee = _employeeService.GetCurrentEmployee(token);
            try
            {
                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using employeeEmail record are not found";
                    return BadRequest(response);  //OR return response
                }

                dynamic empObject = new ExpandoObject();
                empObject.Id = employee.Id;
                empObject.NameEng = employee.NameEng;
                empObject.NameArabic = employee.NameArabic;
                empObject.Status = employee.Status;
                empObject.CompanyId = employee.CompanyId;
                empObject.UserName = employee.UserName;
                empObject.FileNumber = employee.FileNumber;
                empObject.MobileNumber = employee.MobileNumber;
                empObject.Email = employee.Email;
                empObject.MobileType = Enum.GetName((MobileType)employee.MobileType);
                empObject.RegistrationType = Enum.GetName((RegistrationType)employee.MobileType);
                empObject.DeviceId = employee.DeviceId;
                empObject.RegisterStatus = employee.RegisterStatus;
                empObject.CID = employee.CID;
                empObject.MobRegistrationDate = employee.MobRegistrationDate;
                empObject.Token = token;
                empObject.IsFreeLocation = _employeeLocationService.IsFreeLocationSelected(employee.Id);

                var picture = _pictureService.GetPictureById(employee?.PictureId ?? 0);
                if (picture is not null)
                {
                    var url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/";
                    empObject.Path = url + picture.Path;
                }

                var company = _companyService.GetCompanyById(employee.CompanyId);
                if (company is not null)
                {
                    empObject.CompanyName = company.CompanyName;
                    empObject.IsAllowedPhoto = company.IsAllowedPhoto;
                }

                response.Success = true;
                response.Message = "success";
                response.Data = empObject;
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

        #region Sign / sign Out 

        [HttpPost]
        public IActionResult SignInOut(int langId, [FromBody] EmployeeAttendanceModel queryModel)
        {
            var response = new ResponseModel<ExpandoObject>();
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var employee = _employeeService.GetCurrentEmployee(token);

                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using employeeEmail record are not found";
                    return BadRequest(response);  //OR return response
                }

                if (string.IsNullOrEmpty(employee.DeviceId) == false && !employee.DeviceId.Equals(queryModel.MobileSerialNumber, StringComparison.OrdinalIgnoreCase))
                {
                    response.Success = false;
                    response.Message = "the given Device Id is not mapped with current employee ";
                    return BadRequest(response);
                }

                var actionTypeStatus = false;
                if (queryModel.LocationId > 0)
                    actionTypeStatus = true;
                else
                    actionTypeStatus = _employeeLocationService.IsFreeLocationSelected(employee.Id);

                var empAttendance = new EmployeeAttendanceLogs()
                {
                    EmployeeId = employee.Id,
                    AttendanceDateTime = queryModel.AttendanceDateTime,
                    LocalTimeAttendanceDateTime = queryModel.AttendanceDateTime.ConvertToUserTime(),
                    TransferDateTime = queryModel.TransferDateTime,
                    MobileSerialNumber = queryModel.MobileSerialNumber,
                    LocationId = queryModel.LocationId,
                    LocationBeaconMappingId = queryModel.BeaconId,
                    Latitude = queryModel.Latitude,
                    Longtitude = queryModel.Longitude,
                    PictureId = queryModel.PictureId,
                    ActionTypeId = (int)queryModel.ActionType, //In and out enum 
                    ProofTypeId = (int)queryModel.ActionTypeMode, // GPS or BEacon 
                    IsVerifiedLocation = queryModel.IsverifiedLocation,
                    CreatedDateTime = DateTime.UtcNow,
                    ActionTypeStatus = actionTypeStatus
                };

                var location = _locationService.GetLocationById(queryModel.LocationId);
                if (queryModel.LocationId > 0 && location is not null)
                {
                    empAttendance.CurrentLocation = location.GPSLocationAddress;
                }
                else if (queryModel.Latitude>decimal.Zero && queryModel.Longitude>decimal.Zero)
                    empAttendance.CurrentLocation=GetAddressByLatLong(queryModel.Latitude, queryModel.Longitude);

                _employeeAttendanceService.AddLog(empAttendance);

                response.Success = true;
                response.Message = "Item retrieved successfully.";
                //response.Data = empObject;
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
        public IActionResult GetEmployeeAttendance(int langId, [FromBody] EmployeeGetAttendanceModel queryModel)
        {
            var response = new ResponseModel<List<EmployeeAttendanceResponseModel>>();
            try
            {
                var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                var employee = _employeeService.GetCurrentEmployee(token);

                if (employee is null)
                {
                    response.Success = false;
                    response.Message = "Using employeeEmail record are not found";
                    return BadRequest(response);  //OR return response
                }

                var employeeAttendanceList = _employeeAttendanceService.GetLogsByEmployeeId(employee.Id).ToList();
                if (employeeAttendanceList.Any() == false)
                {
                    response.Success = false;
                    response.Message = "No record are found";
                    return BadRequest(response);  //OR return response
                }

                if (queryModel.AttendanceDateTime.HasValue)
                {
                    var attendanceDateTime = queryModel.AttendanceDateTime.Value.Date.ConvertToUserTime();
                    employeeAttendanceList = employeeAttendanceList.Where(x => x.AttendanceDateTime.ConvertToUserTime().Date == attendanceDateTime.Date).ToList();
                }

                var employeeAttendanceResponseList = new List<EmployeeAttendanceResponseModel>();
                foreach (var item in employeeAttendanceList)
                {
                    var model = new EmployeeAttendanceResponseModel();

                    model.EmployeeId = item.EmployeeId;
                    model.Id = item.Id;
                    model.LocationId = item.LocationId;
                    model.LocationName = _locationService.GetLocationById(item.LocationId)?.LocationNameEnglish ?? string.Empty;
                    model.ActionType = item.ActionTypeId;
                    model.ActionTypeMode = item.ProofTypeId;
                    model.TransferDateTime = item.TransferDateTime;
                    model.AttendanceDateTime = item.AttendanceDateTime;
                    model.LocalTimeAttendanceDateTime = item.LocalTimeAttendanceDateTime;

                    if (item.LocationId == 0)
                    {
                        model.LocationName = string.IsNullOrEmpty(item.CurrentLocation)==false? item.CurrentLocation: GetAddressByLatLong(item.Latitude,item.Longtitude);
                        model.IsFreeLocation = _employeeLocationService.IsFreeLocationSelected(employee.Id);
                    }

                    employeeAttendanceResponseList.Add(model);
                }


                response.Success = true;
                response.Message = "Item retrieved successfully.";
                response.Data = employeeAttendanceResponseList;
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
        #endregion

        #region My account / Change password

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        [HttpPost]
        public virtual IActionResult ChangePassword(int langId, [FromBody] ChangePasswordModel queryModel)
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
                //DateTime? generatedDateTime = DateTime.Now;

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


        #region Utilities 
        private string GetAddressByLatLong(decimal latitude,decimal longitude)
        {
            var address = string.Empty;
            string url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={latitude}&lon={longitude}";
            using (WebClient client = new WebClient())
            {
                // Nominatim requires a valid User-Agent
                client.Headers.Add("User-Agent", "CSharpReverseGeocoderApp");

                try
                {
                    string response = client.DownloadString(url);
                    JObject json = JObject.Parse(response);

                    address = json["display_name"]?.ToString();

                    return address;

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }

                return address;
            }
        }
        #endregion

    }

}
