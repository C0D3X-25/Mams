using Mams.src.items;

namespace Mams.src.fees;

public class FeeItem : ABaseItem {
    public int fee_id { get; set; } = 0;
    public string supplier_name { get; set; } = string.Empty;
    public string product_name { get; set; } = string.Empty;
    public int fee_year { get; set; } = 0;
    public double fee_price_unity { get; set; } = 0.0;
    public double fee_price_total { get; set; } = 0.0;
}
