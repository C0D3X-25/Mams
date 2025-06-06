using Mams.src.items;
using Mams.src.products;

namespace Mams.src.fees;

// TODO: Can use the same class as ReceiptProductItem? 
// It's 1 line in a fee receipt
public class FeeProductItem : ABaseItem {
    public int fk_receipt_id { get; set; } = 0;
    public int fee_quantity { get; set; } = 1;
    public decimal fee_price_unity { get; set; } = 0.0M;
    public decimal fee_price_total { get; set; } = 0.0M;
    public ProductItem product_item { get; set; } = new();
}
