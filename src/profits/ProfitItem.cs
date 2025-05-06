using Mams.src.items;

namespace Mams.src.profits;

public class ProfitItem : ABaseItem{
    public int profit_id { get; set; } = 0;
    public string client_name { get; set; } = string.Empty;
    public string product_name { get; set; } = string.Empty;
    public int profit_year { get; set; } = 0;
    public double profit_price_unity { get; set; } = 0.0;
    public double profit_price_total { get; set; } = 0.0;
}
