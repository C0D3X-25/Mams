using Mams.src.items;

namespace Mams.src.clients;

public class ClientItem : ABaseItem {

    public int client_id { get; set; } = 0;
    public int fk_entity_id { get; set; } = 0;
}
