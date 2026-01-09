using Mams_App.src.items;

namespace Mams_App.src.receipts;

public class ReceiptClientItem : ABaseItem
{
    public int fk_client_id { get; set; } = 0;
    public int fk_receipt_id { get; set; } = 0;
}
