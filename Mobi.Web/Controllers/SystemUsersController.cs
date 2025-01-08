using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.Factories;
using Mobi.Service.SystemUser;
using Mobi.Web.Areas.Admin.Utilities;
using Mobi.Web.Models;
using Mobi.Web.Models.SystemUser;

namespace Mobi.Web.Controllers
{
    public class SystemUsersController : BasePublicController
    {
        private readonly ISystemUserService _systemUserService;
        private readonly ISystemUserFactory _systemUserFactory;
        private readonly IAccessControlService _accessControlService;

        public SystemUsersController(ISystemUserService systemUserService, ISystemUserFactory systemUserFactory, IAccessControlService accessControlService)
        {
            _systemUserService = systemUserService;
            _systemUserFactory = systemUserFactory;
            _accessControlService = accessControlService;
        }

        [HttpGet]
        public IActionResult Index(string employeeName, string userName, bool? userStatus, int page = 1, int pageSize = 5)
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
                UserStatus = user.UserStatus
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(int id, RegisterModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.Remove(nameof(model.Password));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = _systemUserService.GetSystemUserById(id);
            if (existingUser == null)
            {
                return NotFound("The specified user was not found.");
            }

            // Check for duplicate username
            var duplicateUser = _systemUserService.GetSystemUserByUserName(model.UserName);
            if (duplicateUser != null && duplicateUser.Id != id)
            {
                ModelState.AddModelError("UserName", "A user with this username already exists.");
                return View(model);
            }

            try
            {
                // Update the entity
                existingUser.EmployeeName = model.EmployeeName;
                existingUser.UserName = model.UserName;
                existingUser.CompanyID = model.CompanyID;
                existingUser.UserStatus = model.UserStatus;

                // Save changes
                _systemUserService.UpdateSystemUser(existingUser);

                TempData["SuccessMessage"] = "System user updated successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log the error (in a real-world scenario)
                ModelState.AddModelError(string.Empty, "An error occurred while updating the system user.");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var user = _systemUserService.GetSystemUserById(id);

            if (user != null)
                _systemUserService.DeleteSystemUser(user);

            return RedirectToAction("Index");
        }
    }
}
