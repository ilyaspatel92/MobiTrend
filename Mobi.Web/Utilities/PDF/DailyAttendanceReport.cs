// File: Utilities/PDF/DailyAttendanceReport.cs
using Mobi.Web.Models.EmployeeAttendance;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace Mobi.Web.Utilities.PDF
{
    public class DailyAttendanceReport : BaseReportDocument
    {
        readonly List<EmployeeAttendanceLogModel> _logs;

        public DailyAttendanceReport(List<EmployeeAttendanceLogModel> logs,
                                     string title,
                                     string printedBy,
                                     string _dateRangeText,
                                     bool isRtl = false)
          : base(title, printedBy, isRtl: isRtl,dateRangeText: _dateRangeText)
        {
            _logs = logs;
        }

        protected override void ComposeContent(IContainer c)
        {
            c.Table(table =>
            {
                // Define column widths
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(30);    // #
                    cols.ConstantColumn(60);    // File No
                    cols.RelativeColumn();      // Name
                    cols.ConstantColumn(70);    // Date
                    cols.ConstantColumn(60);    // Time
                    cols.ConstantColumn(50);    // Type
                    cols.RelativeColumn();      // Location
                    cols.ConstantColumn(50);    // Proof
                });

                // Table Header
                table.Header(header =>
                {
                    foreach (var h in new[] { "#", "File No", "Name", "Date", "Time", "Type", "Location", "Proof" })
                    {
                        header.Cell()
                              .Border(1)
                              .Padding(4)
                              .Background("#f1f1f1")
                              .AlignCenter()
                              .Text(h).SemiBold().FontSize(9);
                    }
                });

                // Table Rows — Apply .ShowEntire() to each cell!
                int idx = 1;
                foreach (var item in _logs)
                {
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(idx++.ToString());
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(item.FileNumber ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignLeft().Text(item.EmployeeName ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(item.Date ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(item.Time ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(item.ActionTypeName ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignLeft().Text(item.Location ?? "").WrapAnywhere();
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(item.ProofType ?? "");
                }
            });
        }


    }
}
