using Mams_App.src.items;

namespace Mams_App.src.receipts;

public class ReceiptItem : ABaseItem {
    public int receipt_id { get; set; } = 0;
    public string receipt_number { get; set; } = string.Empty;
    public decimal receipt_total_price { get; set; } = 0.0M;
    public string receipt_date_created { get; set; } = DateOnly.FromDateTime(DateTime.Today).ToString(globals.SGlobals.g_EU_DATE_FORMAT);
}