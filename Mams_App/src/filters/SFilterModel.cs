using Mams_App.src.databaseOperations;
using System.Collections.ObjectModel;

namespace Mams_App.src.filters;

/// <summary>
/// Static model class that maintains shared filter state across the application.
/// Stores the current filter selections for tables, filter items, and years.
/// </summary>
public static class SFilterModel
{
    /// <summary>
    /// Gets or sets the collection of available database tables for filtering.
    /// </summary>
    public static ObservableCollection<DatabaseTablesNameItem>? m_list_table { get; set; }

    /// <summary>
    /// Gets or sets the currently selected database table for filtering.
    /// </summary>
    public static DatabaseTablesNameItem? m_selected_table { get; set; }

    /// <summary>
    /// Gets or sets the collection of available filter items based on the selected table.
    /// </summary>
    public static ObservableCollection<FilterItem>? m_list_filter_item { get; set; }

    /// <summary>
    /// Gets or sets the currently selected filter item.
    /// </summary>
    public static FilterItem? m_selected_filter_item { get; set; }

    /// <summary>
    /// Gets or sets the collection of available years for filtering.
    /// </summary>
    public static ObservableCollection<FilterItem>? m_list_year { get; set; }

    /// <summary>
    /// Gets or sets the currently selected year for filtering.
    /// </summary>
    public static FilterItem? m_selected_year { get; set; }

    /// <summary>
    /// Initializes the filter model collections if they are null.
    /// </summary>
    public static void initialize()
    {
        m_list_table ??= new ObservableCollection<DatabaseTablesNameItem>();
        m_list_filter_item ??= new ObservableCollection<FilterItem>();
        m_list_year ??= new ObservableCollection<FilterItem>();
    }
}
