using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.Compnay;
using Mobi.Service.Employees;
using Mobi.Service.Factories;
using Mobi.Service.Helpers;
using Mobi.Service.SystemUser;
using Mobi.Service.SystemUserAuthoritys;
using Mobi.Web.Areas.Admin.Utilities;
using Mobi.Web.Models;
using Mobi.Web.Models.AccessControl;
using Mobi.Web.Models.Employees;
using Mobi.Web.Models.SystemUser;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Mobi.Web.Controllers
{
    public class SystemUsersController : BasePublicController
    {
        private readonly ISystemUserService _systemUserService;
        private readonly ISystemUserFactory _systemUserFactory;
        private readonly IAccessControlService _accessControlService;
        private readonly ISystemUserAuthorityService _systemUserAuthorityService;
        private readonly IEmployeeService _employeeService;
        private readonly ICompanyService _companyService;
        public SystemUsersController(ISystemUserService systemUserService, ISystemUserFactory systemUserFactory, IAccessControlService accessControlService, ISystemUserAuthorityService systemUserAuthorityService, IEmployeeService employeeService, ICompanyService companyService)
        {
            _systemUserService = systemUserService;
            _systemUserFactory = systemUserFactory;
            _accessControlService = accessControlService;
            _systemUserAuthorityService = systemUserAuthorityService;
            _employeeService = employeeService;
            _companyService = companyService;
        }

        [HttpGet]
        public IActionResult GetUserData(string employeeName, string userName, bool? userStatus)
        {
            try
            {
                var query = _systemUserService.GetAllUsers();

                if (!string.IsNullOrEmpty(employeeName))
                    query = query.Where(x => x.EmployeeName.Contains(employeeName));

                if (!string.IsNullOrEmpty(userName))
                    query = query.Where(x => x.UserName.Contains(userName));

                if (userStatus.HasValue)
                    query = query.Where(x => x.UserStatus == userStatus.Value);

                var totalRecords = query.Count();

                var data = query.Select(user => new
                {
                    id = user.Id,
                    fileNumber = _employeeService.GetEmployeeById(user.EmployeeId)?.FileNumber,
                    employeeName = user.EmployeeName,
                    companyId = _companyService.GetCompanyById(user.CompanyID)?.CompanyId,
                    userName = user.UserName,
                    userStatus = user.UserStatus,
                    actions = $@"
                <a href='/SystemUsers/Edit/{user.Id}' class='btn btn-warning btn-sm'>Edit</a>
                <button class='btn btn-danger btn-sm delete-btn' data-id='{user.Id}'>Delete</button>"
                }).ToList();

                return Json(new
                {
                    draw = Request.Query["draw"].FirstOrDefault() ?? "1",
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = data
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred while fetching data.", details = ex.Message });
            }
        }





        [HttpGet]
        public IActionResult Index(string employeeName, string userName, bool? userStatus, int page = 1, int pageSize = 10)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.SystemUsers));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            ViewData["EmployeeName"] = employeeName;
            ViewData["UserName"] = userName;
            ViewData["UserStatus"] = userStatus;

            var query = _systemUserService.GetAllUsers();

            if (!string.IsNullOrEmpty(employeeName))
            {
                query = query.Where(x => x.EmployeeName.Contains(employeeName));
            }

            if (!string.IsNullOrEmpty(userName))
            {
                query = query.Where(x => x.UserName.Contains(userName));
            }

            if (userStatus.HasValue)
            {
                query = query.Where(x => x.UserStatus == userStatus.Value);
            }

            var totalItems = query.Count();
            var items = query.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var model = new PaginatedList<SystemUsers>
            {
                Items = items,
                TotalItems = totalItems,
                CurrentPage = page,
                PageSize = pageSize
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.SystemUsers));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            return View();
        }

        [HttpPost]
        public IActionResult Create(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                // Return the view with the model to display validation errors
                return View(model);
            }

            // Check if username already exists
            var existingUser = _systemUserService.GetSystemUserByUserName(model.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "A user with this username already exists.");
                return View(model);
            }

            try
            {
                // Prepare the entity
                var newUser = _systemUserFactory.PrepareSystemUser(model);

                // Save to database
                _systemUserService.InsertSystemUser(newUser);

                // Redirect to list page after successful creation
                TempData["SuccessMessage"] = "System user created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error (ex can be logged in a real scenario)
                ModelState.AddModelError(string.Empty, "An error occurred while creating the system user.");
                return View(model);
            }
        }


        //[HttpGet]
        //public IActionResult Edit(int id)
        //{
        //    var user = _systemUserService.GetSystemUserById(id);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(user);
        //}

        //[HttpPost]
        //public IActionResult Edit(SystemUsers model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _systemUserService.UpdateSystemUser(model);
        //        return RedirectToAction("Index");
        //    }
        //    return View(model);
        //}

        [HttpGet]
        public IActionResult Edit(int id)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.SystemUsers));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            // Retrieve the user by ID
            var user = _systemUserService.GetSystemUserById(id);
            if (user == null)
            {
                return NotFound("The specified user was not found.");
            }

            // Prepare the view model
            var model = new RegisterModel
            {
                Id = user.Id,
                EmployeeName = user.EmployeeName,
                UserName = user.UserName,
                CompanyID = user.CompanyID,
                UserStatus = user.UserStatus,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, RegisterModel model, SaveAccessRequest request)
        {
            var existingUser = _systemUserService.GetSystemUserById(id);
            if (existingUser == null)
            {
                return NotFound("The specified user was not found.");
            }

            // Check for duplicate username
            //var duplicateUser = _systemUserService.GetSystemUserByUserName(model.UserName);
            //if (duplicateUser != null && duplicateUser.Id != id)
            //{
            //    ModelState.AddModelError("UserName", "A user with this username already exists.");
            //    return View(model);
            //}

            //var duplicateEmail = _systemUserService.GetSystemUserByEmail(model.Email);
            //if (duplicateEmail != null && duplicateEmail.Id != id)
            //{
            //    ModelState.AddModelError("Email", "A user with this email already exists.");
            //    return View(model);
            //}

            try
            {
                // Update the entity
                //existingUser.EmployeeName = model.EmployeeName;
                //existingUser.UserName = model.UserName;
                //existingUser.CompanyID = model.CompanyID;
                //existingUser.UserStatus = model.UserStatus;
                //existingUser.Email = model.Email;
                //// Save changes
                //_systemUserService.UpdateSystemUser(existingUser);

                // Now handle access control update
                if (request.Authorities != null && request.Authorities.Any())
                {
                    // First delete existing mappings for the user
                    _systemUserAuthorityService.DeleteByUserId(id);

                    // Insert new mappings for the selected authorities
                    foreach (var authority in request.Authorities)
                    {
                        var mapping = new SystemUserAuthorityMapping
                        {
                            SystemUserID = id,
                            ScreenAuthority = authority,
                            ScreenAuthoritySystemName = authority.Replace(" ", "").ToLower()
                        };

                        _systemUserAuthorityService.Insert(mapping);
                    }


                    TempData["SuccessMessage"] = "System user updated successfully!";
                }
            }
            catch (Exception ex)
            {
                // Log the error (in a real-world scenario)
                ModelState.AddModelError(string.Empty, "An error occurred while updating the system user.");
                return View(model);
            }

            return RedirectToAction("Index");

        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _systemUserService.GetSystemUserById(id);

            if (user != null) 
            {
                _systemUserAuthorityService.DeleteByUserId(user.Id);
                _systemUserService.DeleteSystemUser(user);
            }
                

            return RedirectToAction("Index");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel changePasswordModel)
        {
            // Get user information from claims
            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if (!ModelState.IsValid)
            {
                return View(changePasswordModel);
            }
            if (!string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(changePasswordModel.OldPassword) ||
                string.IsNullOrEmpty(changePasswordModel.NewPassword) || string.IsNullOrEmpty(changePasswordModel.ConfirmNewPassword))
            {
                var systemUser = _systemUserService.GetSystemUserById(int.Parse(userId));

                if (systemUser != null)
                {
                    // Check if the old password matches
                    if (!PasswordHelper.VerifyPassword(changePasswordModel.OldPassword, systemUser.Password))
                    {
                        ModelState.AddModelError("OldPassword", "Old password is incorrect.");
                    }

                    // Check if NewPassword matches ConfirmNewPassword
                    if (changePasswordModel.NewPassword != changePasswordModel.ConfirmNewPassword)
                    {
                        ModelState.AddModelError("ConfirmNewPassword", "New password and confirm password do not match.");
                    }

                    // If no errors, update the password
                    if (ModelState.IsValid)
                    {
                        systemUser.Password = PasswordHelper.HashPassword(changePasswordModel.NewPassword);
                        _systemUserService.UpdateSystemUser(systemUser);

                        TempData["SuccessMessage"] = "Password changed successfully.";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User not found.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user session.");
            }

            return View(changePasswordModel);
        }


        [HttpGet]
        public JsonResult GetEmployeeNames(string term)
        {
            if (string.IsNullOrEmpty(term) || term.Length < 3)
                return Json(new List<string>());

            var employeeNames = _employeeService
                .GetAllEmployees()
                .Where(x => x.NameEng.ToLower().Contains(term.ToLower()) || x.NameArabic.ToLower().Contains(term.ToLower()))
                .Select(x => new { x.Id, x.NameEng, x.UserName })
                .ToList();

            return Json(employeeNames);
        }

        [HttpGet]
        public JsonResult GetAccessForEmployee(int userId)
        {
            var systemUser = _systemUserService.GetSystemUserByEmployeeId(userId);
            var accessList = _systemUserAuthorityService
                .GetAuthoritiesByUserId(systemUser?.Id ?? 0)
                .Select(x => new
                {
                    x.ScreenAuthority,
                    x.ScreenAuthoritySystemName
                })
                .ToList();

            return Json(accessList);
        }

        [HttpPost]
        public IActionResult SaveAccess([FromBody] SaveAccessRequest request)
        {
            if (request.UserId == 0)
            {
                TempData["ErrorMessage"] = "Invalid user selection.";
                return RedirectToAction("Index");
            }

            try
            {
                var systemUser = _systemUserService.GetSystemUserByEmployeeId(request.UserId);

                if (systemUser == null)
                {
                    var employee = _employeeService.GetEmployeeById(request.UserId);

                    _systemUserService.InsertSystemUser(new SystemUsers
                    {
                        EmployeeName = employee.NameEng,
                        UserName = employee.UserName,
                        Email = employee.Email,
                        Password = employee.Password,
                        CompanyID = employee.CompanyId,
                        CreatedDate = DateTime.UtcNow,
                        Deleted = false,
                        UserStatus = true,
                        EmployeeId = employee.Id
                    });

                    systemUser = _systemUserService.GetSystemUserByEmployeeId(request.UserId);
                }

                _systemUserAuthorityService.DeleteByUserId(systemUser.Id);

                if (request.Authorities != null && request.Authorities.Any())
                {
                    foreach (var authority in request.Authorities)
                    {
                        var mapping = new SystemUserAuthorityMapping
                        {
                            SystemUserID = systemUser.Id,
                            ScreenAuthority = authority,
                            ScreenAuthoritySystemName = authority.Replace(" ", "").ToLower()
                        };
                        _systemUserAuthorityService.Insert(mapping);
                    }
                }
                TempData["SuccessMessage"] = "Access control updated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

    }
}
