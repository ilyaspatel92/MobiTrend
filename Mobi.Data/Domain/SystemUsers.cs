using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class SystemUsers : BaseEntity
    {
        [Required]
        public string? EmployeeName { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? UserStatus { get; set; }

        [Required]
        public string? Password { get; set; }

        [Required]
        public int CompanyID { get; set; }

        [Required]
        public int CreatedDate { get; set; }

        [Required]
        public bool Deleted { get; set; }

    }
}
