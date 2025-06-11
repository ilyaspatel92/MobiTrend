using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;

namespace Mobi.Web.Utilities.PDF
{
    public abstract class BaseReportDocument : IDocument
    {
        readonly string _title, _printedBy, _logoPath;
        readonly DateTime _printTime;
        readonly bool _isRtl;

        protected BaseReportDocument(string title,
                                     string printedBy = "SYSTEM",
                                     string logoPath = null,
                                     bool isRtl = false)
        {
            _title = title;
            _printedBy = printedBy;
            _printTime = DateTime.Now;
            _isRtl = isRtl;
            _logoPath = logoPath
                         ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pdflogo.png");
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);

                // Global text style + direction
                page.DefaultTextStyle(x =>
                    _isRtl
                      ? TextStyle.Default.DirectionFromRightToLeft().FontSize(9).FontFamily("Arial")
                      : TextStyle.Default.DirectionFromLeftToRight().FontSize(9).FontFamily("Arial")
                );

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContentContainer);
                page.Footer().Element(ComposeFooter);
            });
        }

        // 1) Bordered header with logo + dynamic title
        void ComposeHeader(IContainer c)
        {
            c.Border(1)
             .Padding(8)
             .Row(r =>
             {
                 r.ConstantColumn(60)
                  .Height(40)
                  .AlignMiddle()
                  .Image(_logoPath, ImageScaling.FitArea);

                 r.Spacing(10);

                 r.RelativeColumn()
                  .AlignMiddle()
                  .Text(_title)
                  .FontSize(14)
                  .SemiBold();
             });
        }

        // Wraps child content in its own bordered box
        void ComposeContentContainer(IContainer c)
        {
            c.PaddingVertical(10)
             .Border(1)
             .Padding(8)
             .Element(ComposeContent);
        }

        // To be implemented by each report
        protected abstract void ComposeContent(IContainer c);

        // 2) Bordered footer with ©, printed info, page numbers
        void ComposeFooter(IContainer c)
        {
            c.Border(1)
             .Padding(5)
             .Row(r =>
             {
                 // Left: © year & company
                 r.RelativeColumn().Text($"© {_printTime.Year} Mobitend system").FontSize(8);

                 // Center: printed info
                 r.RelativeColumn()
                  .AlignCenter()
                  .Text($"Printed by: {_printedBy} at {_printTime:dd/MM/yyyy @ hh:mm tt}")
                  .FontSize(8);

                 // Right: page X of Y
                 r.RelativeColumn()
                  .AlignRight()
                  .Text(txt =>
                  {
                      txt.Span("Page ").FontSize(8);
                      txt.CurrentPageNumber().FontSize(8);
                      txt.Span(" of ").FontSize(8);
                      txt.TotalPages().FontSize(8);
                  });
             });
        }
    }
}
