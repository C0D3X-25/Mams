using Mams.src.items;

namespace Mams.src.productsTypes;

public class ProductTypeItem : ABaseItem {
    public int product_type_id { get; set; } = 0;
    public string product_type_name { get; set; } = string.Empty;
    public string product_type_archive { get; set; } = string.Empty;
}
