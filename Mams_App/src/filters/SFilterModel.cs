using Mams_App.src.databaseOperations;
using System.Collections.ObjectModel;

namespace Mams_App.src.filters;

public static class SFilterModel
{
    public static ObservableCollection<DatabaseTablesNameItem>? m_list_table { get; set; }
    public static DatabaseTablesNameItem? m_selected_table { get; set; }
    public static ObservableCollection<FilterItem>? m_list_filter_item { get; set; }
    public static FilterItem? m_selected_filter_item { get; set; }
    public static ObservableCollection<FilterItem>? m_list_year { get; set; }
    public static FilterItem? m_selected_year { get; set; }

    public static void initialize()
    {
        m_list_table ??= new ObservableCollection<DatabaseTablesNameItem>();
        m_list_filter_item ??= new ObservableCollection<FilterItem>();
        m_list_year ??= new ObservableCollection<FilterItem>();
    }
}
