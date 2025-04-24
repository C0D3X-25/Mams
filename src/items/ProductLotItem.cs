using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.items;

public class ProductLotItem : ABaseItem {
    public int product_lot_id { get; set; } = 0;
    public string product_lot_name { get; set; } = String.Empty;
    public int product_lot_year { get; set; } = 0;
    public string product_lot_archive { get; set; } = String.Empty;
    public int fk_beehive_id { get; set; } = 0;
    public string beehive_name { get; set; } = String.Empty;
}
