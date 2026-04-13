using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.receipts;
using Mams_App.src.treatments;
using QuestPDF.Fluent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Mams_App.src.invoices;

/// <summary>
/// Service class for generating and managing PDF invoices.
/// Handles the creation, saving, and opening of invoice PDFs based on receipt data.
/// </summary>
public static class SInvoicePdfService
{
    private static readonly string _m_invoice_directory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
    private static string _m_invoice_folder_name => Loc.Get("Pdf.FolderName");

    /// <summary>
    /// Gets the full file path where the invoice PDF will be saved.
    /// Combines the user's Pictures folder, the invoice folder name, and the specified filename.
    /// </summary>
    private static string getInvoiceSavePath(string filename) => Path.Combine(
        _m_invoice_directory,
        _m_invoice_folder_name,
        filename
    );

    /// <summary>
    /// Ensures the invoice directory exists.
    /// </summary>
    private static void ensureDirectoryExists()
    {
        string directoryPath = Path.Combine(_m_invoice_directory, _m_invoice_folder_name);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    /// <summary>
    /// Generates a profit invoice PDF for the specified receipt ID and opens it using the default PDF viewer.
    /// In this invoice, the User (app owner) is the seller and the Entity is the client.
    /// </summary>
    /// <param name="receipt_id">The ID of the receipt for which to generate an invoice.</param>
    public static void generateAndOpenProfitInvoice(string receipt_id)
    {

        if (!SDataValidation.isIdValid(receipt_id))
        {
            MessageBox.Show(Loc.Get("Pdf.InvalidReceiptId"));
            return;
        }

        ReceiptHandlerModel receipt_handler_model = new();
        var result = receipt_handler_model.getItemByID(receipt_id);

        if (!result.is_found || result.returned_item is null)
        {
            MessageBox.Show(Loc.Get("Pdf.ReceiptNotFound"));
            return;
        }

        var item = result.returned_item;
        string invoiceFilename = Loc.Get("Pdf.Filename", item.receipt_item.receipt_number);
        string invoiceSavePath = getInvoiceSavePath(invoiceFilename);

        ensureDirectoryExists();

        InvoiceProfitTemplate document = new(item);
        document.GeneratePdf(invoiceSavePath);
        openInvoice(invoiceSavePath);
    }

    /// <summary>
    /// Generates a fee invoice PDF for the specified receipt ID and opens it using the default PDF viewer.
    /// In this invoice, the Entity (supplier) is the seller and the User (app owner) is the client.
    /// </summary>
    /// <param name="receipt_id">The ID of the receipt for which to generate an invoice.</param>
    public static void generateAndOpenFeeInvoice(string receipt_id)
    {

        if (!SDataValidation.isIdValid(receipt_id))
        {
            MessageBox.Show(Loc.Get("Pdf.InvalidReceiptId"));
            return;
        }

        ReceiptHandlerModel receipt_handler_model = new();
        var result = receipt_handler_model.getItemByID(receipt_id);

        if (!result.is_found || result.returned_item is null)
        {
            MessageBox.Show(Loc.Get("Pdf.ReceiptNotFound"));
            return;
        }

        var item = result.returned_item;
        string invoiceFilename = Loc.Get("Pdf.Filename", item.receipt_item.receipt_number);
        string invoiceSavePath = getInvoiceSavePath(invoiceFilename);

        ensureDirectoryExists();

        InvoiceFeeTemplate document = new(item);
        document.GeneratePdf(invoiceSavePath);
        openInvoice(invoiceSavePath);
    }

    /// <summary>
    /// Opens the generated invoice PDF file using the default PDF viewer.
    /// </summary>
    private static void openInvoice(string invoicePath)
    {
        var p = new Process
        {
            StartInfo = new ProcessStartInfo(invoicePath)
            {
                UseShellExecute = true
            }
        };
        p.Start();
    }

    /// <summary>
    /// Generates a treatment log PDF and opens it using the default PDF viewer.
    /// </summary>
    /// <param name="items">The collection of treatment items to include in the PDF.</param>
    public static void generateAndOpenTreatmentPdf(ObservableCollection<TreatmentItem> items)
    {
        string filename = Loc.Get("Pdf.TreatmentFilename", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        string savePath = getInvoiceSavePath(filename);

        ensureDirectoryExists();

        TreatmentPdfTemplate document = new(items);
        document.GeneratePdf(savePath);
        openInvoice(savePath);
    }
}
