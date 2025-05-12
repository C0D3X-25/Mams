using Mams.src.items;

namespace Mams.src.fees;

public class FeeItem : ABaseItem {
    public int receipt_id { get; set; } = 0;
    public string supplier_name { get; set; } = string.Empty;
    public string product_name { get; set; } = string.Empty;
    public string fee_date { get; set; } = string.Empty;
    public int fee_quantity { get; set; } = 0;
    public decimal fee_price_unity { get; set; } = 0.0M;
    public decimal fee_price_total { get; set; } = 0.0M;
}
