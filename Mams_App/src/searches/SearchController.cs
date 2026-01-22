using Mams_App.src.beehives;
using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.entities;
using Mams_App.src.fees;
using Mams_App.src.helpers;
using Mams_App.src.navigations;
using Mams_App.src.products;
using Mams_App.src.productsCategories;
using Mams_App.src.productsLots;
using Mams_App.src.productsShapes;
using Mams_App.src.productsTypes;
using Mams_App.src.profits;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Threading;

namespace Mams_App.src.searches;

/// <summary>
/// Controller for the search page following MVVM pattern.
/// Handles global search across multiple database tables.
/// </summary>
public class SearchController : ABaseController
{
    private readonly SearchModel _m_search_model = new();
    
    /// <summary>
    /// Timer for debouncing search input to avoid excessive searches while typing.
    /// </summary>
    private readonly DispatcherTimer _m_debounce_timer;
    
    /// <summary>
    /// Cancellation token source to cancel in-flight searches when new input arrives.
    /// </summary>
    private CancellationTokenSource? _m_search_cts;
    
    /// <summary>
    /// Minimum number of characters required to trigger automatic search.
    /// </summary>
    private const int MIN_SEARCH_LENGTH = 2;
    
    /// <summary>
    /// Debounce delay in milliseconds before triggering search after typing stops.
    /// </summary>
    private const int DEBOUNCE_DELAY_MS = 250;

    /// <summary>
    /// Command to execute the search operation.
    /// </summary>
    public ICommand m_search_command { get; set; }

    /// <summary>
    /// Command to sort the search results by a specific column.
    /// </summary>
    public ICommand m_sort_command { get; set; }

    /// <summary>
    /// Command to navigate to the selected item's detail page on double-click.
    /// </summary>
    public ICommand m_double_click_command { get; set; }

    /// <summary>
    /// The text entered by the user for searching.
    /// Automatically triggers debounced search when at least 2 characters are entered.
    /// </summary>
    public string m_search_text
    {
        get { return SSearchModel.m_search_text; }
        set
        {
            SSearchModel.m_search_text = value;
            onPropertyChanged();
            onSearchTextChanged();
        }
    }

