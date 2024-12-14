namespace Mobi.Data.Domain
{
    public class LocationBeaconMapping : BaseEntity
    {
        public int LocationId { get; set; }
        public string BeaconName { get; set; }
        public Guid UUID { get; set; }
        public bool Status  { get; set; }
    }
}
