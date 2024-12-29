using Mobi.Data.Domain;

namespace Mobi.Service.EmployeeAttendances
{
    public interface IEmployeeAttendanceService
    {
        IEnumerable<EmployeeAttendanceLogs> GetAllLogs();
        EmployeeAttendanceLogs GetLogById(int id);
        void AddLog(EmployeeAttendanceLogs log);
        void UpdateLog(EmployeeAttendanceLogs log);
        void RemoveLog(EmployeeAttendanceLogs log);
        IEnumerable<EmployeeAttendanceLogs> GetLogsByEmployeeId(int employeeId);
        IEnumerable<EmployeeAttendanceLogs> GetLogsByDateRange(DateTime startDate, DateTime endDate);
        bool IsLogVerified(int id);
        IEnumerable<EmployeeAttendanceLogs> GetLogsByLocation(int locationId);
    }
}
