namespace Mobi.Data.Domain
{
    public class EmployeeAttendanceLogs : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime DateandTime { get; set; }
        public int ActionTypeId { get; set; }
        public int ActionTypeModeId { get; set; }
        public bool IsVerifiedLocation { get; set; }
        public string CurrentLocation { get; set; }
        public int ProofTypeId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public string MobileSerialNumber { get; set; }
        public int PictureId { get; set; }
        public int LocationId { get; set; }
        public bool Transferred { get; set; }
        public DateTime TransferTime { get; set; }
        public int LocationBeaconMappingId { get; set; }
    }
}
