using Mams_App.src.items;

namespace Mams_App.src.beehives;

public class BeehiveItem : ABaseItem
{
    public int beehive_id { get; set; } = 0;
    public string beehive_name { get; set; } = string.Empty;
    public string beehive_number { get; set; } = string.Empty;
    public string beehive_archive { get; set; } = string.Empty;
    public int fk_region_id { get; set; } = 0;

    // Joined field (not stored in beehives table)
    public string region_name { get; set; } = string.Empty;
}
