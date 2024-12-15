using Mobi.Data.Domain.Employees;
using Mobi.Web.Models.Employees;
using QRCoder;
namespace Mobi.Web.Factories.Employees
{
    public class EmployeeFactory : IEmployeeFactory
    {
        /// <summary>
        /// Prepares the EmployeeModel ViewModel from the Employee domain object.
        /// </summary>
        /// <param name="employee">The domain Employee object.</param>
        /// <returns>The ViewModel EmployeeModel.</returns>
        public EmployeeModel PrepareEmployeeViewModel(Employee employee)
        {
            return new EmployeeModel
            {
                Id = employee.Id,
                NameEng = employee.NameEng,
                NameArabic = employee.NameArabic,
                Status = employee.Status,
                CompanyId = employee.CompanyId,
                FileNumber = employee.FileNumber,
                MobileNumber = employee.MobileNumber,
                Email = employee.Email,
                PhotoPath = employee.PhotoPath,
                Password = employee.Password, // Map the password for initial creation or edit
                MobileType = employee.MobileType,
                DeviceId = employee.DeviceId,
                RegistrationVia = employee.RegistrationType,
                RegisterStatus = employee.RegisterStatus,
                CreatedDate = employee.CreatedDate,
                QrCode = GenerateQrCode(employee.Email)
            };
        }

        /// <summary>
        /// Prepares a collection of EmployeeModel ViewModels from a collection of Employee domain objects.
        /// </summary>
        /// <param name="employees">The collection of Employee domain objects.</param>
        /// <returns>The collection of ViewModel EmployeeModels.</returns>
        public IEnumerable<EmployeeModel> PrepareEmployeeViewModels(IEnumerable<Employee> employees)
        {
            return employees.Select(PrepareEmployeeViewModel);
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
