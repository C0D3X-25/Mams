using Mams.src.items;
using Mams.src.products;
using Mams.src.productsLots;

namespace Mams.src.receipts;

// It's 1 line in a fee receipt
public class ReceiptProductItem : ABaseItem {
    public int receipt_product_id { get; set; } = 0;
    public int receipt_product_quantity { get; set; } = 0;
    public decimal receipt_product_unity_price { get; set; } = 0.0M;
    public int fk_product_id { get; set; } = 0;
    public int fk_receipt_id { get; set; } = 0;
    public int fk_product_lot_id { get; set; } = 0;
    public ProductItem product_item { get; set; } = new();
    public ProductLotItem product_lot_item { get; set;} = new();

    public ProductItem m_current_product {
        get => product_item;
        set {
            product_item = value;
            if (value != null) {
                fk_product_id = value.product_id;
            }
        }
    }

    public ProductLotItem m_current_product_lot {
        get => product_lot_item;
        set {
            product_lot_item = value;
            if (value != null) {
                fk_product_lot_id = value.product_lot_id;
            }
        }
    }
}
