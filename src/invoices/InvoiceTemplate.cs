using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Diagnostics;
using System.IO;

namespace Mams.src.invoices;

/// <summary>
/// What the PDF will look like
/// </summary>
public class InvoiceTemplate : IDocument {

    public InvoiceItem m_item { get; }
    private const string _m_pdf_file_name = "invoice.pdf";

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
    public DocumentSettings GetSettings() => DocumentSettings.Default;


    public InvoiceTemplate(InvoiceItem model) {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        m_item = model;
    }


    public void Compose(IDocumentContainer container) {
        container
            .Page(page => {
                page.Margin(50);

                page.Header().Height(100).Background(Colors.Grey.Lighten1);
                page.Content().Background(Colors.Grey.Lighten3);
                page.Footer().Height(50).Background(Colors.Grey.Lighten1);
            });
    }

    public void generateInvoice() {
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(20));

                page.Header()
                    .Text("Hello PDF!")
                    .SemiBold().FontSize(36).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(x => {
                        x.Spacing(20);

                        x.Item().Text(Placeholders.LoremIpsum());
                        x.Item().Image(Placeholders.Image(200, 100));
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x => {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                    });
            });
        })
        .GeneratePdf(_m_pdf_file_name);

        var p = new Process();
        p.StartInfo = new ProcessStartInfo(Path.Combine(Directory.GetCurrentDirectory(), _m_pdf_file_name)) {
            UseShellExecute = true
        };
        p.Start();
    }
}