    /// <summary>
    /// Collection of search result items displayed in the list.
    /// </summary>
    public ObservableCollection<SearchItem>? m_list_items
    {
        get { return SSearchModel.m_list_items; }
        set
        {
            SSearchModel.m_list_items = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// The currently selected item in the search results list.
    /// </summary>
    public SearchItem? m_selected_item
    {
        get { return SSearchModel.m_selected_item; }
        set
        {
            SSearchModel.m_selected_item = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// The name of the column currently used for sorting.
    /// </summary>
    public string m_sorted_column
    {
        get { return SSearchModel.m_sorted_column; }
        set
        {
            SSearchModel.m_sorted_column = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// The current sort direction (ascending or descending).
    /// </summary>
    public ListSortDirection m_sort_direction
    {
        get { return SSearchModel.m_sort_direction; }
        set
        {
            SSearchModel.m_sort_direction = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// The total number of search results found.
    /// </summary>
    public int m_results_count
    {
        get { return SSearchModel.m_results_count; }
        set
        {
            SSearchModel.m_results_count = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchController"/> class.
    /// Sets up the search, sort, and double-click commands.
    /// </summary>
    public SearchController()
    {
        SSearchModel.initialize();
        m_search_command = new RelayCommand(executeSearch);
        m_sort_command = new RelayCommand(sortByColumn);
        m_double_click_command = new RelayCommand(navigateToItemPage, isItemSelected);
        
        // Initialize debounce timer
        _m_debounce_timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(DEBOUNCE_DELAY_MS)
        };
        _m_debounce_timer.Tick += onDebounceTimerTick;
    }
    
    /// <summary>
    /// Called when the search text changes. Restarts the debounce timer.
    /// </summary>
    private void onSearchTextChanged()
    {
        // Cancel any pending search
        _m_search_cts?.Cancel();
        
        // Stop and restart the debounce timer
        _m_debounce_timer.Stop();
        
        if (string.IsNullOrWhiteSpace(m_search_text) || m_search_text.Length < MIN_SEARCH_LENGTH)
        {
            // Clear results if text is too short
            m_list_items = [];
            m_results_count = 0;
            return;
        }
        
        _m_debounce_timer.Start();
    }
    
    /// <summary>
    /// Called when the debounce timer elapses. Triggers the async search.
    /// </summary>
    private void onDebounceTimerTick(object? sender, EventArgs e)
    {
        _m_debounce_timer.Stop();
        _ = executeSearchAsync();
    }
    
    /// <summary>
    /// Executes the search asynchronously with cancellation support.
    /// </summary>
    private async Task executeSearchAsync()
    {
        if (string.IsNullOrWhiteSpace(m_search_text) || m_search_text.Length < MIN_SEARCH_LENGTH)
        {
            m_list_items = [];
            m_results_count = 0;
            return;
        }
        
        // Cancel previous search and create new cancellation token
        _m_search_cts?.Cancel();
        _m_search_cts = new CancellationTokenSource();
        var cancellationToken = _m_search_cts.Token;
        
        // Clear current results
        m_list_items = [];
        m_results_count = 0;
        
        try
        {
            var searchText = m_search_text;
            
            // Execute search asynchronously with progressive updates
            await _m_search_model.searchAllItemsAsync(
                searchText, 
                cancellationToken,
                onBatchCompleted: (items) =>
                {
                    if (cancellationToken.IsCancellationRequested) return;
                    
                    // Update UI on dispatcher thread
                    System.Windows.Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (cancellationToken.IsCancellationRequested) return;
                        
                        foreach (var item in items)
                        {
                            m_list_items?.Add(item);
                        }
                        m_results_count = m_list_items?.Count ?? 0;
                    });
                });
        }
        catch (OperationCanceledException)
        {
            // Search was cancelled, ignore
        }
    }

    /// <summary>
    /// Determines whether an item is currently selected in the search results.
    /// </summary>
    /// <param name="parameter">Command parameter (not used).</param>
    /// <returns>True if an item is selected; otherwise, false.</returns>
    private bool isItemSelected(object? parameter)
    {
        return m_selected_item != null;
    }

    /// <summary>
    /// Navigates to the appropriate detail/edit page based on the selected item's type.
    /// </summary>
    /// <param name="parameter">Command parameter (not used).</param>
    private void navigateToItemPage(object? parameter)
    {
        if (m_selected_item == null)
        {
            return;
        }

        int itemId = m_selected_item.item_id;

        switch (m_selected_item.item_type)
        {
            case "Product":
                SPageNavigationController.navigateTo(new SaveProductPage(itemId));
                break;
            case "Entity":
                SPageNavigationController.navigateTo(new SaveEntityPage(itemId));
                break;
            case "Beehive":
                SPageNavigationController.navigateTo(new SaveBeehivePage(itemId));
                break;
            case "ProductLot":
                SPageNavigationController.navigateTo(new SaveProductLotPage(itemId));
                break;
            case "ProductType":
                SPageNavigationController.navigateTo(new SaveProductTypePage(itemId));
                break;
            case "ProductCategory":
                SPageNavigationController.navigateTo(new SaveProductCategoryPage(itemId));
                break;
            case "ProductShape":
                SPageNavigationController.navigateTo(new SaveProductShapePage(itemId));
                break;
            case "Fee":
                SPageNavigationController.navigateTo(new SaveFeePage(itemId));
                break;
            case "Profit":
                SPageNavigationController.navigateTo(new SaveProfitPage(itemId));
                break;
        }
    }

    /// <summary>
    /// Executes the search operation using the current search text.
    /// Searches across all relevant database tables (products, entities, beehives, etc.).
    /// Called when button is clicked or Enter is pressed (bypasses debounce).
    /// </summary>
    /// <param name="parameter">Command parameter (not used).</param>
    private void executeSearch(object? parameter)
    {
        // Stop debounce timer and trigger immediate search
        _m_debounce_timer.Stop();
        
        if (string.IsNullOrWhiteSpace(m_search_text))
        {
            m_list_items = [];
            m_results_count = 0;
            return;
        }

        _ = executeSearchAsync();
    }

    /// <summary>
    /// Sorts the search results by the specified column.
    /// Toggles between ascending and descending order if the same column is clicked.
    /// </summary>
    /// <param name="parameter">The column name to sort by.</param>
    private void sortByColumn(object? parameter)
    {
        var result = SortHelper.sortByColumn(parameter, m_list_items, m_sorted_column, m_sort_direction);
        if (result.HasValue)
        {
            m_sorted_column = result.Value.ColumnName;
            m_sort_direction = result.Value.Direction;
        }
    }
}
