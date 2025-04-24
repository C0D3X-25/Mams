using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.items;

public class ProductCategoryItem : ABaseItem {
    public int product_category_id { get; set; } = 0;
    public string product_category_name { get; set; } = String.Empty;
    public string product_category_archive { get; set; } = String.Empty;
}
