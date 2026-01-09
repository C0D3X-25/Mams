using Mams_App.src.clients;
using Mams_App.src.entities;
using Mams_App.src.items;
using Mams_App.src.receipts;
using System.Collections.ObjectModel;

namespace Mams_App.src.profits;

// Class holding all details of a receipt profit item for frontend interaction
public class ReceiptProfitDetailedItem : ABaseItem
{
    public EntityItem entity { get; set; } = new();
    public ClientItem client { get; set; } = new();
    public ReceiptClientItem receipt_client { get; set; } = new();
    public ReceiptItem receipt { get; set; } = new();
    public ObservableCollection<ReceiptProductItem> receipt_products { get; set; } = new();
}
