using Mobi.Data.Domain;

namespace Mobi.Web.Models.APIModels
{
    public class EmployeeAttendanceResponseModel
    {
        public EmployeeAttendanceResponseModel()
        {
            employeeAttendanceModels = new List<EmployeeAttendanceLogs>();
        }
        public IList<EmployeeAttendanceLogs> employeeAttendanceModels { get; set; }
    }
}
