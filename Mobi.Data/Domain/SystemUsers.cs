using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobi.Data.Domain
{
    public class SystemUsers : BaseEntity
    {
        [Required]
        public string EmployeeName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public bool UserStatus { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public int CompanyID { get; set; }
        
        public DateTime CreatedDate { get; set; }

        [Required]
        public bool Deleted { get; set; }

        public string PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }
    }
}
