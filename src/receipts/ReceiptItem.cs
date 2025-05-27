using Mams.src.items;

namespace Mams.src.receipts;

public class ReceiptItem : ABaseItem {
    public int receipt_id { get; set; } = 0;
    public decimal receipt_total_price { get; set; } = 0.0M;
    public string receipt_date_created { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString();
}