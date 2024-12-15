using Mobi.Data;

namespace Mobi.Web.Models.LocationBeacons
{
    public class LocationBeaconModel : BaseEntity
    {
        public string BeaconName { get; set; } 
        public Guid UUID { get; set; } 
        public bool Status { get; set; } 
        public int LocationId { get; set; }
    }
}
