using Microsoft.AspNetCore.Http.HttpResults;
using Mobi.Data.Domain.Employees;
using Mobi.Data.Enums;
using Mobi.Service.Compnay;
using Mobi.Service.Employees;
using Mobi.Service.SystemUser;
using Mobi.Web.Models.Employees;
using QRCoder;
namespace Mobi.Web.Factories.Employees
{
    public class EmployeeFactory : IEmployeeFactory
    {
        private readonly ICompanyService _companyService;
        private readonly ISystemUserService _systemUserService;
        public EmployeeFactory(ICompanyService companyService, ISystemUserService systemUserService)
        {
            _companyService = companyService;
            _systemUserService = systemUserService;
        }

        /// <summary>
        /// Prepares the EmployeeModel ViewModel from the Employee domain object.
        /// </summary>
        /// <param name="employee">The domain Employee object.</param>
        /// <returns>The ViewModel EmployeeModel.</returns>
        public EmployeeModel PrepareEmployeeViewModel(Employee employee, bool isMobileManage=false )
        {
            var emp = new EmployeeModel
            {
                Id = employee.Id,
                NameEng = employee.NameEng,
                NameArabic = employee.NameArabic,
                Status = employee.Status,
                FileNumber = employee.FileNumber,
                MobileNumber = employee.MobileNumber,
                Email = employee.Email,
                PhotoPath = "",
                CompanyId = employee.CompanyId.ToString(),
                //CompanyId = _companyService.GetCompanyById(employee.CompanyId)?.CompanyId,
                Password = employee.Password,
                MobileType = employee.MobileType,
                MobileTypeName = Enum.GetName(typeof(MobileType), employee.MobileType),
                DeviceId = employee.DeviceId,
                RegistrationVia = employee.RegistrationType,
                RegistrationTypeName = Enum.GetName(typeof(RegistrationType), employee.RegistrationType),
                RegisterStatus = employee.RegisterStatus,
                CreatedDate = employee.CreatedDate.ToString("dd/MM/yyyy @ hh:mm tt"),
                CID = employee.CID,
                UserName = employee.UserName,
                IsQrVerify = employee.IsQrVerify,
                MobRegistrationDate = employee.MobRegistrationDate?.ToString("dd/MM/yyyy"),
                //QrCode = GenerateQrCode(employee.Email)
            };

            if (isMobileManage)
                emp.CreatedBy = _systemUserService.GetSystemUserById(employee.CreatedBy)?.EmployeeName;
            else
            {
                emp.UserName = employee.UserName;
                emp.CompanyId = _companyService.GetCompanyById(employee.CompanyId)?.CompanyId;
            }

            return emp;
        }

        /// <summary>
        /// Prepares a collection of EmployeeModel ViewModels from a collection of Employee domain objects.
        /// </summary>
        /// <param name="employees">The collection of Employee domain objects.</param>
        /// <returns>The collection of ViewModel EmployeeModels.</returns>
        public IEnumerable<EmployeeModel> PrepareEmployeeViewModels(IEnumerable<Employee> employees, bool isMobileManage = false)
        {
            return employees.Select(emp => PrepareEmployeeViewModel(emp, isMobileManage));

            //return employees.Select(PrepareEmployeeViewModel);
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
