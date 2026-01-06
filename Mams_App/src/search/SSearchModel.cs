using Mams_App.src.databaseOperations;
using System.Collections.ObjectModel;

namespace Mams_App.src.search; 
public static class SSearchModel
{
    public static ObservableCollection<DatabaseTablesNameItem>? m_list_table { get; set; }
    public static DatabaseTablesNameItem? m_selected_table { get; set; }
    public static ObservableCollection<SearchItem>? m_list_search_item { get; set; }
    public static SearchItem? m_selected_search_item { get; set; }
    public static ObservableCollection<SearchItem>? m_list_year { get; set; }
    public static SearchItem? m_selected_year { get; set; }

    public static void initialize()
    {
        m_list_table ??= new ObservableCollection<DatabaseTablesNameItem>();
        m_list_search_item ??= new ObservableCollection<SearchItem>();
        m_list_year ??= new ObservableCollection<SearchItem>();
    }
}
