using Mams_App.src.items;

namespace Mams_App.src.regions;

public class RegionItem : ABaseItem
{
    public int region_id { get; set; } = 0;
    public string region_name { get; set; } = string.Empty;
    public string region_archive { get; set; } = string.Empty;
}
