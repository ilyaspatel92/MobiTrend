namespace Mobi.Web.Models.Reports
{
    public class DailyWorkingHoursDto
    {
        public string Date { get; set; }
        public string Day { get; set; }
        public int EmployeeId { get; set; }
        public string FileNumber { get; set; }
        public string EmployeeName { get; set; }
        public int TotalHours { get; set; }
        public int TotalMinutes { get; set; }
        public int TotalTransactions { get; set; }
        public string Notes { get; set; }
    }

}
