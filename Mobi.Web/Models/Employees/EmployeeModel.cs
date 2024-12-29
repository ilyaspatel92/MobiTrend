using System;
using System.ComponentModel.DataAnnotations;
using Mobi.Data;
using Mobi.Web.Utilities.Validations;

namespace Mobi.Web.Models.Employees
{
    public class EmployeeModel : BaseEntity
    {
        [Required(ErrorMessage = "Name (English) is required.")]
        [StringLength(100, ErrorMessage = "Name (English) cannot exceed 100 characters.")]
        public string NameEng { get; set; }

        [Required(ErrorMessage = "Name (Arabic) is required.")]
        [StringLength(100, ErrorMessage = "Name (Arabic) cannot exceed 100 characters.")]
        public string NameArabic { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public bool Status { get; set; }

        [StringLength(50, ErrorMessage = "File Number cannot exceed 50 characters.")]
        public string FileNumber { get; set; }

        [Required(ErrorMessage = "Please Enter Mobile Number.")]        
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Please provide a valid mobile number.")]

        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [CustomEmail(ErrorMessage = "Please provide a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; }
        public string? UserName { get; set; }
        
        public string? PhotoPath { get; set; }

        [Required(ErrorMessage = "Cid is required.")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "CID Should have 12 characters.")]
        public string CID { get; set; }


        //[Required(ErrorMessage = "Password is required.")]
        //[StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters.")]
        public string? Password { get; set; }

        public int MobileType { get; set; }
        public int RegistrationVia { get; set; }

        public string? DeviceId { get; set; }

        public bool RegisterStatus { get; set; }
        public string CompanyId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? QrCode { get; set; }

    }
}
