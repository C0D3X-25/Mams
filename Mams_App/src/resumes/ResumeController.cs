using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.databaseOperations;
using Mams_App.src.fees;
using Mams_App.src.filters;
using Mams_App.src.globals;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.profits;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.resumes;

/// <summary>
/// Controller for the Resume page that displays summary of profits, fees, and transaction statistics.
/// Handles filtering and data aggregation for financial reports.
/// </summary>
public class ResumeController : ABaseController
{
    public ICommand m_clear_search_command { get; }
    public ICommand m_double_click_command_profit { get; }
    public ICommand m_double_click_command_fee { get; }
    public ICommand m_sort_profit_command { get; }
    public ICommand m_sort_fee_command { get; }

    private readonly ResumeModel _m_resume_model = new();

    // Bind directly to SFilterModel instead of local backing fields
    public ObservableCollection<DatabaseTablesNameItem>? m_list_table
    {
        get => SFilterModel.m_list_table;
        set
        {
            if (SFilterModel.m_list_table != value)
            {
                SFilterModel.m_list_table = value;
                onPropertyChanged();
            }
        }
    }

    public DatabaseTablesNameItem? m_selected_table
    {
        get => SFilterModel.m_selected_table;
        set
        {
            if (SFilterModel.m_selected_table != value)
            {
                SFilterModel.m_selected_table = value;
                updateListFilterItems();
                updateDisplayedProfitsAndFeesLists();
                onPropertyChanged();
                onPropertyChanged(nameof(m_is_filter_item_enabled));
            }
        }
    }

    /// <summary>
    /// Returns true if the filter item combobox should be enabled.
    /// Disabled when the first table item (index 0) is selected.
    /// </summary>
    public bool m_is_filter_item_enabled
    {
        get
        {
            if (m_list_table == null || m_list_table.Count == 0)
                return false;

            return m_selected_table != m_list_table[0];
        }
    }

    public ObservableCollection<FilterItem>? m_list_filter_item
    {
        get => SFilterModel.m_list_filter_item;
        set
        {
            if (SFilterModel.m_list_filter_item != value)
            {
                SFilterModel.m_list_filter_item = value;
                onPropertyChanged();
            }
        }
    }

    public FilterItem? m_selected_filter_item
    {
        get => SFilterModel.m_selected_filter_item;
        set
        {
            if (SFilterModel.m_selected_filter_item != value)
            {
                SFilterModel.m_selected_filter_item = value;
                updateDisplayedProfitsAndFeesLists();
                onPropertyChanged();
            }
        }
    }

    public ObservableCollection<FilterItem>? m_list_year
    {
        get => SFilterModel.m_list_year;
        set
        {
            if (SFilterModel.m_list_year != value)
            {
                SFilterModel.m_list_year = value;
                onPropertyChanged();
            }
        }
    }

    public FilterItem? m_selected_year
    {
        get => SFilterModel.m_selected_year;
        set
        {
            if (SFilterModel.m_selected_year != value)
            {
                SFilterModel.m_selected_year = value;
                updateDisplayedProfitsAndFeesLists();
                onPropertyChanged();
            }
        }
    }

    private ObservableCollection<ReceiptProfitDetailedItem>? _m_list_profit_item;
    public ObservableCollection<ReceiptProfitDetailedItem>? m_list_profit_item
    {
        get => _m_list_profit_item;
        set
        {
            if (_m_list_profit_item != value)
            {
                _m_list_profit_item = value;
                onPropertyChanged();
            }
        }
    }

    private ObservableCollection<ReceiptFeeDetailedItem>? _m_list_fee_item;
    public ObservableCollection<ReceiptFeeDetailedItem>? m_list_fee_item
    {
        get => _m_list_fee_item;
        set
        {
            if (_m_list_fee_item != value)
            {
                _m_list_fee_item = value;
                onPropertyChanged();
            }
        }
    }

