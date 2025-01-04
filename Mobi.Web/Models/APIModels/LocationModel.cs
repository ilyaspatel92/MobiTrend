using Mobi.Web.Models.LocationBeacons;

namespace Mobi.Web.Models.APIModels
{
    public class LocationModel
    {
        public LocationModel()
        {
            BeaconList = new List<LocationBeaconModel>();
        }
        public int Id { get; set; }
        public string LocationNameEnglish { get; set; }
        public string LocationNameArabic { get; set; }
        public bool Status { get; set; }
        public bool BeaconProof { get; set; }
        public bool GPSProof { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? GPSLocationAddress { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public decimal SetRadius { get; set; }
        public int CompanyId { get; set; }
        public IList<LocationBeaconModel> BeaconList { get; set; }
    }
}
