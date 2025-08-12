using Mams.src.helpers;
using Mams.src.invoices;
using Mams.src.receipts;
using QuestPDF.Fluent;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Mams.src.services; 

public class ServicePDF {

    private const string _m_invoice_filename = "invoice.pdf";

    // TODO: Find a better place to save PDF
    private string _m_invoice_save_path => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        _m_invoice_filename
    );

    public void generateInvoice(string receipt_id) {
        if (!SDataValidation.isIdValid(receipt_id)) {
            MessageBox.Show("Invalid receipt ID.");
            return;
        }

        ReceiptHandlerModel receipt_handler_model = new();
        ReceiptHandlerItem? item = receipt_handler_model.getItemByID(receipt_id);
        if (item is null) {
            MessageBox.Show("Receipt not found.");
            return;
        }
        InvoiceTemplate document = new(item);

        document.GeneratePdf(_m_invoice_save_path);
        openInvoice();
    }

    public void openInvoice() {
        var p = new Process();
        p.StartInfo = new ProcessStartInfo(_m_invoice_save_path) {
            UseShellExecute = true
        };
        p.Start();
    }
}
