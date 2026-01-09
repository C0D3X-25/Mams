using Mams_App.src.items;

namespace Mams_App.src.products;


public class ProductItem : ABaseItem
{
    public int product_id { get; set; } = 0;
    public string product_name { get; set; } = string.Empty;
    public int product_weight { get; set; } = 0;
    public string product_archive { get; set; } = string.Empty;

    public int fk_product_type_id { get; set; } = 0;
    public string product_type_name { get; set; } = string.Empty;

    public int fk_product_category_id { get; set; } = 0;
    public string product_category_name { get; set; } = string.Empty;

    public int fk_product_shape_id { get; set; } = 0;
    public string product_shape_name { get; set; } = string.Empty;
}

