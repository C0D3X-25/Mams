using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.databaseOperations;
using Mams_App.src.globals;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.beehives;

/// <summary>
/// Provides functionality for managing a list of beehive items, including adding, modifying, deleting, and restoring
/// items. Supports filtering between archived and active items.
/// </summary>
public class ListBeehiveController : ABaseController
{

    private SolidColorBrush _m_delete_button_color = SGlobalView.DELETE_BUTTON_COLOR;
    public SolidColorBrush m_delete_button_color
    {
        get => _m_delete_button_color;
        set
        {
            if (_m_delete_button_color != value)
            {
                _m_delete_button_color = value;
                onPropertyChanged();
            }
        }
    }

    private SolidColorBrush _m_delete_button_text_color = SGlobalView.DELETE_BUTTON_TEXT_COLOR;
    public SolidColorBrush m_delete_button_text_color
    {
        get => _m_delete_button_text_color;
        set
        {
            if (_m_delete_button_text_color != value)
            {
                _m_delete_button_text_color = value;
                onPropertyChanged();
            }
        }
    }


    private readonly BeehiveModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }
    public ICommand m_double_click_command { get; set; }
    public ICommand m_sort_command { get; set; }



    private bool _m_is_show_archived_checked = false;
    public bool m_is_show_archived_checked
    {
        get { return _m_is_show_archived_checked; }
        set
        {
            _m_is_show_archived_checked = value;
            onPropertyChanged();
            updateListItems();
        }
    }

    private string _m_delete_button_text = string.Empty;
    public string m_delete_button_text
    {
        get { return _m_delete_button_text; }
        set
        {
            _m_delete_button_text = value;
            onPropertyChanged();
        }
    }

    private ObservableCollection<BeehiveItem>? _m_list_items;
    public ObservableCollection<BeehiveItem>? m_list_items
    {
        get { return _m_list_items; }
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    private BeehiveItem? _m_selected_item;
    public BeehiveItem? m_selected_item
    {
        get { return _m_selected_item; }
        set
        {
            _m_selected_item = value;
            onPropertyChanged();
            _ = updateDeleteButtonTextAsync();
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
    /// Initializes a new instance of the ListBeehiveController class.
    /// Sets up commands and loads the initial list of items.
    /// </summary>
    public ListBeehiveController()
    {
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteOrRestoreItem, isItemSelected);
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
    /// Updates the list of beehive items based on the archive status filter.
    /// </summary>
    private void updateListItems()
    {
        var all_items = _m_item_model.getAllItems().returned_items;

        if (!_m_is_show_archived_checked)
        {
            m_list_items = new ObservableCollection<BeehiveItem>(
                all_items.Where(item => item.beehive_archive == string.Empty)
            );
            m_delete_button_color = SGlobalView.DELETE_BUTTON_COLOR;
            m_delete_button_text_color = SGlobalView.DELETE_BUTTON_TEXT_COLOR;
        }
        else
        {
            m_list_items = new ObservableCollection<BeehiveItem>(
                all_items.Where(item => item.beehive_archive != string.Empty)
            );
            m_delete_button_color = SGlobalView.RESTORE_BUTTON_COLOR;
            m_delete_button_text_color = SGlobalView.DEFAULT_TEXT_COLOR;
        }
        _ = updateDeleteButtonTextAsync();
    }

    /// <summary>
    /// Updates the delete button text based on the selected item and archive status asynchronously.
    /// </summary>
    private async Task updateDeleteButtonTextAsync()
    {
        if (_m_is_show_archived_checked)
        {
            m_delete_button_text = Loc.Get("Common.Restore");
        }
        else if (_m_selected_item != null)
        {
            var selectedId = _m_selected_item.beehive_id;
            bool canHardDelete = await Task.Run(() => _m_item_model.canBeHardDeleted(selectedId.ToString()));

            if (_m_selected_item?.beehive_id == selectedId)
            {
                m_delete_button_text = canHardDelete ? Loc.Get("Common.Delete") : Loc.Get("Common.Archive");
            }
        }
        else
        {
            m_delete_button_text = Loc.Get("Common.Delete");
        }
    }


    /// <summary>
    /// Navigates to the save page for creating a new beehive item.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void navigateToSavePage(object? obj)
    {
        SPageNavigationController.navigateTo(new SaveBeehivePage());
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
    /// Navigates to the save page for modifying the selected beehive item.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    public void navigateToModifyPage(object? obj)
    {
        if (_m_selected_item != null)
        {
            SPageNavigationController.navigateTo(new SaveBeehivePage(_m_selected_item.beehive_id));
        }
    }


    /// <summary>
    /// Deletes or restores the selected item based on the current archive filter status.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void deleteOrRestoreItem(object? obj)
    {
        if (_m_selected_item != null)
        {
            ResponseDeleteItem result;
            if (!_m_is_show_archived_checked)
            {
                result = _m_item_model.deleteItem(_m_selected_item.beehive_id.ToString());
            }
            else
            {
                result = _m_item_model.deleteItem(_m_selected_item.beehive_id.ToString(), EDeleteItemOperation.RESTORE);
            }

            if (!result.is_success)
            {
                MessageBox.Show(Loc.Get("Message.DeleteErrorOccurred"),
                    Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            updateListItems();
        }
    }
}
