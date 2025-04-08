using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.items;
public class ProductItem : ABaseItem {
    public int product_id { get; set; } = 0;
    public string product_name { get; set; } = String.Empty;
    public int product_weight { get; set; } = 0;
    public string product_archive { get; set; } = String.Empty;

    public int fk_product_type_id { get; set; } = 0;
    public string product_type_name { get; set; } = String.Empty;

    public int fk_product_category_id { get; set; } = 0;
    public string product_category_name { get; set; } = String.Empty;

    public int fk_product_shape_id { get; set; } = 0;
    public string product_shape_name { get; set; } = String.Empty;

    public int fk_product_lot_id { get; set; } = 0;
    public string product_lot_name { get; set; } = String.Empty;
}

