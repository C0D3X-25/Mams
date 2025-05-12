using Mams.src.databaseOperations;

namespace Mams.src.resumes; 
public class SearchItem {
    public int search_id { get; set; } = 0;
    public EDatabaseTableName search_table { get; set; } = EDatabaseTableName.NONE;
    public string search_item_to_display { get; set; } = string.Empty;
    public string search_year { get; set; } = string.Empty;
}
