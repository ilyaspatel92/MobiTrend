using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobi.Data.Domain
{
    public class EmployeeAttendanceLogs : BaseEntity
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }

        public DateTime DateandTime { get; set; }

        public int TransTypeId { get; set; }

        public string CurrentLocation { get; set; }
        public int ProofTypeId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longtitude { get; set; }
        public string MobileSerialNumber { get; set; }
        public string Photo { get; set; }
        public int LocationId { get; set; }
        public bool Transferred { get; set; }
        public DateTime TransferTime { get; set; }
        public int LocationBeaconMappingId { get; set; }
    }
}
