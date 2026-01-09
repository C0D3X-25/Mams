using Mams_App.src.items;

namespace Mams_App.src.productsShapes;

public class ProductShapeItem : ABaseItem
{
    public int product_shape_id { get; set; } = 0;
    public string product_shape_name { get; set; } = string.Empty;
    public string product_shape_archive { get; set; } = string.Empty;
}
