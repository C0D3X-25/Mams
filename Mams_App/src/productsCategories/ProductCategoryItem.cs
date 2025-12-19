using Mams.src.items;

namespace Mams.src.productsCategories;

public class ProductCategoryItem : ABaseItem {
    public int product_category_id { get; set; } = 0;
    public string product_category_name { get; set; } = string.Empty;
    public string product_category_archive { get; set; } = string.Empty;
}
