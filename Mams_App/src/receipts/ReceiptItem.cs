using Mams_App.src.items;

namespace Mams_App.src.receipts;

public class ReceiptItem : ABaseItem
{
    public int receipt_id { get; set; } = 0;
    public string receipt_number { get; set; } = string.Empty;
    public decimal receipt_total_price { get; set; } = 0.0M;

    /// <summary>
    /// The date value used for sorting. Set this first, then receipt_date_created will be auto-formatted.
    /// </summary>
    public DateOnly receipt_date_created_value { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    /// <summary>
    /// The formatted date string for display (dd.MM.yyyy format).
    /// </summary>
    public string receipt_date_created
    {
        get => receipt_date_created_value.ToString(globals.SGlobals.g_EU_DATE_FORMAT);
        set
        {
            if (DateOnly.TryParseExact(value, globals.SGlobals.g_EU_DATE_FORMAT, out var parsed))
            {
                receipt_date_created_value = parsed;
            }
        }
    }
}