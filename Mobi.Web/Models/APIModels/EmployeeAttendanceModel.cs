using System.ComponentModel.DataAnnotations.Schema;

namespace Mobi.Web.Models.APIModels
{
    public class EmployeeAttendanceModel 
    {
        public int LocationId { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public int BeaconId { get; set; }
        [Column(TypeName = "decimal(18, 9)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(18, 9)")]
        public decimal Longitude { get; set; }
        public int PictureId { get; set; }
        public int ActionType { get; set; }
        public string MobileSerialNumber { get; set; }
        public int ActionTypeMode { get; set; }
        public bool IsverifiedLocation { get; set; }

        public DateTime TransferDateTime { get; set; }

    }
}
