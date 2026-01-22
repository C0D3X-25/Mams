using Mams_App.src.databaseOperations;

namespace Mams_App.src.filters;

public class FilterItem
{
    public int filter_id { get; set; } = 0;
    public EDatabaseTableName filter_table { get; set; } = EDatabaseTableName.NONE;
    public string filter_item_to_display { get; set; } = string.Empty;
    public string filter_year { get; set; } = string.Empty;
}
