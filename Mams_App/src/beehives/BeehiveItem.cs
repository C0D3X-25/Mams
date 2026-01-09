using Mams_App.src.items;

namespace Mams_App.src.beehives;

public class BeehiveItem : ABaseItem
{
    public int beehive_id { get; set; } = 0;
    public string beehive_name { get; set; } = string.Empty;
    public string beehive_archive { get; set; } = string.Empty;
}
