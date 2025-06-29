namespace Mobi.Web.Models.Reports
{
    public class MonthlyWorkingHoursWrapper
    {
        public List<MonthlyWorkingHoursDto> Data { get; set; }
    }

    public class MonthlyWorkingHoursDto
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int TotalHours { get; set; }
        public int TotalMinutes { get; set; }
        public string Notes { get; set; }
    }

}
