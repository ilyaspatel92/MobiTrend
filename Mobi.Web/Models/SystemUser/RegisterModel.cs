using System.ComponentModel.DataAnnotations;

namespace Mobi.Web.Models.SystemUser
{
    public class RegisterModel
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string EmployeeName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public bool UserStatus { get; set; }
    }
}
