using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Enums;
using Mobi.Service.AccessControls;
using Mobi.Service.Employees;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models;
using Mobi.Web.Models.Employees;
using Mobi.Web.Models.MobileManage;
using QRCoder;

namespace Mobi.Web.Controllers
{
    public class MobileManageController : BasePublicController
    {

        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly IAccessControlService _accessControlService;
        public MobileManageController(IEmployeeFactory employeeFactory, IEmployeeService employeeService, IAccessControlService accessControlService)
        {
            _employeeFactory = employeeFactory;
            _employeeService = employeeService;
            _accessControlService = accessControlService;
        }

        [HttpGet]
        public IActionResult MobileManage()
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.MobileManage));
            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");
            return View();
        }

        [HttpGet]
        public IActionResult GetMobileManageData(string name, string? filenumber)
        {
            var query = _employeeService.GetAllEmployees();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(e => e.NameEng.Contains(name, StringComparison.OrdinalIgnoreCase) || e.NameArabic.Contains(name, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrEmpty(filenumber))
            {
                query = query.Where(e => e.FileNumber == filenumber);
            }


            var employeeViewModels = _employeeFactory.PrepareEmployeeViewModels(query.ToList(),true);

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = employeeViewModels.Count(),
                recordsFiltered = employeeViewModels.Count(),
                data = employeeViewModels
            });
        }


        [HttpGet]
        public IActionResult RegisterMobile(int id)
        {
            var employee = _employeeService.GetEmployeeById(id);

            if (employee == null)
                return RedirectToAction(nameof(MobileManage));

            var qrCode = GenerateQrCode(employee.Email);

            var viewModel = new RegisterMobileViewModel
            {
                QrCode = qrCode
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult EditMobile(int id)
        {
            var employee = _employeeService.GetEmployeeById(id);

            if (employee == null)
                return RedirectToAction(nameof(MobileManage));

            return View(employee.Id);
        }

        [HttpPost]
        public IActionResult EditMobile(int id, string action)
        {
            var employee = _employeeService.GetEmployeeById(id);

            if (employee == null)
                return RedirectToAction(nameof(MobileManage));

            if (action == "register")
            {
                employee.MobileType = 0;
                employee.RegistrationType = 10;
                employee.DeviceId = string.Empty;
                employee.RegisterStatus = false;
                employee.IsQrVerify = false;
                employee.MobRegistrationDate = null;

                // Redirect to the RegisterMobile action with the employee ID
                return RedirectToAction(nameof(RegisterMobile), new { id });
            }
            else if (action == "remove")
            {
                employee.MobileType = 0;
                employee.RegistrationType = 10;
                employee.DeviceId = string.Empty;
                employee.RegisterStatus =false;
                employee.IsQrVerify = false;
                employee.MobRegistrationDate = null;


                // Perform removal logic if necessary
                _employeeService.UpdateEmployee(employee);
                TempData["SuccessMessage"] = "Phone has been Successfully unregistered";
                // Redirect to the MobileManage action
                return RedirectToAction(nameof(MobileManage));
            }

            // Default redirect if no valid action is found
            return RedirectToAction(nameof(MobileManage));
        }


        private string GenerateQrCode(string email)
        {
            // Initialize the QRCode generator
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(email, QRCodeGenerator.ECCLevel.Q);

            // Generate the QRCode image
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);

            // Convert to Base64 string
            string base64String = Convert.ToBase64String(qrCode.GetGraphic(20));

            // Return as a data URI for embedding in an <img> tag
            return string.Format("data:image/png;base64,{0}", base64String);
        }
    }
}
