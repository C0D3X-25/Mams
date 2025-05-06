using Mams.src.items;

namespace Mams.src.receipts; 
public class ReceiptItem : ABaseItem {
    public int receipt_id { get; set; } = 0;
    public double receipt_total_price { get; set; } = 0.0;
    public string receipt_date_created { get; set; } = string.Empty;
    public int fk_client_id { get; set; } = 0;
    public int fk_supplier_id { get; set; } = 0;
}