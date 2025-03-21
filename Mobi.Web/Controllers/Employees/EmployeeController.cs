using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.Compnay;
using Mobi.Service.Employees;
using Mobi.Service.Helpers;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models.Employees;
using System.Security.Claims;

namespace Mobi.Web.Controllers.Employees
{
    public class EmployeeController : BasePublicController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly ICompanyService _companyService;
        private readonly IAccessControlService _accessControlService;


        public EmployeeController(IEmployeeService employeeService,
                                  IEmployeeFactory employeeFactory,
                                  ICompanyService companyService,
                                  IAccessControlService accessControlService)
        {
            _employeeService = employeeService;
            _employeeFactory = employeeFactory;
            _companyService = companyService;
            _accessControlService = accessControlService;
        }

        [HttpGet]
        public IActionResult List(string name, int? id)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.Employees));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            return View(new List<EmployeeModel>());
        }

        [HttpGet]
        public IActionResult GetEmployees(string name, string? id)
        {
            var employees = _employeeService.GetAllEmployees();

            if (!string.IsNullOrEmpty(name))
            {
                employees = employees.Where(e => e.NameEng.Contains(name, StringComparison.OrdinalIgnoreCase) || e.NameArabic.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(id))
            {
                employees = employees.Where(e => e.FileNumber == id).ToList();
            }

            var employeeViewModels = _employeeFactory.PrepareEmployeeViewModels(employees);

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = employees.Count(),
                recordsFiltered = employees.Count(),
                data = employeeViewModels
            });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeModel model)
        {
            if (_employeeService.IsEmailExists(model.Email))
            {
                ModelState.AddModelError("Email", "The email address is already in use.");
            }

            if (string.IsNullOrEmpty(model.CompanyId))
                model.CompanyId = _companyService.GetCompanies(string.Empty).FirstOrDefault()?.CompanyId ?? "";

            if (_employeeService.IsFileNumberExists(model.FileNumber))
            {
                ModelState.AddModelError("FileNumber", "The FileNumber is already exsist");
            }

            if (ModelState.IsValid)
            {
                var employee = new Employee
                {
                    NameEng = model.NameEng,
                    NameArabic = model.NameArabic,
                    Status = model.Status,
                    FileNumber = model.FileNumber,
                    MobileNumber = model.MobileNumber,
                    Email = model.Email,
                    Password = PasswordHelper.HashPassword("Pass@word"),
                    CompanyId = _companyService.GetCompanies(string.Empty).FirstOrDefault()?.Id ?? 1,
                    CID = model.CID,
                    CreatedDate = DateTime.Now,
                    UserName = model.NameEng,
                    RegistrationType = (int)RegistrationType.Web
                };

                _employeeService.AddEmployee(employee);

                TempData["SuccessMessage"] = "Employee has been Added Successfully.";

                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.Employees));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            var employee = _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            // Use the factory to prepare the ViewModel
            var EmployeeModel = _employeeFactory.PrepareEmployeeViewModel(employee);

            return View(EmployeeModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EmployeeModel model)
        {
            if (id != model.Id)
            {
                ModelState.AddModelError("", "Invalid employee ID.");
                return View(model);
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                ModelState.Remove("Password");
            }

            if (_employeeService.IsFileNumberExists(model.FileNumber,id))
            {
                ModelState.AddModelError("FileNumber", "The FileNumber is already exsist");
            }

            if (ModelState.IsValid)
            {
                // Retrieve the existing employee record
                var existingEmployee = _employeeService.GetEmployeeById(id);
                if (existingEmployee == null)
                {
                    return NotFound();
                }
                // Check for email uniqueness if the email is being updated
                if (existingEmployee.Email != model.Email)
                {
                    if (_employeeService.IsEmailExists(model.Email))
                    {
                        ModelState.AddModelError("Email", "The email address is already in use.");
                        return View(model);
                    }
                }

                // Update fields
                existingEmployee.NameEng = model.NameEng;
                existingEmployee.NameArabic = model.NameArabic;
                existingEmployee.Status = model.Status;
                existingEmployee.FileNumber = model.FileNumber;
                existingEmployee.MobileNumber = model.MobileNumber;
                existingEmployee.Email = model.Email;
                existingEmployee.UserName = model.Email;
                //existingEmployee.CompanyId = model.CompanyId;

                // Update the record
                _employeeService.UpdateEmployee(existingEmployee);

                TempData["SuccessMessage"] = "Employee has been Updated Successfully.";

                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var employee = _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found." });
            }

            _employeeService.RemoveEmployee(employee);

            return Json(new { success = true, message = "Employee deleted successfully." });
        }

        public IActionResult Details(int id)
        {
            var employee = _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            var EmployeeModel = _employeeFactory.PrepareEmployeeViewModel(employee);

            return View(EmployeeModel);
        }

        [HttpPost]
        public IActionResult AjaxChangePassword([FromBody] ChangePasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.NewPassword))
            {
                return Json(new { success = false, message = "Password cannot be empty." });
            }

            var employee = _employeeService.GetEmployeeById(model.EmployeeId);
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found." });
            }

            // Update the password (hash it before saving)
            employee.Password = PasswordHelper.HashPassword(model.NewPassword);
            _employeeService.UpdateEmployee(employee);

            return Json(new { success = true, message = "Password updated successfully." });
        }
    }
}
