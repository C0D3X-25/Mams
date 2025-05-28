using Mams.src.entities;
using Mams.src.items;
using Mams.src.receipts;
using Mams.src.suppliers;
using System.Collections.ObjectModel;

namespace Mams.src.fees;

// Class holding all details of a receipt fee item for frontend interaction
public class ReceiptFeeDetailsItem : ABaseItem {
    public EntityItem entity { get; set; } = new EntityItem();
    public SupplierItem supplier { get; set; } = new SupplierItem();
    public ReceiptSupplierItem receipt_supplier { get; set; } = new ReceiptSupplierItem();
    public ReceiptItem receipt { get; set; } = new ReceiptItem();
    public ObservableCollection<ReceiptProductItem> receipt_products = new ObservableCollection<ReceiptProductItem>();
}
