using Mams.src.items;

namespace Mams.src.receipts;

public class ReceiptSupplierItem : ABaseItem {
    public int fk_supplier_id { get; set; } = 0;
    public int fk_receipt_id { get; set; } = 0;
}
