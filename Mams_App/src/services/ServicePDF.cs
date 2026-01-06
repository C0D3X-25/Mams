using Mams_App.src.helpers;
using Mams_App.src.invoices;
using Mams_App.src.receipts;
using QuestPDF.Fluent;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Mams_App.src.services; 

/// <summary>
/// Service class for generating and managing PDF invoices.
/// Handles the creation, saving, and opening of invoice PDFs based on receipt data.
/// </summary>
public class ServicePDF {

    private string _m_invoice_directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    private string _m_invoice_folder_name = "Factures Miel";
    private string _m_invoice_filename = "Facture.pdf";

    /// <summary>
    /// The full file path where the invoice PDF will be saved.
    /// Combines the user's Pictures folder, the invoice folder name, and the current invoice filename.
    /// </summary>
    private string _m_invoice_save_path => Path.Combine(
        _m_invoice_directory,
        _m_invoice_folder_name,
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
        var result = receipt_handler_model.getItemByID(receipt_id);

        if (!result.is_found || result.returned_item is null) {
            MessageBox.Show("Receipt not found.");
            return;
        }

        var item = result.returned_item;
        _m_invoice_filename = $"Facture {item.receipt_item.receipt_number}.pdf";
        
        // Ensure the directory exists before generating the PDF
        string directoryPath = Path.Combine(_m_invoice_directory, _m_invoice_folder_name);
        if (!Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
        }
        
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
