using Mams.src.items;

namespace Mams.src.receipts;

public class ReceiptClientItem : ABaseItem {
    public int fk_client_id { get; set; } = 0;
    public int fk_receipt_id { get; set; } = 0;
}
