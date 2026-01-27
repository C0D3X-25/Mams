using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.globals;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.fees;

/// <summary>
/// Controller for managing the list of fee items, handling fee-related operations and UI interactions.
/// </summary>
public class ListFeeController : ABaseController
{

    public SolidColorBrush m_delete_button_color { get; } = SGlobalView.DELETE_BUTTON_COLOR;
    public SolidColorBrush m_delete_button_text_color { get; } = SGlobalView.DELETE_BUTTON_TEXT_COLOR;
    public string m_delete_button_text => Loc.Get("Common.Delete");


    private readonly ReceiptFeeDetailedModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }
    public ICommand m_double_click_command { get; set; }
    public ICommand m_sort_command { get; set; }



    private ObservableCollection<ReceiptFeeDetailedItem>? _m_list_items;
    public ObservableCollection<ReceiptFeeDetailedItem>? m_list_items
    {
        get { return _m_list_items; }
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }


    private ReceiptFeeDetailedItem? _m_selected_item;
    public ReceiptFeeDetailedItem? m_selected_item
    {
        get { return _m_selected_item; }
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
    /// Initializes a new instance of the ListFeeController class.
    /// Sets up commands and loads the initial list of items.
    /// </summary>
    public ListFeeController()
    {
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteItem, isItemSelected);
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_sort_command = new RelayCommand(sortByColumn);
    }


    /// <summary>
    /// Sorts the list of items by the specified column.
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


    /// <summary>
    /// Updates the list of fee items from the database.
    /// </summary>
    private void updateListItems()
    {
        m_list_items = _m_item_model.getAllItems().returned_items;
    }


    /// <summary>
    /// Navigates to the save page for creating a new fee item.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void navigateToSavePage(object? obj)
    {
        SPageNavigationController.navigateTo(new SaveFeePage());
    }


    /// <summary>
    /// Determines whether an item is currently selected in the list.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if an item is selected; otherwise, false.</returns>
    private bool isItemSelected(object? arg)
    {
        return m_selected_item != null;
    }


    /// <summary>
    /// Navigates to the save page for modifying the selected fee item.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    public void navigateToModifyPage(object? obj)
    {
        if (_m_selected_item != null)
        {
            SPageNavigationController.navigateTo(new SaveFeePage(_m_selected_item.receipt.receipt_id));
        }
    }


    /// <summary>
    /// Deletes the selected fee item after user confirmation.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void deleteItem(object? obj)
    {
        if (_m_selected_item != null)
        {
            MessageBoxResult result = MessageBox.Show(Loc.Get("Message.DeleteInvoiceConfirm"),
                Loc.Get("Message.DeleteTitle"), MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            var deleteResult = _m_item_model.deleteItem(_m_selected_item.receipt.receipt_id.ToString());
            if (!deleteResult.is_success)
            {
                MessageBox.Show(Loc.Get("Message.DeleteErrorOccurred"),
                    Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            updateListItems();
        }
    }
}

