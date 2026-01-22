using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.helpers;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Mams_App.src.searches;

/// <summary>
/// Controller for the search page following MVVM pattern.
/// Handles global search across multiple database tables.
/// </summary>
public class SearchController : ABaseController
{
    private readonly SearchModel _m_search_model = new();

    /// <summary>
    /// Command to execute the search operation.
    /// </summary>
    public ICommand m_search_command { get; set; }

    /// <summary>
    /// Command to sort the search results by a specific column.
    /// </summary>
    public ICommand m_sort_command { get; set; }

    /// <summary>
    /// The text entered by the user for searching.
    /// </summary>
    private string _m_search_text = string.Empty;
    public string m_search_text
    {
        get { return _m_search_text; }
        set
        {
            _m_search_text = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Collection of search result items displayed in the list.
    /// </summary>
    private ObservableCollection<SearchItem>? _m_list_items;
    public ObservableCollection<SearchItem>? m_list_items
    {
        get { return _m_list_items; }
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// The currently selected item in the search results list.
    /// </summary>
    private SearchItem? _m_selected_item;
    public SearchItem? m_selected_item
    {
        get { return _m_selected_item; }
        set
        {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// The name of the column currently used for sorting.
    /// </summary>
    private string _m_sorted_column = string.Empty;
    public string m_sorted_column
    {
        get { return _m_sorted_column; }
        set
        {
            _m_sorted_column = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// The current sort direction (ascending or descending).
    /// </summary>
    private ListSortDirection _m_sort_direction = ListSortDirection.Ascending;
    public ListSortDirection m_sort_direction
    {
        get { return _m_sort_direction; }
        set
        {
            _m_sort_direction = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// The total number of search results found.
    /// </summary>
    private int _m_results_count = 0;
    public int m_results_count
    {
        get { return _m_results_count; }
        set
        {
            _m_results_count = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// Sets up the search and sort commands.
    /// </summary>
    public SearchController()
    {
        m_list_items = [];
        m_search_command = new RelayCommand(executeSearch);
        m_sort_command = new RelayCommand(sortByColumn);
    }

    /// <summary>
    /// Executes the search operation using the current search text.
    /// Searches across all relevant database tables (products, entities, beehives, etc.).
    /// </summary>
    /// <param name="parameter">Command parameter (not used).</param>
    private void executeSearch(object? parameter)
    {
        if (string.IsNullOrWhiteSpace(m_search_text))
        {
            m_list_items = [];
            m_results_count = 0;
            return;
        }

        var response = _m_search_model.searchAllItems(m_search_text);

        if (response.is_success && response.returned_items != null)
        {
            m_list_items = response.returned_items;
            m_results_count = response.returned_items.Count;
        }
        else
        {
            m_list_items = [];
            m_results_count = 0;
        }
    }

    /// <summary>
    /// Sorts the search results by the specified column.
    /// Toggles between ascending and descending order if the same column is clicked.
    /// </summary>
    /// <param name="parameter">The column name to sort by.</param>
    private void sortByColumn(object? parameter)
    {
        var result = SortHelper.sortByColumn(parameter, m_list_items, _m_sorted_column, _m_sort_direction);
        if (result.HasValue)
        {
            m_sorted_column = result.Value.ColumnName;
            m_sort_direction = result.Value.Direction;
        }
    }
}
