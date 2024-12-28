using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain.Employees;
using Mobi.Service.Compnay;
using Mobi.Service.Employees;
using Mobi.Service.Helpers;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models.Employees;

namespace Mobi.Web.Controllers.Employees
{
    public class EmployeeController : BasePublicController
    {
        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly ICompanyService _companyService;


        public EmployeeController(IEmployeeService employeeService,
                                  IEmployeeFactory employeeFactory,
                                  ICompanyService companyService)
        {
            _employeeService = employeeService;
            _employeeFactory = employeeFactory;
            _companyService = companyService;
        }

        [HttpGet]
        public IActionResult List(string name, int? id)
        {
            // Retrieve all employees
            var employees = _employeeService.GetAllEmployees();

            // Apply filters if the parameters are provided
            if (!string.IsNullOrEmpty(name))
            {
                employees = employees.Where(e => e.NameEng.Contains(name, StringComparison.OrdinalIgnoreCase) || e.NameArabic.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (id.HasValue)
            {
                employees = employees.Where(e => e.Id == id.Value).ToList();
            }

            // Convert to ViewModels and pass to the view
            var employeeViewModels = _employeeFactory.PrepareEmployeeViewModels(employees);

            return View(employeeViewModels);
        }

        [HttpGet]
        public IActionResult GetEmployees(string name, int? id)
        {
            var employees = _employeeService.GetAllEmployees();

            if (!string.IsNullOrEmpty(name))
            {
                employees = employees.Where(e => e.NameEng.Contains(name, StringComparison.OrdinalIgnoreCase) || e.NameArabic.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (id.HasValue)
            {
                employees = employees.Where(e => e.Id == id.Value).ToList();
            }

            var employeeViewModels = _employeeFactory.PrepareEmployeeViewModels(employees);

            return Json(new
            {
                draw = Request.Query["draw"], // DataTables 'draw' parameter
                recordsTotal = employees.Count(), // Total records before filtering
                recordsFiltered = employees.Count(), // Filtered records after applying search
                data = employeeViewModels // The filtered data
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

            if (model.CompanyId == 0)
                model.CompanyId = _companyService.GetCompanies(string.Empty).FirstOrDefault()?.Id ?? 1;

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
                    UserName = model.Email,
                    Password = PasswordHelper.HashPassword("Pass@word"),
                    CompanyId = _companyService.GetCompanies(string.Empty).FirstOrDefault()?.Id ?? 1,
                    CID = model.CID,
                    CreatedDate = DateTime.Now
                };

                _employeeService.AddEmployee(employee);
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        public IActionResult Edit(int id)
        {
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
                existingEmployee.CompanyId = model.CompanyId;

                // Update the record
                _employeeService.UpdateEmployee(existingEmployee);
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
    }
}
