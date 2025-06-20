﻿using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Mobi.Data.Domain;
using Mobi.Data.Domain.Employees;
using Mobi.Data.Enums;
using Mobi.Repository;
using Mobi.Service.EmployeeLocationServices;
using Mobi.Service.Employees;
using Mobi.Service.Locations;
using Mobi.Web.Factories.Employees;
using Mobi.Web.Models.EmployeeAttendance;
using Mobi.Web.Models.Employees;
using Mobi.Web.Models.Reports;
using Mobi.Web.Utilities.PDF;
using QuestPDF.Fluent;
using static Azure.Core.HttpHeader;
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
        private readonly IRepository<Location> _locationRepository;
        private readonly IEmployeeLocationService _employeeLocationService;

        #endregion

        #region Ctor
        public ReportsController(IEmployeeFactory employeeFactory,
                                 IEmployeeService employeeService,
                                 IRepository<EmployeeAttendanceLogs> attendanceRepository,
                                 IRepository<Employee> employeeRepository,
                                 ILocationService locationService,
                                 IRepository<Location> locationRepository,
                                 IEmployeeLocationService employeeLocationService)
        {
            _employeeFactory = employeeFactory;
            _employeeService = employeeService;
            _attendanceRepository = attendanceRepository;
            _employeeRepository = employeeRepository;
            _locationService = locationService;
            _locationRepository = locationRepository;
            _employeeLocationService = employeeLocationService;            
        }
        #endregion

        #region Methods

        #region Daily Attendance Reports
        private List<EmployeeAttendanceLogModel> GetAttendanceLogs(DateTime? startDate, DateTime? endDate, int employeeId)
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
                          emp.FileNumber
                      });

            if (startDate.HasValue)
                query = query.Where(entry => entry.log.AttendanceDateTime.Date >= startDate.Value.Date);
            if (endDate.HasValue)
                query = query.Where(entry => entry.log.AttendanceDateTime.Date <= endDate.Value.Date);
            if (employeeId > 0)
                query = query.Where(entry => entry.Id == employeeId);

            var kuwaitTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");

            return query.Select(entry =>
            {
                var kuwaitTime = TimeZoneInfo.ConvertTimeFromUtc(entry.log.AttendanceDateTime, kuwaitTimeZone);

                return new EmployeeAttendanceLogModel
                {
                    Id = entry.log.Id,
                    FileNumber = entry.FileNumber,
                    EmployeeName = entry.NameEng,
                    Date = kuwaitTime.ToString("dd/MM/yyyy"),
                    Time = kuwaitTime.ToString("hh:mm tt"),
                    ActionTypeId = entry.log.ActionTypeId,
                    ActionTypeName = GetActionTypeName(entry.log.ActionTypeId),
                    ProofType = GetProofType(entry.log.ProofTypeId),
                    Location = entry.log.CurrentLocation,
                    ActionTypeStatus = entry.log.ActionTypeId == 1
                };
            }).ToList();
        }


        [HttpGet]
        public IActionResult DailyAttendanceReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetDailyAttendanceData(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var logs = GetAttendanceLogs(startDate, endDate, employeeId);

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = logs.Count,
                recordsFiltered = logs.Count,
                data = logs
            });
        }

        [HttpGet]
        public IActionResult DownloadDailyAttendancePdf(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var logs = GetAttendanceLogs(startDate, endDate, employeeId); // Your method
            var employeeName = logs.FirstOrDefault()?.EmployeeName ?? "Unknown";

            //var report = new DailyAttendanceReport(logs, $"Daily Attendance Report - {employeeName}", "USER1");
            var report = new DailyAttendanceReport(
            logs,
           title: $"Daily Attendance Report - {employeeName}",
           printedBy: User.Identity.Name,
           _dateRangeText: $"From {startDate.Value.ToString("dd/MM/yy")} To {endDate.Value.ToString("dd/MM/yy")}",
        isRtl: false           
       );

            using var stream = new MemoryStream();
            report.GeneratePdf(stream);
            return File(stream.ToArray(), "application/pdf", $"AttendanceReport_{employeeName}_{DateTime.Now:yyyyMMdd}.pdf");
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

        #region Registered Phones Reports

        [HttpGet]
        public IActionResult RegisteredPhones()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetRegisteredPhones()
        {
            var employees = _employeeService.GetAllEmployees().ToList();

            var employeeViewModels = employees.Select(emp =>
            {
                var employeeModel = new EmployeeModel
                {
                    FileNumber = emp.FileNumber,
                    NameEng = emp.NameEng,
                    Email = emp.Email,
                    MobileType = emp.MobileType,
                    MobileTypeName = Enum.GetName(typeof(MobileType), emp.MobileType),
                    MobRegistrationDate = emp.MobRegistrationDate?.ToLocalTime().ToString("dd/MM/yyyy")
                };

                // Fetch last transaction date from attendance records
                var lastTransaction = _attendanceRepository.GetAll().Where(x => x.EmployeeId == emp.Id).OrderBy(x => x.CreatedDateTime).LastOrDefault();
                employeeModel.LastTransactionDate = lastTransaction?.CreatedDateTime.ToString("dd/MM/yyyy");

                return employeeModel;
            }).ToList();

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = employeeViewModels.Count(),
                recordsFiltered = employeeViewModels.Count(),
                data = employeeViewModels
            });
        }


        #endregion

        #region Monthly working hours Reports

        [HttpGet]
        public IActionResult MonthlyWorkingHours()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetMonthlyWorkingHours(int year, int month, int? employeeId)
        {
            var query = _attendanceRepository.GetAll()
                .Where(log => log.LocalTimeAttendanceDateTime.Year == year && log.LocalTimeAttendanceDateTime.Month == month)
                .Join(_employeeRepository.GetAll(),
                      log => log.EmployeeId,
                      emp => emp.Id,
                      (log, emp) => new { log, emp.NameEng, emp.FileNumber, emp.Id })
                .OrderBy(entry => entry.Id)
                .ThenBy(entry => entry.log.LocalTimeAttendanceDateTime);

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.Id == employeeId.Value)
                             .OrderBy(entry => entry.Id)
                             .ThenBy(entry => entry.log.LocalTimeAttendanceDateTime);
            }

            var groupedData = query
                .AsEnumerable()
                .GroupBy(entry => new { entry.Id, entry.NameEng, entry.FileNumber })
                .Select(group =>
                {
                    var logs = group.OrderBy(x => x.log.LocalTimeAttendanceDateTime).ToList();
                    var groupedByDate = logs.GroupBy(x => x.log.LocalTimeAttendanceDateTime.Date);
                    int totalMinutes = 0;
                    bool hasMissingOut = false;

                    foreach (var dateGroup in groupedByDate)
                    {
                        var dayLogs = dateGroup.OrderBy(x => x.log.LocalTimeAttendanceDateTime).ToList();
                        var ins = new Queue<DateTime>();
                        var outs = new Queue<DateTime>();

                        foreach (var entry in dayLogs)
                        {
                            if (entry.log.ActionTypeId == 1)
                                ins.Enqueue(entry.log.LocalTimeAttendanceDateTime);
                            else if (entry.log.ActionTypeId == 2)
                                outs.Enqueue(entry.log.LocalTimeAttendanceDateTime);
                        }

                        while (ins.Count > 0 && outs.Count > 0)
                        {
                            var inTime = ins.Dequeue();
                            var possibleOut = outs.FirstOrDefault(outTime => outTime > inTime && outTime.Date == inTime.Date);
                            if (possibleOut != default)
                            {
                                outs = new Queue<DateTime>(outs.Where(x => x != possibleOut));
                                var duration = (possibleOut - inTime).TotalMinutes;

                                int rounded = 0;
                                if (duration > 0 && duration < 1)
                                    rounded = 1;
                                else if (duration >= 1)
                                    rounded = (int)Math.Floor(duration); // ✅ Use Floor to avoid overcounting

                                totalMinutes += rounded;
                            }
                            else
                            {
                                hasMissingOut = true;
                                break;
                            }
                        }

                        if (ins.Count > 0 || outs.Count > 0)
                        {
                            hasMissingOut = true;
                        }
                    }

                    return new
                    {
                        EmployeeId = group.Key.FileNumber,
                        EmployeeName = group.Key.NameEng,
                        TotalHours = totalMinutes / 60,
                        TotalMinutes = totalMinutes % 60,
                        Notes = hasMissingOut ? "Missing OUT" : ""
                    };
                })
                .ToList();

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = groupedData.Count,
                recordsFiltered = groupedData.Count,
                data = groupedData
            });
        }



        #endregion

        #region Daily working hours report
        [HttpGet]
        public IActionResult DailyWorkingHours()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetDailyWorkingHours(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var query = _attendanceRepository.GetAll()
                .Where(log => (!startDate.HasValue || log.AttendanceDateTime.Date >= startDate.Value.Date)
                              && (!endDate.HasValue || log.AttendanceDateTime.Date <= endDate.Value.Date)
                              && (employeeId <= 0 || log.EmployeeId == employeeId)) // Apply filter only if employeeId > 0
                .Join(_employeeRepository.GetAll(),
                      log => log.EmployeeId,
                      emp => emp.Id,
                      (log, emp) => new { log, emp.NameEng, emp.Id, emp.FileNumber })
                .ToList();

            var groupedData = query
                .GroupBy(entry => new { entry.Id, entry.NameEng, entry.FileNumber, entry.log.AttendanceDateTime.Date })
                .Select(group =>
                {
                    var logs = group.OrderBy(x => x.log.AttendanceDateTime).ToList();
                    int totalMinutes = 0;
                    int totalTransactions = logs.Count();
                    var inQueue = new Queue<DateTime>();

                    foreach (var entry in logs)
                    {
                        if (entry.log.ActionTypeId == 1) // IN
                        {
                            inQueue.Enqueue(entry.log.AttendanceDateTime);
                        }
                        else if (entry.log.ActionTypeId == 2 && inQueue.Count > 0) // OUT
                        {
                            var inTime = inQueue.Dequeue();
                            totalMinutes += (int)Math.Round((entry.log.AttendanceDateTime - inTime).TotalMinutes);
                        }
                    }

                    bool hasMissingOut = inQueue.Count > 0;

                    return new DailyWorkingHoursDto
                    {
                        Date = group.Key.Date.ToString("dd/MM/yyyy"),
                        Day = group.Key.Date.DayOfWeek.ToString(),
                        EmployeeId = group.Key.Id,
                        EmployeeName = group.Key.NameEng,
                        FileNumber = group.Key.FileNumber,
                        TotalHours = totalMinutes / 60,
                        TotalMinutes = totalMinutes % 60,
                        TotalTransactions = totalTransactions,
                        Notes = hasMissingOut ? "Missing OUT" : ""
                    };
                })
                .ToList();

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = groupedData.Count(),
                recordsFiltered = groupedData.Count(),
                data = groupedData
            });
        }







        #endregion

        #region Daily Attendance Reports by Location

        public IActionResult DailyAttendanceReportbyLocation()
        {
            return View();
        }

        public IActionResult GetDailyAttendanceReportbyLocation(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var query = _attendanceRepository.GetAll()
                        .Where(log => (!startDate.HasValue || log.AttendanceDateTime.Date >= startDate.Value.Date)
                                      && (!endDate.HasValue || log.AttendanceDateTime.Date <= endDate.Value.Date)
                                      && log.EmployeeId == employeeId)
                        .Join(_employeeRepository.GetAll(),
                              log => log.EmployeeId,
                              emp => emp.Id,
                              (log, emp) => new { log, emp.NameEng, emp.Id })
                        .Join(_locationRepository.GetAll(),
                              log => log.log.LocationId,
                              loc => loc.Id,
                              (log, loc) => new { log, loc.LocationNameEnglish });

            var groupedData = query.AsEnumerable()
                .GroupBy(entry => new { entry.log.Id, entry.log.NameEng, entry.log.log.AttendanceDateTime.Date, entry.LocationNameEnglish })
                .SelectMany(group => group.OrderBy(x => x.log.log.AttendanceDateTime)
                .Select((entry, index) => new
                {
                    SerialNumber = index + 1,
                    Date = group.Key.Date.ToString("dd/MM/yyyy"),
                    EmployeeId = group.Key.Id,
                    EmployeeName = group.Key.NameEng,
                    Location = group.Key.LocationNameEnglish,
                    Time = entry.log.log.AttendanceDateTime.ToString("hh:mm tt"),
                    EventType = entry.log.log.ActionTypeId == 1 ? "IN" : "OUT",
                    ProofType = entry.log.log.ProofTypeId == 1 ? "beacon" : "GPS"
                })).ToList();

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = groupedData.Count,
                recordsFiltered = groupedData.Count,
                data = groupedData
            });
        }



        #endregion

        #region Employee Location Authority Report

        public IActionResult EmployeeLocationAuthorityReport()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetEmployeeLocationAuthorityReport()
        {
            var allEmployees = _employeeService.GetAllEmployees();
            var allEmployeeLocations = _employeeLocationService.GetAllEmployeeLocations();
            var allLocations = _locationService.GetAllLocations();

            var reportData = allEmployees
                .Select(emp =>
                {
                    var empLocationLinks = allEmployeeLocations
                        .Where(el => el.EmployeeId == emp.Id)
                        .ToList();

                    var empLocations = empLocationLinks
                        .Join(allLocations,
                            el => el.LocationId,
                            loc => loc.Id,
                            (el, loc) => new
                            {
                                loc.LocationNameEnglish,
                                loc.ProofType
                            })
                        .ToList();

                    var locationNames = empLocations.Select(l => l.LocationNameEnglish).ToList();
                    var isFreeLocation = empLocationLinks.Any(x => x.IsFreeLocation);

                    return new
                    {
                        FileNumber = emp.FileNumber,
                        EmployeeName = emp.NameEng,
                        LocationName = locationNames.Any() ? string.Join(", ", locationNames) : "",
                        ProofType = isFreeLocation ? "Free Location" : "GPS"
                    };
                })
                .ToList();

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = allEmployees.Count(),
                recordsFiltered = reportData.Count(),
                data = reportData
            });
        }





        #endregion

        #endregion
    }
}