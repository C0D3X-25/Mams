using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Mams_App.src.searches;

/// <summary>
/// Static model to persist search state across navigation.
/// Data is preserved when navigating away and back to the search page.
/// </summary>
public static class SSearchModel
{
    public static string m_search_text { get; set; } = string.Empty;
    public static ObservableCollection<SearchItem>? m_list_items { get; set; }
    public static SearchItem? m_selected_item { get; set; }
    public static string m_sorted_column { get; set; } = string.Empty;
    public static ListSortDirection m_sort_direction { get; set; } = ListSortDirection.Ascending;
    public static int m_results_count { get; set; } = 0;

    public static void initialize()
    {
        m_list_items ??= [];
    }
}
