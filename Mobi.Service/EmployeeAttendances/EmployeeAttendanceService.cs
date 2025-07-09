using Mobi.Data.Domain;
using Mobi.Repository;

namespace Mobi.Service.EmployeeAttendances
{
    public class EmployeeAttendanceService : IEmployeeAttendanceService

    {
        private readonly IRepository<EmployeeAttendanceLogs> _attendanceRepository;

        public EmployeeAttendanceService(IRepository<EmployeeAttendanceLogs> attendanceRepository)
        {
            _attendanceRepository = attendanceRepository;
        }

        public IEnumerable<EmployeeAttendanceLogs> GetAllLogs()
        {
            return _attendanceRepository.GetAll();
        }

        public EmployeeAttendanceLogs GetLogById(int id)
        {
            return _attendanceRepository.GetById(id);
        }

        public void AddLog(EmployeeAttendanceLogs log)
        {
            _attendanceRepository.Insert(log);
        }

        public void UpdateLog(EmployeeAttendanceLogs log)
        {
            _attendanceRepository.Update(log);
        }

        public void RemoveLog(EmployeeAttendanceLogs log)
        {
            _attendanceRepository.Delete(log);
        }

        public IEnumerable<EmployeeAttendanceLogs> GetLogsByEmployeeId(int employeeId)
        {
            return _attendanceRepository.GetAll().Where(l => l.EmployeeId == employeeId).ToList();
        }

        public IEnumerable<EmployeeAttendanceLogs> GetLogsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _attendanceRepository
                .GetAll()
                .Where(l => l.AttendanceDateTime.ConvertToUserTime() >= startDate && l.AttendanceDateTime.ConvertToUserTime() <= endDate)
                .ToList();
        }

        public bool IsLogVerified(int id)
        {
            var log = _attendanceRepository.GetById(id);
            return log != null && log.IsVerifiedLocation;
        }

        public IEnumerable<EmployeeAttendanceLogs> GetLogsByLocation(int locationId)
        {
            return _attendanceRepository.GetAll().Where(l => l.LocationId == locationId).ToList();
        }
    }
}
