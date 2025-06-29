using Mobi.Web.Models.Employees;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace Mobi.Web.Utilities.PDF
{
    public class RegisteredPhonesReport : BaseReportDocument
    {
        private readonly List<EmployeeModel> _employees;

        public RegisteredPhonesReport(List<EmployeeModel> employees, string title, string printedBy)
            : base(title, printedBy)
        {
            _employees = employees;
        }

        protected override void ComposeContent(IContainer container)
        {
            container.Table(table =>
            {
                // Define column widths
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(30);  // #
                    columns.ConstantColumn(50);  // ID
                    columns.RelativeColumn();    // Name
                    columns.RelativeColumn();    // Email
                    columns.ConstantColumn(70);  // Mob type
                    columns.ConstantColumn(70);  // Reg date
                    columns.ConstantColumn(90);  // Last trans
                });

                // Table header
                table.Header(header =>
                {
                    var headers = new[] { "#", "ID", "Employee Name", "Email", "Mob type", "Reg date", "Last trans date" };
                    foreach (var h in headers)
                    {
                        header.Cell().Border(1).Background("#f1f1f1")
                            .Padding(4).AlignCenter().Text(h).SemiBold().FontSize(9);
                    }
                });

                // Table rows
                int index = 1;
                foreach (var emp in _employees)
                {
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(index++.ToString());
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(emp.FileNumber ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignLeft().Text(emp.NameEng ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignLeft().Text(emp.Email ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(emp.MobileTypeName ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(emp.MobRegistrationDate ?? "");
                    table.Cell().Element(e => e.ShowEntire()).Border(1).Padding(3).AlignCenter().Text(emp.LastTransactionDate ?? "");
                }

                // Footer: total records
                table.Cell().ColumnSpan(6).Border(1).Padding(4).AlignRight().Text("Total Records").Bold();
                table.Cell().Border(1).Padding(4).AlignCenter().Text(_employees.Count.ToString()).Bold();
            });
        }
    }
}
