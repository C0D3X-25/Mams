using Mams_App.src.items;
using System.Collections.ObjectModel;

namespace Mams_App.src.receipts; 
public class ReceiptHandlerItem : ABaseItem {
    public ReceiptItem receipt_item { get; set; } = new();
    public ReceiptClientItem receipt_client_item { get; set; } = new();
    public ReceiptSupplierItem receipt_supplier_item { get; set; } = new();
    public ObservableCollection<ReceiptProductItem> receipt_product_items { get; set; } = new();
}
