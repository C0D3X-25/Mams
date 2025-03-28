using System.ComponentModel;

namespace Mams.src.items;

public class EntityItem : ABaseItem{
    public int entity_id { get; set; } = 0;
    public string entity_name { get; set; } = String.Empty;
    public string entity_phone { get; set; } = String.Empty;
    public string entity_email { get; set; } = String.Empty;
    public string entity_city { get; set; } = String.Empty;
    public string entity_address { get; set; } = String.Empty;
    public string entity_archive { get; set; } = String.Empty;
}
