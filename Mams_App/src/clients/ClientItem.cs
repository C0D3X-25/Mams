using Mams_App.src.items;

namespace Mams_App.src.clients;

public class ClientItem : ABaseItem
{

    public int client_id { get; set; } = 0;
    public int fk_entity_id { get; set; } = 0;
    public string client_archive { get; set; } = string.Empty;
}
