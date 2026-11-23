using Mams_App.src.items;

namespace Mams_App.src.searches;

/// <summary>
/// Represents a unified search result item that can hold data from any table in the database.
/// </summary>
public class SearchItem : ABaseItem
{
    /// <summary>
    /// The original ID from the source table.
    /// </summary>
    public int item_id { get; set; } = 0;

    /// <summary>
    /// The type/category of the item (e.g., "Product", "Entity", "Fee", "Profit", "Beehive", etc.)
    /// </summary>
    public string item_type { get; set; } = string.Empty;

    /// <summary>
    /// The name or primary identifier of the item.
    /// </summary>
    public string item_name { get; set; } = string.Empty;

    /// <summary>
    /// Custom ID for Fee or Profit (receipt_number).
    /// </summary>
    public string item_custom_id { get; set; } = string.Empty;

    /// <summary>
    /// The date value used for sorting.
    /// </summary>
    public DateOnly item_date_value { get; set; } = DateOnly.MinValue;

    /// <summary>
    /// The formatted date string for display (dd.MM.yyyy format).
    /// </summary>
    public string item_date
    {
        get => item_date_value == DateOnly.MinValue ? string.Empty : item_date_value.ToString(globals.SGlobals.g_EU_DATE_FORMAT);
        set
        {
            if (DateOnly.TryParseExact(value, globals.SGlobals.g_EU_DATE_FORMAT, out var parsed))
            {
                item_date_value = parsed;
            }
        }
    }

    /// <summary>
    /// Additional details about the item for display.
    /// </summary>
    public string item_details { get; set; } = string.Empty;
}
