using System.ComponentModel.DataAnnotations;
using Mobi.Data;
using Mobi.Web.Utilities.Validations;

namespace Mobi.Web.Models.SystemUser
{
    public class RegisterModel : BaseEntity
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string EmployeeName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [CustomEmail(ErrorMessage = "Please provide a valid email address.")]
        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        public string Email { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public bool UserStatus { get; set; }
    }
}
