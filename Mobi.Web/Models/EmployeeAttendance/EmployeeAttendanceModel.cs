namespace Mobi.Web.Models.EmployeeAttendance
{

    public class EmployeeAttendanceModel
    {
        public EmployeeAttendanceModel()
        {
            AttendanceLogs = new List<EmployeeAttendanceLogModel>();
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<EmployeeAttendanceLogModel> AttendanceLogs { get; set; }
    }

    public class EmployeeAttendanceLogModel
    {
        public int Id { get; set; }
        public string FileNumber { get; set; }
        public string EmployeeName { get; set; }
        public string DateAndTime { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string ActionTypeName { get; set; }
        public int ActionTypeId { get; set; }
        public string ActionTypeClass { get; set; }
        public string TransStatusName { get; set; }
        public string ProofType { get; set; }
        public string Location { get; set; }
        public string GoogleMapsUrl { get; set; }
        public bool ActionTypeStatus { get; set; }
    }
}
