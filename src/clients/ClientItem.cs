using Mams.src.items;

namespace Mams.src.clients;

public class ClientItem : ABaseItem {

    public int client_id { get; set; }
    public string client_name { get; set; } = string.Empty;
    public string client_phone { get; set; } = string.Empty;
    public string client_email { get; set; } = string.Empty;
    public string client_city { get; set; } = string.Empty;
    public string client_address { get; set; } = string.Empty;
    public string client_archive { get; set; } = string.Empty;
}
