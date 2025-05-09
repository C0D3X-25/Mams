using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.receipts; 
public class ReceiptCascadeOperationItem : ABaseItem {
    public ReceiptItem receipt_item { get; set; } = new();
    public ReceiptClientItem receipt_client_item { get; set; } = new();
    public ReceiptSupplierItem receipt_supplier_item { get; set; } = new();
    public ObservableCollection<ReceiptProductItem> receipt_product_items { get; set; } = new();
}
