using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Repository;
using Mobi.Service.Employees;
using Mobi.Service.Locations;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models.EmployeeAttendance;
using static QRCoder.PayloadGenerator;

namespace Mobi.Web.Controllers
{
    public class ReportsController : BasePublicController
    {
        #region Fields

        private readonly IEmployeeService _employeeService;
        private readonly IEmployeeFactory _employeeFactory;
        private readonly IRepository<EmployeeAttendanceLogs> _attendanceRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly ILocationService _locationService;
        #endregion

        #region Ctor
        public ReportsController(IEmployeeFactory employeeFactory,
                                 IEmployeeService employeeService,
                                 IRepository<EmployeeAttendanceLogs> attendanceRepository,
                                 IRepository<Employee> employeeRepository,
                                 ILocationService locationService)
        {
            _employeeFactory = employeeFactory;
            _employeeService = employeeService;
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _locationService = locationService;
        }
        #endregion

        #region Methods

        #region Employee Data Report

        [HttpGet]
        public IActionResult EmployeeData()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetEmployeeDataReport()
        {
            var employees = _employeeService.GetAllEmployees();
            var employeeViewModels = _employeeFactory.PrepareEmployeeViewModels(employees);

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = employees.Count(),
                recordsFiltered = employees.Count(),
                data = employeeViewModels
            });
        }

        #endregion

        #region Daily Transaction Reports

        [HttpGet]
        public IActionResult DailyAttendanceReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetDailyAttendanceData(DateTime? startDate, DateTime? endDate,int EmployeeId)
        {
            var query = _attendanceRepository.GetAll()
        .Join(_employeeRepository.GetAll(),
              log => log.EmployeeId,
              emp => emp.Id,
              (log, emp) => new
              {
                  log,
                  emp.NameEng,
                  emp.Id,
              });

            if (startDate.HasValue)
                query = query.Where(entry => entry.log.AttendanceDateTime.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                query = query.Where(entry => entry.log.AttendanceDateTime.Date <= endDate.Value.Date);
            if (EmployeeId > 0)
                query = query.Where(entry => entry.Id == EmployeeId);

            var attendanceLogs = query.Select(entry => new EmployeeAttendanceLogModel
            {
                Id = entry.log.Id,
                EmployeeName = entry.NameEng,
                Date = entry.log.AttendanceDateTime.ToString("dd/MM/yyyy"),
                Time = entry.log.AttendanceDateTime.ToString("hh:mm tt"),
                ActionTypeId = entry.log.ActionTypeId,
                ActionTypeName = GetActionTypeName(entry.log.ActionTypeId),
                ProofType = GetProofType(entry.log.ProofTypeId),
                Location = _locationService.GetLocationById(Convert.ToInt32(entry.log.LocationId))?.LocationNameEnglish,
                ActionTypeStatus = entry.log.ActionTypeId == 1
            }).ToList();


            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = query.Count(),
                recordsFiltered = attendanceLogs.Count(),
                data = attendanceLogs
            });
        }

        private string GetActionTypeName(int actionTypeId)
        {
            return actionTypeId switch
            {
                0 => "None",
                1 => "IN",
                2 => "OUT",
                3 => "Rejected"
            };
        }

        private string GetActionTypeClass(int actionTypeId)
        {
            return actionTypeId switch
            {
                0 => "None",
                1 => "badge-custom-in",
                2 => "badge-custom-out",
                3 => "badge-custom-rejected"
            };
        }

        private string GetProofType(int proofTypeId)
        {
            return proofTypeId switch
            {
                0 => "None",
                1 => "GPS",
                2 => "Beacon"
            };
        }


        #endregion

        #region Total Working Hours Reports

        [HttpGet]
        public IActionResult TotalWorkingHours()
        {
            return View();
        }

        #endregion

        #region Registered Phones Reports

        [HttpGet]
        public IActionResult RegisteredPhones()
        {
            return View();
        }

        #endregion

        #region Employees Location Report

        [HttpGet]
        public IActionResult EmployeesLocation()
        {
            return View();
        }

        #endregion

        #endregion
    }
}