    private decimal _m_total_profit;
    public decimal m_total_profit
    {
        get => _m_total_profit;
        set
        {
            if (_m_total_profit != value)
            {
                _m_total_profit = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_profit_ui));
            }
        }
    }
    public string m_total_profit_ui => $"{_m_total_profit:F2} {Loc.Currency}";

    private decimal _m_total_fee;
    public decimal m_total_fee
    {
        get => _m_total_fee;
        set
        {
            if (_m_total_fee != value)
            {
                _m_total_fee = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_fee_ui));
            }
        }
    }
    public string m_total_fee_ui => $"{_m_total_fee:F2} {Loc.Currency}";

    private decimal _m_total;
    public decimal m_total
    {
        get => _m_total;
        set
        {
            if (_m_total != value)
            {
                _m_total = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_ui));
            }
        }
    }
    public string m_total_ui => $"{_m_total:F2} {Loc.Currency}";

    private decimal _m_total_weight;
    public decimal m_total_weight
    {
        get => _m_total_weight;
        set
        {
            if (_m_total_weight != value)
            {
                _m_total_weight = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_weight_ui));
            }
        }
    }
    public string m_total_weight_ui => $"{_m_total_weight:F2} {Loc.Get("Unit.Kg")}";

    private int _m_total_quantity;
    public int m_total_quantity
    {
        get => _m_total_quantity;
        set
        {
            if (_m_total_quantity != value)
            {
                _m_total_quantity = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_quantity_ui));
            }
        }
    }
    public string m_total_quantity_ui => $"{_m_total_quantity}";

    private decimal _m_average_price_per_unit;
    public decimal m_average_price_per_unit
    {
        get => _m_average_price_per_unit;
        set
        {
            if (_m_average_price_per_unit != value)
            {
                _m_average_price_per_unit = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_average_price_per_unit_ui));
            }
        }
    }
    public string m_average_price_per_unit_ui => $"{m_average_price_per_unit:F2} {Loc.Currency}/u";

    private decimal _m_average_price_per_weight;
    public decimal m_average_price_per_weight
    {
        get => _m_average_price_per_weight;
        set
        {
            if (_m_average_price_per_weight != value)
            {
                _m_average_price_per_weight = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_average_price_per_weight_ui));
            }
        }
    }
    public string m_average_price_per_weight_ui => $"{m_average_price_per_weight:F2} {Loc.Currency}/kg";

    private bool _m_include_free_items;
    /// <summary>
    /// When true, includes items with price of 0 (gifts/free) in average calculations.
    /// </summary>
    public bool m_include_free_items
    {
        get => _m_include_free_items;
        set
        {
            if (_m_include_free_items != value)
            {
                _m_include_free_items = value;
                onPropertyChanged();
                updateDisplayedDetailTransactions();
            }
        }
    }

    private SolidColorBrush _m_total_fee_color = SGlobalView.DEFAULT_TEXT_COLOR;
    public SolidColorBrush m_total_fee_color
    {
        get => _m_total_fee_color;
        set
        {
            if (_m_total_fee_color != value)
            {
                _m_total_fee_color = value;
                onPropertyChanged();
            }
        }
    }

    private SolidColorBrush _m_total_color = SGlobalView.DEFAULT_TEXT_COLOR;
    public SolidColorBrush m_total_color
    {
        get => _m_total_color;
        set
        {
            if (_m_total_color != value)
            {
                _m_total_color = value;
                onPropertyChanged();
            }
        }
    }

    private ReceiptProfitDetailedItem? _m_selected_profit_item;
    public ReceiptProfitDetailedItem? m_selected_profit_item
    {
        get => _m_selected_profit_item;
        set
        {
            if (_m_selected_profit_item != value)
            {
                _m_selected_profit_item = value;
                onPropertyChanged();
            }
        }
    }

    private ReceiptFeeDetailedItem? _m_selected_fee_item;
    public ReceiptFeeDetailedItem? m_selected_fee_item
    {
        get => _m_selected_fee_item;
        set
        {
            if (_m_selected_fee_item != value)
            {
                _m_selected_fee_item = value;
                onPropertyChanged();
            }
        }
    }

    private string _m_profit_sorted_column = string.Empty;
    public string m_profit_sorted_column
    {
        get { return _m_profit_sorted_column; }
        set
        {
            _m_profit_sorted_column = value;
            onPropertyChanged();
        }
    }

    private ListSortDirection _m_profit_sort_direction = ListSortDirection.Ascending;
    public ListSortDirection m_profit_sort_direction
    {
        get { return _m_profit_sort_direction; }
        set
        {
            _m_profit_sort_direction = value;
            onPropertyChanged();
        }
    }

    private string _m_fee_sorted_column = string.Empty;
    public string m_fee_sorted_column
    {
        get { return _m_fee_sorted_column; }
        set
        {
            _m_fee_sorted_column = value;
            onPropertyChanged();
        }
    }

    private ListSortDirection _m_fee_sort_direction = ListSortDirection.Ascending;
    public ListSortDirection m_fee_sort_direction
    {
        get { return _m_fee_sort_direction; }
        set
        {
            _m_fee_sort_direction = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the ResumeController class.
    /// Sets up commands and loads filter data and initial display.
    /// </summary>
    public ResumeController()
    {
        m_clear_search_command = new RelayCommand(clearSelectedItems);
        m_double_click_command_profit = new RelayCommand(navigateToProfitDetails, isProfitItemSelected);
        m_double_click_command_fee = new RelayCommand(navigateToFeeDetails, isFeeItemSelected);
        m_sort_profit_command = new RelayCommand(sortProfitByColumn);
        m_sort_fee_command = new RelayCommand(sortFeeByColumn);

        loadStaticFilterData();
        updateDisplayedProfitsAndFeesLists();
    }

    /// <summary>
    /// Sorts the profit list by the specified column.
    /// </summary>
    /// <param name="parameter">The column name to sort by.</param>
    private void sortProfitByColumn(object? parameter)
    {
        var result = SortHelper.sortByColumn(parameter, m_list_profit_item, _m_profit_sorted_column, _m_profit_sort_direction);
        if (result.HasValue)
        {
            m_profit_sorted_column = result.Value.ColumnName;
            m_profit_sort_direction = result.Value.Direction;
        }
    }

    /// <summary>
    /// Sorts the fee list by the specified column.
    /// </summary>
    /// <param name="parameter">The column name to sort by.</param>
    private void sortFeeByColumn(object? parameter)
    {
        var result = SortHelper.sortByColumn(parameter, m_list_fee_item, _m_fee_sorted_column, _m_fee_sort_direction);
        if (result.HasValue)
        {
            m_fee_sorted_column = result.Value.ColumnName;
            m_fee_sort_direction = result.Value.Direction;
        }
    }

    /// <summary>
    /// Navigates to the profit details page for the selected profit item.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    public void navigateToProfitDetails(object? obj)
    {
        if (_m_selected_profit_item != null)
        {
            SPageNavigationController.navigateTo(new SaveProfitPage(_m_selected_profit_item.receipt.receipt_id));
        }
    }

    /// <summary>
    /// Navigates to the fee details page for the selected fee item.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    public void navigateToFeeDetails(object? obj)
    {
        if (_m_selected_fee_item != null)
        {
            SPageNavigationController.navigateTo(new SaveFeePage(_m_selected_fee_item.receipt.receipt_id));
        }
    }

    /// <summary>
    /// Clears all selected filter items and resets to default selections.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void clearSelectedItems(object? obj)
    {
        if (m_list_table?.Count > 0)
        {
            m_selected_table = m_list_table[0];
        }

        if (m_list_filter_item?.Count > 0)
        {
            m_selected_filter_item = m_list_filter_item[0];
        }

        if (m_list_year?.Count > 0)
        {
            m_selected_year = m_list_year[0];
        }

        // Force update display
        updateDisplayedProfitsAndFeesLists();
    }

    /// <summary>
    /// Determines whether a profit item is currently selected.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if a profit item is selected; otherwise, false.</returns>
    private bool isProfitItemSelected(object? arg) => m_selected_profit_item != null;

    /// <summary>
    /// Determines whether a fee item is currently selected.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if a fee item is selected; otherwise, false.</returns>
    private bool isFeeItemSelected(object? arg) => m_selected_fee_item != null;

    /// <summary>
    /// Loads static filter data from the model and sets default selections.
    /// Initializes tables, filter items, and years if not already set.
    /// </summary>
    private void loadStaticFilterData()
    {
        if (SFilterModel.m_list_table == null)
        {
            SFilterModel.m_list_table = _m_resume_model.m_filter_tables;
        }

        if (SFilterModel.m_list_filter_item == null)
        {
            updateListFilterItems();
        }

        if (SFilterModel.m_list_year == null)
        {
            SFilterModel.m_list_year = _m_resume_model.getListYears();
        }

        // Set default selections to element 0 if not already set
        if (SFilterModel.m_selected_table == null && SFilterModel.m_list_table?.Count > 0)
        {
            SFilterModel.m_selected_table = SFilterModel.m_list_table[0];
        }

        if (SFilterModel.m_selected_filter_item == null && SFilterModel.m_list_filter_item?.Count > 0)
        {
            SFilterModel.m_selected_filter_item = SFilterModel.m_list_filter_item[0];
        }

        if (SFilterModel.m_selected_year == null && SFilterModel.m_list_year?.Count > 0)
        {
            SFilterModel.m_selected_year = SFilterModel.m_list_year[0];
        }

        onPropertyChanged(nameof(m_list_table));
        onPropertyChanged(nameof(m_selected_table));
        onPropertyChanged(nameof(m_list_filter_item));
        onPropertyChanged(nameof(m_selected_filter_item));
        onPropertyChanged(nameof(m_list_year));
        onPropertyChanged(nameof(m_selected_year));
        onPropertyChanged(nameof(m_is_filter_item_enabled));
    }

    /// <summary>
    /// Updates the displayed profit and fee lists based on current filter selections.
    /// </summary>
    private void updateDisplayedProfitsAndFeesLists()
    {
        var filter_item = m_selected_filter_item ?? new FilterItem();
        var table_item = m_selected_table ?? new DatabaseTablesNameItem();
        var year_item = m_selected_year ?? new FilterItem();

        filter_item.filter_table = table_item.m_name_in_database;
        filter_item.filter_year = year_item.filter_year;

        var resume_item = _m_resume_model.getFilteredResume(filter_item);

        m_list_profit_item = resume_item.profit_items;
        m_list_fee_item = resume_item.fee_items;

        updateDisplayedTotalTransactions();
        updateDisplayedDetailTransactions();
    }

    /// <summary>
    /// Updates the total transaction amounts and colors based on current data.
    /// </summary>
    private void updateDisplayedTotalTransactions()
    {
        var transaction_totals = _m_resume_model.calculateTotalTransactions(m_list_profit_item, m_list_fee_item);

        m_total_profit = transaction_totals.total_profit;
        m_total_fee = transaction_totals.total_fee;
        m_total = transaction_totals.total;

        // Update colors based on negative values
        m_total_fee_color = transaction_totals.total_fee < 0 ? SGlobalView.NEGATIVE_VALUE_COLOR : SGlobalView.DEFAULT_TEXT_COLOR;
        m_total_color = transaction_totals.total < 0 ? SGlobalView.NEGATIVE_VALUE_COLOR : SGlobalView.DEFAULT_TEXT_COLOR;
    }

    /// <summary>
    /// Updates the detailed transaction statistics (weight, quantity, averages).
    /// </summary>
    private void updateDisplayedDetailTransactions()
    {
        var transaction_details = _m_resume_model.calculateDetailTransactions(
            m_list_profit_item,
            m_selected_filter_item,
            m_total_profit,
            m_include_free_items);

        m_total_weight = transaction_details.total_weight_kg;
        m_total_quantity = transaction_details.total_quantity;
        m_average_price_per_unit = transaction_details.average_price_per_unit;
        m_average_price_per_weight = transaction_details.average_price_per_weight;
    }

    /// <summary>
    /// Updates the list of filter items based on the selected table.
    /// </summary>
    private void updateListFilterItems()
    {
        if (m_selected_table != null)
        {
            SFilterModel.m_list_filter_item = _m_resume_model.getListFilterItems(m_selected_table);
            SFilterModel.m_list_year = _m_resume_model.getListYears();

            onPropertyChanged(nameof(m_list_filter_item));
            onPropertyChanged(nameof(m_selected_filter_item));
            onPropertyChanged(nameof(m_list_year));
        }
    }
}