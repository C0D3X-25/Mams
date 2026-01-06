using Mams_App.src.items;

namespace Mams_App.src.receipts;

public class ReceiptSupplierItem : ABaseItem {
    public int fk_supplier_id { get; set; } = 0;
    public int fk_receipt_id { get; set; } = 0;
}
