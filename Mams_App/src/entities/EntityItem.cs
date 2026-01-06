using Mams_App.src.items;

namespace Mams_App.src.entities;

public class EntityItem : ABaseItem{
    public int entity_id { get; set; } = 0;
    public string entity_name { get; set; } = string.Empty;
    public string entity_phone { get; set; } = string.Empty;
    public string entity_email { get; set; } = string.Empty;
    public string entity_city { get; set; } = string.Empty;
    public string entity_address { get; set; } = string.Empty;
    public string entity_archive { get; set; } = string.Empty;
}
