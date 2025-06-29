using Mobi.Web.Models.Reports;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Mobi.Web.Utilities.PDF
{
    public class EmployeeLocationAuthorityReport : BaseReportDocument
    {
        private readonly List<EmployeeLocationDto> _data;

        public EmployeeLocationAuthorityReport(List<EmployeeLocationDto> data, string title, string printedBy)
            : base(title, printedBy)
        {
            _data = data;
        }

        protected override void ComposeContent(IContainer container)
        {
            var groupedByEmployee = _data.GroupBy(x => new { x.EmployeeName, x.FileNumber });

            container.Column(column =>
            {
                foreach (var empGroup in groupedByEmployee)
                {
                    column.Item().Background("#d3d3d3").Padding(5).Row(row =>
                    {
                        row.RelativeColumn().Text($"Employee Name: {empGroup.Key.EmployeeName}").Bold();
                        row.ConstantColumn(100).Text($"ID: {empGroup.Key.FileNumber}").Bold();
                    });

                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(30); // #
                            cols.RelativeColumn();   // Location name
                            cols.RelativeColumn();   // Proof Type
                        });

                        table.Header(header =>
                        {
                            foreach (var colH in new[] { "#", "Location name", "Proof Type" })
                            {
                                header.Cell().Background("#d3d3d3").Border(1).Padding(4).AlignCenter().Text(colH).Bold();
                            }
                        });

                        int index = 1;
                        foreach (var item in empGroup)
                        {
                            table.Cell().Border(1).Padding(4).AlignCenter().Text(index++.ToString());
                            table.Cell().Border(1).Padding(4).Text(item.LocationName);
                            table.Cell().Border(1).Padding(4).Text(item.ProofType);
                        }
                    });

                    // Optional: spacing between groups
                    if (empGroup != groupedByEmployee.Last())
                        column.Item().PaddingBottom(10);
                }
            });
        }

    }
}
