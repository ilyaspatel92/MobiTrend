using System.ComponentModel.DataAnnotations;

namespace Mobi.Data.Domain
{
    public class Company : BaseEntity
    {
        [Required]
        public string CompanyName { get; set; }

        [Required]
        public string CompanyId { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsAllowedPhoto { get; set; }
        
    }
}
