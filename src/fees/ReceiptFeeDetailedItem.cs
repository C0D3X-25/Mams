using Mams.src.entities;
using Mams.src.items;
using Mams.src.products;
using Mams.src.receipts;
using Mams.src.suppliers;
using System.Collections.ObjectModel;

namespace Mams.src.fees;

// Class holding all details of a receipt fee item for frontend interaction
public class ReceiptFeeDetailedItem : ABaseItem {
    public EntityItem entity { get; set; } = new();
    public SupplierItem supplier { get; set; } = new();
    public ReceiptSupplierItem receipt_supplier { get; set; } = new();
    public ReceiptItem receipt { get; set; } = new();
    public ObservableCollection<ReceiptProductItem> receipt_products = new();
    public ObservableCollection<ProductItem> product = new();
}
