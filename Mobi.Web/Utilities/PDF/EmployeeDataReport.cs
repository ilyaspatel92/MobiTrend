using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using Mobi.Web.Models.Employees;
using System.Collections.Generic;

namespace Mobi.Web.Utilities.PDF
{
    public class EmployeeDataReport : BaseReportDocument
    {
        private readonly List<EmployeeModel> _employees;

        public EmployeeDataReport(List<EmployeeModel> employees, string title, string printedBy)
            : base(title, printedBy)
        {
            _employees = employees;
        }

        protected override void ComposeContent(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);  // #
                    columns.ConstantColumn(50);  // ID
                    columns.RelativeColumn();    // Name
                    columns.RelativeColumn();    // Email
                    columns.ConstantColumn(70);  // Reg date
                });

                // Header
                table.Header(header =>
                {
                    foreach (var h in new[] { "#", "ID", "Employee Name", "Email", "Reg date" })
                    {
                        header.Cell().Border(1).Background("#f1f1f1")
                            .Padding(4).AlignCenter().Text(h).SemiBold().FontSize(9);
                    }
                });

                int index = 1;
                foreach (var emp in _employees)
                {
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(index++.ToString());
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(emp.FileNumber.ToString());
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignLeft().Text(emp.NameEng ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignLeft().Text(emp.Email ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(emp.MobRegistrationDate ?? "");
                }

                // Footer row
                table.Cell().ColumnSpan(4).Border(1).Padding(4).AlignRight().Text("Total Records").Bold();
                table.Cell().Border(1).Padding(4).AlignCenter().Text(_employees.Count.ToString()).Bold();
            });
        }
    }
}
