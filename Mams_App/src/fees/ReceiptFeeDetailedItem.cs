using Mams_App.src.entities;
using Mams_App.src.items;
using Mams_App.src.receipts;
using Mams_App.src.suppliers;
using System.Collections.ObjectModel;

namespace Mams_App.src.fees;

// Class holding all details of a receipt fee item for frontend interaction
public class ReceiptFeeDetailedItem : ABaseItem
{
    public EntityItem entity { get; set; } = new();
    public SupplierItem supplier { get; set; } = new();
    public ReceiptSupplierItem receipt_supplier { get; set; } = new();
    public ReceiptItem receipt { get; set; } = new();
    public ObservableCollection<ReceiptProductItem> receipt_products { get; set; } = new();
}
