using Mams.src.items;

namespace Mams.src.profits;

public class ProfitItem : ABaseItem{
    public int receipt_id { get; set; } = 0;
    public string client_name { get; set; } = string.Empty;
    public string product_name { get; set; } = string.Empty;
    public string profit_date { get; set; } = DateOnly.FromDateTime(DateTime.Now).ToString();
    public int profit_quantity { get; set; } = 1;
    public decimal profit_price_unity { get; set; } = 0.0M;
    public decimal profit_price_total { get; set; } = 0.0M;
}
