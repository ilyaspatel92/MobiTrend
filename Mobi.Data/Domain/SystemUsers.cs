using System.ComponentModel.DataAnnotations;

namespace Mobi.Data.Domain
{
    public class SystemUsers : BaseEntity
    {
        [Required]
        public string EmployeeName { get; set; }

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

    }
}
