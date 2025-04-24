using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.items;

public class ProductTypeItem : ABaseItem {
    public int product_type_id { get; set; } = 0;
    public string product_type_name { get; set; } = String.Empty;
    public string product_type_archive { get; set; } = String.Empty;
}
