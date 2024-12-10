using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class Locations : BaseEntity
    {
        public int Id { get; set; }
        public string LocationNameEnglish { get; set; }
        public string LocationNameArabic { get; set; }
        public bool Status { get; set; }
        public int BeaconProof { get; set; }
        public DateTime CreatedDate { get; set; }
        public string GPSLocationAddress { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public decimal SetRadius { get; set; }
        public int CompanyId { get; set; }
    }
}
