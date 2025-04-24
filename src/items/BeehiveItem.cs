using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.items;

public class BeehiveItem : ABaseItem {
    public int beehive_id { get; set; } = 0;
    public string beehive_name { get; set; } = String.Empty;
    public string beehive_archive { get; set; } = String.Empty;
}
