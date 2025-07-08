using System.Globalization;
using System.Text.Json;
using System.Xml.Linq;
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
                query = query.Where(entry => entry.log.AttendanceDateTime.ConvertToUserTime().Date >= startDate.Value.Date);
            if (endDate.HasValue)
                query = query.Where(entry => entry.log.AttendanceDateTime.ConvertToUserTime().Date <= endDate.Value.Date);
            if (employeeId > 0)
                query = query.Where(entry => entry.Id == employeeId);

            var kuwaitTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");

            return query.Select(entry =>
            {

                return new EmployeeAttendanceLogModel
                {
                    Id = entry.log.Id,
                    FileNumber = entry.FileNumber,
                    EmployeeName = entry.NameEng,
                    Date = entry.log.AttendanceDateTime.ConvertToUserTime().ToString("dd/MM/yyyy"),
                    Time = entry.log.AttendanceDateTime.ConvertToUserTime().ToString("hh:mm tt"),
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
            Response.Headers.Add("Content-Disposition", "inline; filename=DailyAttendancePdf.pdf");
            return File(stream.ToArray(), "application/pdf");
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

        [HttpGet]
        public IActionResult DownloadEmployeeDataPdf()
        {
            var employees = _employeeService.GetAllEmployees();
            var employeeViewModels = _employeeFactory.PrepareEmployeeViewModels(employees).ToList();

            var report = new EmployeeDataReport(
                employeeViewModels,
                title: "Employee Master Report",
                printedBy: User.Identity?.Name ?? "System"
            );

            using var stream = new MemoryStream();
            report.GeneratePdf(stream);

            Response.Headers.Add("Content-Disposition", "inline; filename=EmployeeDataPdf.pdf");
            return File(stream.ToArray(), "application/pdf");
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
                    MobRegistrationDate = emp.MobRegistrationDate?.ConvertToUserTime().ToString("dd/MM/yyyy")
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

        [HttpGet]
        public IActionResult DownloadRegisteredPhonesPdf()
        {
            var employees = _employeeService.GetAllEmployees().ToList();

            var employeeViewModels = employees.Select(emp =>
            {
                var model = new EmployeeModel
                {
                    FileNumber = emp.FileNumber,
                    NameEng = emp.NameEng,
                    Email = emp.Email,
                    MobileType = emp.MobileType,
                    MobileTypeName = Enum.GetName(typeof(MobileType), emp.MobileType),
                    MobRegistrationDate = emp.MobRegistrationDate?.ConvertToUserTime().ToString("dd/MM/yyyy")
                };

                var last = _attendanceRepository.GetAll()
                    .Where(x => x.EmployeeId == emp.Id)
                    .OrderByDescending(x => x.CreatedDateTime)
                    .FirstOrDefault();

                model.LastTransactionDate = last?.CreatedDateTime.ToString("dd/MM/yyyy");
                return model;
            }).ToList();

            var report = new RegisteredPhonesReport(
                employeeViewModels,
                title: "Registered Phones Report",
                printedBy: User.Identity?.Name ?? "SYSTEM"
            );

            using var stream = new MemoryStream();
            report.GeneratePdf(stream);

            Response.Headers.Add("Content-Disposition", "inline; filename=RegisteredPhonesPdf.pdf");
            return File(stream.ToArray(), "application/pdf");
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
                .Where(log => log.AttendanceDateTime.ConvertToUserTime().Year == year && log.AttendanceDateTime.ConvertToUserTime().Month == month)
                .Join(_employeeRepository.GetAll(),
                      log => log.EmployeeId,
                      emp => emp.Id,
                      (log, emp) => new { log, emp.NameEng, emp.FileNumber, emp.Id })
                .OrderBy(entry => entry.Id)
                .ThenBy(entry => entry.log.AttendanceDateTime.ConvertToUserTime());

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.Id == employeeId.Value)
                             .OrderBy(entry => entry.Id)
                             .ThenBy(entry => entry.log.AttendanceDateTime.ConvertToUserTime());
            }

            var groupedData = query
                .AsEnumerable()
                .GroupBy(entry => new { entry.Id, entry.NameEng, entry.FileNumber })
                .Select(group =>
                {
                    var logs = group.OrderBy(x => x.log.AttendanceDateTime.ConvertToUserTime()).ToList();
                    var groupedByDate = logs.GroupBy(x => x.log.AttendanceDateTime.ConvertToUserTime().Date);
                    int totalMinutes = 0;
                    bool hasMissingOut = false;

                    foreach (var dateGroup in groupedByDate)
                    {
                        var dayLogs = dateGroup.OrderBy(x => x.log.AttendanceDateTime.ConvertToUserTime()).ToList();
                        var ins = new Queue<DateTime>();
                        var outs = new Queue<DateTime>();

                        foreach (var entry in dayLogs)
                        {
                            if (entry.log.ActionTypeId == 1)
                                ins.Enqueue(entry.log.AttendanceDateTime.ConvertToUserTime());
                            else if (entry.log.ActionTypeId == 2)
                                outs.Enqueue(entry.log.AttendanceDateTime.ConvertToUserTime());
                        }

                        while (ins.Count > 0 && outs.Count > 0)
                        {
                            var inTime = ins.Dequeue();

                            // Find first out that is after inTime and same date
                            var possibleOut = outs.FirstOrDefault(outTime => outTime > inTime && outTime.Date == inTime.Date);

                            if (possibleOut != default)
                            {
                                // Remove used OUT
                                outs = new Queue<DateTime>(outs.Where(x => x != possibleOut));

                                // Trim seconds
                                var inTrimmed = new DateTime(inTime.Year, inTime.Month, inTime.Day, inTime.Hour, inTime.Minute, 0);
                                var outTrimmed = new DateTime(possibleOut.Year, possibleOut.Month, possibleOut.Day, possibleOut.Hour, possibleOut.Minute, 0);

                                var duration = outTrimmed - inTrimmed;
                                var minutes = duration.TotalMinutes;

                                // Apply rule: ignore <1 min or count as 1
                                int rounded = 0;
                                if (minutes > 0 && minutes < 1)
                                    rounded = 1;
                                else if (minutes >= 1)
                                    rounded = (int)Math.Ceiling(minutes); // ✅ Ceil for proper rounding

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


        [HttpGet]
        public IActionResult DownloadMonthlyWorkingHoursPdf(int year, int month, int? employeeId)
        {
            var result = GetMonthlyWorkingHours(year, month, employeeId) as JsonResult;

            if (result?.Value is null)
                return BadRequest("No data found.");

            // Serialize and deserialize to a known DTO
            var json = JsonSerializer.Serialize(result.Value);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var wrapper = JsonSerializer.Deserialize<MonthlyWorkingHoursWrapper>(json, options);

            if (wrapper?.Data == null || !wrapper.Data.Any())
                return BadRequest("No records to print.");

            var title = "Monthly Working Hours Report";
            var printedBy = User.Identity?.Name ?? "SYSTEM";
            var dateRange = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year}";

            var report = new MonthlyWorkingHoursReport(wrapper.Data, title, printedBy, dateRange, isRtl: false);

            using var stream = new MemoryStream();
            report.GeneratePdf(stream);
            Response.Headers.Add("Content-Disposition", "inline; filename=MonthlyWorkingHoursPdf.pdf");
            return File(stream.ToArray(), "application/pdf");
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
            var groupedData = GetDailyWorkingHoursData(startDate, endDate, employeeId);

            return Json(new
            {
                draw = Request.Query["draw"],
                recordsTotal = groupedData.Count(),
                recordsFiltered = groupedData.Count(),
                data = groupedData
            });
        }

        private List<DailyWorkingHoursDto> GetDailyWorkingHoursData(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var query = _attendanceRepository.GetAll()
                .Where(log => (!startDate.HasValue || log.AttendanceDateTime.ConvertToUserTime().Date >= startDate.Value.Date)
                           && (!endDate.HasValue || log.AttendanceDateTime.ConvertToUserTime().Date <= endDate.Value.Date)
                           && (employeeId <= 0 || log.EmployeeId == employeeId))
                .Join(_employeeRepository.GetAll(),
                      log => log.EmployeeId,
                      emp => emp.Id,
                      (log, emp) => new { log, emp.NameEng, emp.Id, emp.FileNumber })
                .ToList();

            var groupedData = query
                .GroupBy(entry => new { entry.Id, entry.NameEng, entry.FileNumber, entry.log.AttendanceDateTime.ConvertToUserTime().Date })
                .Select(group =>
                {
                    var logs = group.OrderBy(x => x.log.AttendanceDateTime.ConvertToUserTime()).ToList();
                    int totalMinutes = 0;
                    int totalTransactions = logs.Count();
                    var inQueue = new Queue<DateTime>();

                    foreach (var entry in logs)
                    {
                        if (entry.log.ActionTypeId == 1)
                            inQueue.Enqueue(entry.log.AttendanceDateTime.ConvertToUserTime());
                        else if (entry.log.ActionTypeId == 2 && inQueue.Count > 0)
                            totalMinutes += (int)(entry.log.AttendanceDateTime.ConvertToUserTime() - inQueue.Dequeue()).TotalMinutes;
                    }

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
                        Notes = inQueue.Count > 0 ? "Missing OUT" : ""
                    };
                })
                .ToList();

            return groupedData;
        }



        [HttpGet]
        public IActionResult DownloadDailyWorkingHoursPdf(DateTime? startDate, DateTime? endDate, int employeeId)
        {
            var data = GetDailyWorkingHoursData(startDate, endDate, employeeId);
            var employeeName = data.FirstOrDefault()?.EmployeeName ?? "Unknown";

            var report = new DailyWorkingHoursReport(
                data,
                title: $"Daily working hours report - {employeeName}",
                printedBy: User.Identity?.Name ?? "System",
                employeeId: employeeId,
                employeeName: employeeName,
                dateRangeText: startDate.HasValue && endDate.HasValue
                    ? $"From {startDate.Value:dd/MM/yyyy} To {endDate.Value:dd/MM/yyyy}"
                    : "",
                isRtl: false
            );

            using var stream = new MemoryStream();
            report.GeneratePdf(stream);
            Response.Headers.Add("Content-Disposition", "inline; filename=DailyWorkingHoursPdf.pdf");
            return File(stream.ToArray(), "application/pdf");
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
                        .Where(log => (!startDate.HasValue || log.AttendanceDateTime.ConvertToUserTime().Date >= startDate.Value.Date)
                                      && (!endDate.HasValue || log.AttendanceDateTime.ConvertToUserTime().Date <= endDate.Value.Date)
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
                .GroupBy(entry => new { entry.log.Id, entry.log.NameEng, entry.log.log.AttendanceDateTime.ConvertToUserTime().Date, entry.LocationNameEnglish })
                .SelectMany(group => group.OrderBy(x => x.log.log.AttendanceDateTime.ConvertToUserTime())
                .Select((entry, index) => new
                {
                    SerialNumber = index + 1,
                    Date = group.Key.Date.ToString("dd/MM/yyyy"),
                    EmployeeId = group.Key.Id,
                    EmployeeName = group.Key.NameEng,
                    Location = group.Key.LocationNameEnglish,
                    Time = entry.log.log.AttendanceDateTime.ConvertToUserTime().ToString("hh:mm tt"),
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

        [HttpGet]
        public IActionResult DownloadEmployeeLocationAuthorityPdf()
        {
            var employees = _employeeService.GetAllEmployees();
            var allLocations = _locationService.GetAllLocations();
            var links = _employeeLocationService.GetAllEmployeeLocations();

            var data = employees.SelectMany(emp =>
            {
                var empLinks = links.Where(el => el.EmployeeId == emp.Id).ToList();

                return empLinks.Select(link =>
                {
                    var location = allLocations.FirstOrDefault(l => l.Id == link.LocationId);
                    return new EmployeeLocationDto
                    {
                        EmployeeName = emp.NameEng,
                        FileNumber = emp.FileNumber,
                        LocationName = location?.LocationNameEnglish ?? "-",
                        ProofType = link.IsFreeLocation ? "Free Location" : "GPS"
                    };
                });
            }).ToList();

            var report = new EmployeeLocationAuthorityReport(data, "Employee Location Authority Report", User.Identity.Name);
            using var stream = new MemoryStream();
            report.GeneratePdf(stream);

            Response.Headers.Add("Content-Disposition", "inline; filename=EmployeeLocationReport.pdf");
            return File(stream.ToArray(), "application/pdf");
        }






        #endregion

        #endregion
    }
}