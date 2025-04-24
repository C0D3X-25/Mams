using Mams.src.items;

namespace Mams.src.productsLots;

public class ProductLotItem : ABaseItem {
    public int product_lot_id { get; set; } = 0;
    public string product_lot_name { get; set; } = string.Empty;
    public int product_lot_year { get; set; } = 0;
    public int fk_beehive_id { get; set; } = 0;
    public string beehive_name { get; set; } = string.Empty;
    public string product_lot_archive { get; set; } = string.Empty;
}
