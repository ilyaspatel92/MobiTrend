using Mobi.Web.Models.Reports;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace Mobi.Web.Utilities.PDF
{
    public class MonthlyWorkingHoursReport : BaseReportDocument
    {
        private readonly List<MonthlyWorkingHoursDto> _data;

        public MonthlyWorkingHoursReport(
            List<MonthlyWorkingHoursDto> data,
            string title,
            string printedBy,
            string dateRangeText = "",
            bool isRtl = false)
            : base(title, printedBy, isRtl: isRtl, dateRangeText: dateRangeText)
        {
            _data = data;
        }

        protected override void ComposeContent(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(25); // #
                    cols.ConstantColumn(50); // File Number
                    cols.RelativeColumn();   // Employee Name
                    cols.ConstantColumn(70); // Total Hours
                    cols.ConstantColumn(80); // Total Minutes
                    cols.RelativeColumn();   // Notes
                });

                // Header
                table.Header(header =>
                {
                    header.Cell().Background("#cccccc").Padding(5).Text("#").Bold().FontSize(10);
                    header.Cell().Background("#cccccc").Padding(5).Text("ID").Bold().FontSize(10);
                    header.Cell().Background("#cccccc").Padding(5).Text("Employee Name").Bold().FontSize(10);
                    header.Cell().Background("#cccccc").Padding(5).Text("Total Hours").Bold().FontSize(10);
                    header.Cell().Background("#cccccc").Padding(5).Text("Total Minutes").Bold().FontSize(10);
                    header.Cell().Background("#cccccc").Padding(5).Text("Notes").Bold().FontSize(10);
                });

                // Data rows
                int rowIndex = 1;
                foreach (MonthlyWorkingHoursDto row in _data)
                {
                    table.Cell().Border(1).Padding(3).AlignCenter().Text(rowIndex++.ToString());
                    table.Cell().Border(1).Padding(3).AlignCenter().Text(row.EmployeeId ?? "");
                    table.Cell().Border(1).Padding(3).AlignLeft().Text(row.EmployeeName ?? "");
                    table.Cell().Border(1).Padding(3).AlignCenter().Text(row.TotalHours.ToString());
                    table.Cell().Border(1).Padding(3).AlignCenter().Text(row.TotalMinutes.ToString());
                    table.Cell().Border(1).Padding(3).AlignCenter().Text(row.Notes ?? "");
                }
            });
        }
    }
}
