using Mams.src.items;

namespace Mams.src.beehives;

public class BeehiveItem : ABaseItem {
    public int beehive_id { get; set; } = 0;
    public string beehive_name { get; set; } = string.Empty;
    public string beehive_archive { get; set; } = string.Empty;
}
