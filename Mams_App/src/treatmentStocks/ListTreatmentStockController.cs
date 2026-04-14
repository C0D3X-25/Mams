using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.treatmentStocks;

/// <summary>
/// Controller for the treatment stock list page that displays and manages treatment stocks.
/// </summary>
public class ListTreatmentStockController : ABaseController, IRefreshable
{
    public ICommand m_add_command { get; set; }
    public ICommand m_modify_command { get; set; }
    public ICommand m_delete_command { get; set; }
    public ICommand m_double_click_command { get; set; }
    public ICommand m_sort_command { get; set; }

    public string m_delete_button_text => Loc.Get("Common.Delete");

    private readonly TreatmentStockModel _m_treatment_stock_model = new();

    private ObservableCollection<TreatmentStockItem>? _m_list_items;
    public ObservableCollection<TreatmentStockItem>? m_list_items
    {
        get => _m_list_items;
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    private TreatmentStockItem? _m_selected_item;
    public TreatmentStockItem? m_selected_item
    {
        get => _m_selected_item;
        set
        {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }

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
    /// Initializes a new instance of the ListTreatmentStockController class.
    /// </summary>
    public ListTreatmentStockController()
    {
        loadItems();

        m_add_command = new RelayCommand(navigateToSavePage);
        m_modify_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_command = new RelayCommand(deleteItem, isItemSelected);
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_sort_command = new RelayCommand(sortByColumn);
    }

    /// <summary>
    /// Sorts the list by the specified column.
    /// </summary>
    private void sortByColumn(object? parameter)
    {
        var result = SortHelper.sortByColumn(parameter, m_list_items, _m_sorted_column, _m_sort_direction);
        if (result.HasValue)
        {
            m_sorted_column = result.Value.ColumnName;
            m_sort_direction = result.Value.Direction;
        }
    }

    /// <summary>
    /// Refreshes the list data from the database.
    /// </summary>
    public void refreshData() => loadItems();

    /// <summary>
    /// Loads all treatment stock items.
    /// </summary>
    private void loadItems()
    {
        var result = _m_treatment_stock_model.getAllItems();
        if (result.is_success && result.returned_items != null)
        {
            m_list_items = result.returned_items;
        }
        else
        {
            m_list_items = [];
        }
    }

    private bool isItemSelected(object? arg) => _m_selected_item != null;

    /// <summary>
    /// Navigates to the save page for creating a new treatment stock.
    /// </summary>
    private void navigateToSavePage(object? obj)
    {
        SPageNavigationController.navigateTo(new SaveTreatmentStockPage());
    }

    /// <summary>
    /// Navigates to the save page for modifying the selected treatment stock.
    /// </summary>
    public void navigateToModifyPage(object? obj)
    {
        if (_m_selected_item != null)
        {
            SPageNavigationController.navigateTo(new SaveTreatmentStockPage(_m_selected_item.treatment_stock_id));
        }
    }

    /// <summary>
    /// Deletes the selected treatment stock (hard delete).
    /// </summary>
    private void deleteItem(object? obj)
    {
        if (_m_selected_item != null)
        {
            var result = _m_treatment_stock_model.deleteItem(_m_selected_item.treatment_stock_id.ToString());

            if (!result.is_success)
            {
                MessageBox.Show(Loc.Get("Message.DeleteErrorOccurred"),
                    Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            loadItems();
        }
    }
}
