namespace Mobi.Web.Models.EmployeeAttendance
{

    public class EmployeeAttendanceModel
    {
        public EmployeeAttendanceModel()
        {
            AttendanceLogs = new List<EmployeeAttendanceLogModel>();
        }
        public DateTime StartDate { get; set; } = DateTime.Now.Date;
        public DateTime EndDate { get; set; } = DateTime.Now.Date;
        public List<EmployeeAttendanceLogModel> AttendanceLogs { get; set; }
    }

    public class EmployeeAttendanceLogModel
    {
        public string EmployeeName { get; set; }
        public string DateAndTime { get; set; }
        public string ActionTypeName { get; set; }
        public string ActionTypeClass { get; set; }
        public string ProofType { get; set; }
        public string Location { get; set; }
    }
}
