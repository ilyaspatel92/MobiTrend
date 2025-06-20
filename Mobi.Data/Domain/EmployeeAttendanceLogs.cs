﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Mobi.Data.Domain
{
    public class EmployeeAttendanceLogs : BaseEntity
    {
        public int EmployeeId { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public DateTime LocalTimeAttendanceDateTime { get; set; }
        public int ActionTypeId { get; set; }
        public bool ActionTypeStatus { get; set; }
        public int ProofTypeId { get; set; }
        public bool IsVerifiedLocation { get; set; }
        [Column(TypeName = "decimal(18, 9)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(18, 9)")]
        public decimal Longtitude { get; set; }
        public string MobileSerialNumber { get; set; }
        public int PictureId { get; set; }
        public int LocationId { get; set; }
        public bool Transferred { get; set; }
        public string CurrentLocation { get; set; }
        public DateTime TransferDateTime { get; set; }
        public int LocationBeaconMappingId { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
