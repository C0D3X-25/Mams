using Mams.src.helpers;
using Mams.src.invoices;
using Mams.src.receipts;
using QuestPDF.Fluent;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Mams.src.services; 

/// <summary>
/// Service class for generating and managing PDF invoices.
/// Handles the creation, saving, and opening of invoice PDFs based on receipt data.
/// </summary>
public class ServicePDF {

    private string _m_invoice_filename = "Facture.pdf";

    /// <summary>
    /// The full file path where the invoice PDF will be saved.
    /// Combines the user's Pictures folder with the current invoice filename.
    /// </summary>
    private string _m_invoice_save_path => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
        _m_invoice_filename
    );

    /// <summary>
    /// Generates an invoice PDF for the specified receipt ID and opens it using the default PDF viewer.
    /// </summary>
    /// <param name="receipt_id">The ID of the receipt for which to generate an invoice.</param>
    public void generateAndOpenInvoice(string receipt_id) {

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

        _m_invoice_filename = $"Facture {item.receipt_item.receipt_number}.pdf";
        InvoiceTemplate document = new(item);

        document.GeneratePdf(_m_invoice_save_path);
        openInvoice();
    }

    /// <summary>
    /// Opens the generated invoice PDF file using the default PDF viewer.
    /// </summary>
    private void openInvoice() {
        var p = new Process();
        p.StartInfo = new ProcessStartInfo(_m_invoice_save_path) {
            UseShellExecute = true
        };
        p.Start();
    }
}
