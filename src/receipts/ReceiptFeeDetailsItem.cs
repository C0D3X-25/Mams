using Mams.src.entities;
using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.receipts;

// Class holding all details of a receipt fee item for frontend display
public class ReceiptFeeDetailsItem : ABaseItem {
    public EntityItem entity { get; set; } = new EntityItem();
    public ReceiptItem receipt { get; set; } = new ReceiptItem();
    public ReceiptSupplierItem receipt_supplier { get; set; } = new ReceiptSupplierItem();

    public ObservableCollection<ReceiptProductItem> receipt_product = new ObservableCollection<ReceiptProductItem>();
}
