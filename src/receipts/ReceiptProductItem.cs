using Mams.src.items;
using Mams.src.products;

namespace Mams.src.receipts;

public class ReceiptProductItem : ABaseItem {
    public int receipt_product_id { get; set; } = 0;
    public int receipt_product_quantity { get; set; } = 0;
    public decimal receipt_product_unity_price { get; set; } = 0.0M;
    public int fk_product_id { get; set; } = 0;
    public int fk_receipt_id { get; set; } = 0;
    public ProductItem product_item { get; set; } = new();
}
