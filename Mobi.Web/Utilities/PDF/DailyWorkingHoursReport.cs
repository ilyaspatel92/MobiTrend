using Mobi.Web.Models.Reports;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Mobi.Web.Utilities.PDF
{
    public class DailyWorkingHoursReport : BaseReportDocument
    {
        private readonly List<DailyWorkingHoursDto> _data;
        private readonly int _employeeId;
        private readonly string _employeeName;

        public DailyWorkingHoursReport(List<DailyWorkingHoursDto> data,
                                       string title,
                                       string printedBy,
                                       int employeeId,
                                       string employeeName,
                                       string dateRangeText = "",
                                       bool isRtl = false)
            : base(title, printedBy, isRtl: isRtl, dateRangeText: dateRangeText)
        {
            _data = data;
            _employeeId = employeeId;
            _employeeName = employeeName;
        }

        protected override void ComposeContent(IContainer container)
        {
            container.Column(rootCol =>
            {
                var groupedByEmployee = _data.GroupBy(x => new { x.EmployeeId, x.EmployeeName, x.FileNumber }).ToList();

                foreach (var empGroup in groupedByEmployee)
                {
                    rootCol.Item().Column(col =>
                    {
                        // 🔹 Employee Header
                        col.Item().Border(1).Background("#f1f1f1").Row(row =>
                        {
                            row.RelativeColumn().Padding(6).Text($"Employee ID: {empGroup.Key.FileNumber}").Bold().FontSize(10).AlignLeft();
                            row.RelativeColumn().Padding(6).Text($"Employee Name: {empGroup.Key.EmployeeName}").Bold().FontSize(10).AlignLeft();
                        });




                        // 🔹 Table for that employee
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(25);   // #
                                cols.ConstantColumn(70);   // Date
                                cols.ConstantColumn(60);   // Day
                                cols.ConstantColumn(60);   // Hours
                                cols.ConstantColumn(60);   // Minutes
                                cols.ConstantColumn(70);   // Transactions
                                cols.RelativeColumn();     // Notes
                            });

                            // Table Header
                            table.Header(header =>
                            {
                                string[] headers = { "#", "Date", "Day", "Total Hours", "Total Minutes", "Total Transactions", "Notes" };
                                foreach (var h in headers)
                                {
                                    header.Cell().Border(1).Padding(4).AlignCenter().Text(h).Bold().FontSize(9);
                                }
                            });

                            // Table Rows
                            int rowIndex = 1;
                            foreach (var log in empGroup)
                            {
                                table.Cell().Border(1).Padding(3).AlignCenter().Text(rowIndex++.ToString());
                                table.Cell().Border(1).Padding(3).AlignCenter().Text(log.Date);
                                table.Cell().Border(1).Padding(3).AlignCenter().Text(log.Day);
                                table.Cell().Border(1).Padding(3).AlignCenter().Text(log.TotalHours.ToString());
                                table.Cell().Border(1).Padding(3).AlignCenter().Text(log.TotalMinutes.ToString());
                                table.Cell().Border(1).Padding(3).AlignCenter().Text(log.TotalTransactions.ToString());
                                table.Cell().Border(1).Padding(3).AlignCenter().Text(log.Notes ?? "");
                            }
                        });

                        // 🔹 Page break if not last
                        if (empGroup.Key != groupedByEmployee.Last().Key)
                        {
                            col.Item().PageBreak();
                        }
                    });
                }
            });
        }

    }
}
