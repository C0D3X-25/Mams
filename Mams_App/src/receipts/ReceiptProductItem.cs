using Mams_App.src.items;
using Mams_App.src.products;
using Mams_App.src.productsLots;
using Mams_App.src.views.globalView;

namespace Mams_App.src.receipts;

// It's 1 line in a fee receipt
public class ReceiptProductItem : ABaseItem
{
    public int receipt_product_id { get; set; } = 0;
    public int receipt_product_quantity { get; set; } = 0;
    public decimal receipt_product_unity_price { get; set; } = 0.0M;
    public int fk_receipt_id { get; set; } = 0;
    public ProductItem product_item { get; set; } = new();
    public ProductLotItem product_lot_item { get; set; } = new();

    // Add UI color properties
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
}
