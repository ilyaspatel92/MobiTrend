using Mobi.Data;
using System.ComponentModel.DataAnnotations;

namespace Mobi.Web.Models.Locations
{
    public class LocationModel : BaseEntity
    {
        [Required(ErrorMessage = "Location Name (English) is required.")]
        public string LocationNameEnglish { get; set; }

        [Required(ErrorMessage = "Location Name (Arabic) is required.")]
        public string LocationNameArabic { get; set; }

        public bool Status { get; set; }

        [Required(ErrorMessage = "Beacon Proof/GPS Proof is required.")]
        public int BeaconProofGPSProof { get; set; }

        [Required(ErrorMessage = "Latitude is required.")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required.")]
        public decimal Longitude { get; set; }

        public decimal SetRadius { get; set; }
        public string GPSLocationAddress { get; set; }
        public int CompanyId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
