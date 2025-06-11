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
                                     bool isRtl = false)
          : base(title, printedBy, isRtl: isRtl)
        {
            _logs = logs;
        }

        protected override void ComposeContent(IContainer c)
        {
            c.Table(table =>
            {
                // Column widths
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(20);
                    cols.ConstantColumn(60);
                    cols.RelativeColumn();
                    cols.ConstantColumn(70);
                    cols.ConstantColumn(60);
                    cols.ConstantColumn(50);
                    cols.RelativeColumn();
                    cols.ConstantColumn(50);
                });

                // Header row
                table.Header(header =>
                {
                    foreach (var h in new[] { "#", "File No", "Name", "Date", "Time", "Type", "Location", "Proof" })
                    {
                        header.Cell()
                              .Background("#f1f1f1")
                              .Padding(4)
                              .BorderBottom(1)
                              .AlignCenter()
                              .Text(h).SemiBold().FontSize(9);
                    }
                });

                // Data rows
                int idx = 1;
                foreach (var item in _logs)
                {
                    table.Cell().Padding(3).AlignCenter().Text(idx++.ToString());
                    table.Cell().Padding(3).AlignCenter().Text(item.FileNumber ?? "");
                    table.Cell().Padding(3).AlignLeft().Text(item.EmployeeName ?? "");
                    table.Cell().Padding(3).AlignCenter().Text(item.Date ?? "");
                    table.Cell().Padding(3).AlignCenter().Text(item.Time ?? "");
                    table.Cell().Padding(3).AlignCenter().Text(item.ActionTypeName ?? "");
                    table.Cell().Padding(3).AlignLeft().Text(item.Location ?? "").WrapAnywhere();
                    table.Cell().Padding(3).AlignCenter().Text(item.ProofType ?? "");
                }
            });
        }
    }
}
