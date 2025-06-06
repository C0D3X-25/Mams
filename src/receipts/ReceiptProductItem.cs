using Mams.src.items;
using Mams.src.products;
using Mams.src.productsLots;

namespace Mams.src.receipts;

// It's 1 line in a fee receipt
public class ReceiptProductItem : ABaseItem {
    public int receipt_product_id { get; set; } = 0;
    public int receipt_product_quantity { get; set; } = 0;
    public decimal receipt_product_unity_price { get; set; } = 0.0M;
    public int fk_receipt_id { get; set; } = 0;
    public ProductItem product_item { get; set; } = new();
    public ProductLotItem product_lot_item { get; set;} = new();
}
