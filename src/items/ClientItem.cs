namespace Mams.src.items;

public class ClientItem : ABaseItem {

    public int client_id { get; set; }
    public string client_name { get; set; } = String.Empty;
    public string client_phone { get; set; } = String.Empty;
    public string client_email { get; set; } = String.Empty;
    public string client_city { get; set; } = String.Empty;
    public string client_address { get; set; } = String.Empty;
    public string client_archive { get; set; } = String.Empty;
}
