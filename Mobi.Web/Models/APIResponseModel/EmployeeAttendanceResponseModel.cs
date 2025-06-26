using Mobi.Data;

namespace Mobi.Web.Models.APIModels
{
    public class EmployeeAttendanceResponseModel:BaseEntity
    {
        public int EmployeeId { get; set; }
        public string? LocationName { get; set; }
        public int LocationId { get; set; }
        public DateTime AttendanceDateTime { get; set; }
        public DateTime LocalTimeAttendanceDateTime { get; set; }
        public int ActionType { get; set; }
        public int ActionTypeMode { get; set; }
        public DateTime TransferDateTime { get; set; }

        public bool IsFreeLocation { get; set; }

    }
}
