using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Data.Enums;
using Mobi.Repository;
using Mobi.Service.AccessControls;
using Mobi.Service.Locations;
using Mobi.Web.Models.EmployeeAttendance;

namespace Mobi.Web.Controllers
{
    public class EmployeeAttendanceController : BasePublicController
    {
        private readonly IRepository<EmployeeAttendanceLogs> _attendanceRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ILocationService _locationService;
        private readonly IAccessControlService _accessControlService;
        public EmployeeAttendanceController(IRepository<EmployeeAttendanceLogs> attendanceRepository,
                                            IRepository<Employee> employeeRepository,
                                            ILocationService locationService,
                                            IAccessControlService accessControlService)
        {
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _locationService = locationService;
            _accessControlService = accessControlService;
        }

        [HttpGet]
        public IActionResult Logs(DateTime startDate, DateTime endDate, string employeeName, string employeeId)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.EmployeeAttendance));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            var attendanceLogs = _attendanceRepository.GetAll().ToList();
            var employees = _employeeRepository.GetAll().ToList();
            var joinedLogs = from log in attendanceLogs
                             join emp in employees on log.EmployeeId equals emp.Id
                             where log.AttendanceDateTime.Date >= startDate.Date && log.AttendanceDateTime.Date <= endDate.Date.Date
                             select new
                             {
                                 Log = log,
                                 EmployeeName = emp.NameEng
                             };
            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                joinedLogs = joinedLogs.Where(entry => entry.EmployeeName.Contains(employeeName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(employeeId) && int.TryParse(employeeId, out int parsedEmployeeId))
            {
                joinedLogs = joinedLogs.Where(entry => entry.Log.EmployeeId == parsedEmployeeId);
            }

            var viewModel = joinedLogs.Select(entry => new EmployeeAttendanceLogModel
            {
                EmployeeName = entry.EmployeeName,
                DateAndTime = entry.Log.AttendanceDateTime.ToString("MM/dd/yyyy @ hh:mm tt"),
                ActionTypeName = GetActionTypeName(entry.Log.ActionTypeId),
                ActionTypeClass = GetActionTypeClass(entry.Log.ActionTypeId),
                ProofType = GetProofType(entry.Log.ProofTypeId),
                Location = _locationService.GetLocationById(Convert.ToInt32(entry.Log.LocationId))?.LocationNameEnglish
            }).ToList();

            var model = new EmployeeAttendanceModel();

            model.AttendanceLogs = viewModel;
            return View(model);
        }

        [HttpGet]
        public IActionResult Search(DateTime startDate, DateTime endDate, string employeeName, string employeeId)
        {
            bool hasAccess = _accessControlService.HasAccess(nameof(ScreenAuthorityEnum.Locations));

            if (!hasAccess)
                return RedirectToAction("AccessDenied", "AccessControl");

            var attendanceLogs = _attendanceRepository.GetAll().ToList();
            var employees = _employeeRepository.GetAll().ToList();
            var joinedLogs = from log in attendanceLogs
                             join emp in employees on log.EmployeeId equals emp.Id
                             where log.AttendanceDateTime.Date >= startDate.Date && log.AttendanceDateTime.Date <= endDate.Date.Date
                             select new
                             {
                                 Log = log,
                                 EmployeeName = emp.NameEng
                             };
            if (!string.IsNullOrWhiteSpace(employeeName))
            {
                joinedLogs = joinedLogs.Where(entry => entry.EmployeeName.Contains(employeeName, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(employeeId) && int.TryParse(employeeId, out int parsedEmployeeId))
            {
                joinedLogs = joinedLogs.Where(entry => entry.Log.EmployeeId == parsedEmployeeId);
            }

            var viewModel = joinedLogs.Select(entry => new EmployeeAttendanceLogModel
            {
                EmployeeName = entry.EmployeeName,
                DateAndTime = entry.Log.AttendanceDateTime.ToString("MM/dd/yyyy @ hh:mm tt"),
                ActionTypeName = GetActionTypeName(entry.Log.ActionTypeId),
                ActionTypeClass = GetActionTypeClass(entry.Log.ActionTypeId),
                ProofType = GetProofType(entry.Log.ProofTypeId),
                Location = _locationService.GetLocationById(Convert.ToInt32(entry.Log.LocationId))?.LocationNameEnglish
            }).ToList();

            return PartialView("_AttendanceTable", viewModel);
        }

        // Helper methods for mapping ActionType and ProofType
        private string GetActionTypeName(int actionTypeId)
        {
            return actionTypeId switch
            {
                1 => "IN",
                2 => "OUT",
                3 => "Rejected"
            };
        }

        private string GetActionTypeClass(int actionTypeId)
        {
            return actionTypeId switch
            {
                1 => "badge-custom-in",
                2 => "badge-custom-out",
                3 => "badge-custom-rejected"
            };
        }

        private string GetProofType(int proofTypeId)
        {
            return proofTypeId switch
            {
                1 => "GPS",
                2 => "Beacon"
            };
        }



    }
}
