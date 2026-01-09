using Mams_App.src.items;

namespace Mams_App.src.users;

/// <summary>
/// Represents a user item containing the application owner's information for invoice generation.
/// </summary>
public class UserItem : ABaseItem
{
    public int user_id { get; set; } = 0;
    public string user_name { get; set; } = string.Empty;
    public string user_phone { get; set; } = string.Empty;
    public string user_email { get; set; } = string.Empty;
    public string user_city { get; set; } = string.Empty;
    public string user_address { get; set; } = string.Empty;
}
