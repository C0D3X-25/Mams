using Mams.src.items;
using Mams.src.products;

namespace Mams.src.fees;

public class FeeItem : ABaseItem {
    public int receipt_id { get; set; } = 0;
    public int supplier_id { get; set; } = 0;
    public string supplier_name { get; set; } = string.Empty;
    public int product_id { get; set; } = 0;
    public string product_name { get; set; } = string.Empty;
    public string fee_date { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString();
    public int fee_quantity { get; set; } = 1;
    public decimal fee_price_unity { get; set; } = 0.0M;
    public decimal fee_price_total { get; set; } = 0.0M;

    private ProductItem? _m_current_product;
    public ProductItem? m_current_product {
        get => _m_current_product;
        set {
            _m_current_product = value;
            if (value != null) {
                product_id = value.product_id;
                product_name = value.product_name;
            }
        }
    }
}
