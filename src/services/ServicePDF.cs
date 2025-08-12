
using Mams.src.invoices;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Diagnostics;
using System.IO;

namespace Mams.src.services; 

public class ServicePDF {

    private const string _m_pdf_file_name = "invoice.pdf";

    public void generateInvoice() {

        var invoice_data = InvoicePdfModel.getInvoiceData();
        InvoiceTemplate document = new(invoice_data);

        document.GeneratePdf(_m_pdf_file_name);

        openInvoice();
    }

    public void openInvoice() {
        var p = new Process();
        p.StartInfo = new ProcessStartInfo(Path.Combine(Directory.GetCurrentDirectory(), _m_pdf_file_name)) {
            UseShellExecute = true
        };
        p.Start();
    }
}
