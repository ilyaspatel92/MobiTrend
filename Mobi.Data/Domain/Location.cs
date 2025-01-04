using System.ComponentModel.DataAnnotations.Schema;

namespace Mobi.Data.Domain
{
    public class Location : BaseEntity
    {
        public string LocationNameEnglish { get; set; }
        public string LocationNameArabic { get; set; }
        public bool Status { get; set; }
        public int ProofType { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? GPSLocationAddress { get; set; }

        [Column(TypeName = "decimal(18, 15)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(18, 15)")]
        public decimal Longitude { get; set; }
        public decimal SetRadius { get; set; }
        public string SetPolygon { get; set; }
        public int CompanyId { get; set; }
    }
}
