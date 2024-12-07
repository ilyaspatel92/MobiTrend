using System;
using System.ComponentModel.DataAnnotations;

namespace Mobi.Web.Models.Employees
{
    public class EmployeeModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name (English) is required.")]
        [StringLength(100, ErrorMessage = "Name (English) cannot exceed 100 characters.")]
        public string NameEng { get; set; }

        [StringLength(100, ErrorMessage = "Name (Arabic) cannot exceed 100 characters.")]
        public string? NameArabic { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public bool Status { get; set; }

        [StringLength(50, ErrorMessage = "File Number cannot exceed 50 characters.")]
        public string? FileNumber { get; set; }

        [Phone(ErrorMessage = "Please provide a valid mobile number.")]
        public string? MobileNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string? Email { get; set; }

        public string? PhotoPath { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 50 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string UserName { get; set; }

        public int? MobileType { get; set; } // Enum value, optional.

        //[StringLength(50, ErrorMessage = "Registration method cannot exceed 50 characters.")]
        public string? RegistrationVia { get; set; }

        //[StringLength(100, ErrorMessage = "Device ID cannot exceed 100 characters.")]
        public string? DeviceId { get; set; }

        //[StringLength(50, ErrorMessage = "Register Status cannot exceed 50 characters.")]
        public string? RegisterStatus { get; set; }

        [Required(ErrorMessage = "Company ID is required.")]
        public string? CompanyId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
