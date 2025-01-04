using Mobi.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mobi.Web.Models.Locations
{
    public class LocationModel : BaseEntity
    {
        [Required(ErrorMessage = "Please Enter Location Name (English).")]
        public string? LocationNameEnglish { get; set; }

        [Required(ErrorMessage = "Please Enter Location Location Name (Arabic).")]
        public string? LocationNameArabic { get; set; }

        public bool Status { get; set; }

        public int ProofType { get; set; }
        public string? ProofTypeName { get; set; }

        //[Required(ErrorMessage = "Latitude is required.")]
        [Column(TypeName = "decimal(18, 15)")]
        public decimal Latitude { get; set; }

        //[Required(ErrorMessage = "Longitude is required.")]
        [Column(TypeName = "decimal(18, 15)")]
        public decimal Longitude { get; set; }

        public decimal SetRadius { get; set; }
        public string? SetPolygon { get; set; }
        public string? GPSLocationAddress { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }

        

    }
}
