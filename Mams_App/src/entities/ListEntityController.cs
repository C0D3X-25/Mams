using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.databaseOperations;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.views.globalView;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.entities;

/// <summary>
/// Controller for managing the list of clients page, handling client-related operations and UI interactions.
/// </summary>
public class ListEntityController : ABaseController
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


    private readonly EntityModel _m_item_model = new();

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


    private string _m_delete_button_text;
    public string m_delete_button_text
    {
        get { return _m_delete_button_text; }
        set
        {
            _m_delete_button_text = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<EntityItem>? _m_list_items;
    public ObservableCollection<EntityItem>? m_list_items
    {
        get { return _m_list_items; }
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }


    private EntityItem? _m_selected_item;
    public EntityItem? m_selected_item
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
    /// Initializes a new instance of the ListClientPageController
    /// </summary>
    /// <param name="page_navigation">The navigation controller for managing page transitions</param>
    public ListEntityController()
    {

        _m_delete_button_text = string.Empty;
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteOrRestoreItem, isItemSelected);
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_sort_command = new RelayCommand(sortByColumn);
    }


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
    /// Updates the list of clients based on the archive status filter
    /// </summary>
    private void updateListItems()
    {
        var all_items = _m_item_model.getAllItems().returned_items;

        if (!_m_is_show_archived_checked)
        {
            m_list_items = new ObservableCollection<EntityItem>(
                all_items.Where(item => item.entity_archive == string.Empty)
            );
            m_delete_button_color = SGlobalView.DELETE_BUTTON_COLOR;
            m_delete_button_text_color = SGlobalView.DELETE_BUTTON_TEXT_COLOR;
        }
        else
        {
            m_list_items = new ObservableCollection<EntityItem>(
                all_items.Where(item => item.entity_archive != string.Empty)
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
            var selectedId = _m_selected_item.entity_id;
            bool canHardDelete = await Task.Run(() => _m_item_model.canBeHardDeleted(selectedId.ToString()));
            
            // Verify the selection hasn't changed while we were checking
            if (_m_selected_item?.entity_id == selectedId)
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
    /// Navigates to the save client page for creating a new client
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void navigateToSavePage(object? obj)
    {
        SPageNavigationController.navigateTo(new SaveEntityPage());
    }


    /// <summary>
    /// Determines if a client is currently selected
    /// </summary>
    /// <param name="arg">Command parameter (not used)</param>
    /// <returns>True if a client is selected, false otherwise</returns>
    private bool isItemSelected(object? arg)
    {
        return m_selected_item != null;
    }


    /// <summary>
    /// Navigates to the save client page for modifying an existing client
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void navigateToModifyPage(object? obj)
    {
        if (_m_selected_item != null)
        {
            SPageNavigationController.navigateTo(new SaveEntityPage(_m_selected_item.entity_id));
        }
    }


    /// <summary>
    /// Deletes or restores the selected client based on current archive status
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void deleteOrRestoreItem(object? obj)
    {
        if (_m_selected_item != null)
        {
            ResponseDeleteItem result;
            if (!_m_is_show_archived_checked)
            {
                result = _m_item_model.deleteItem(_m_selected_item.entity_id.ToString());
            }
            else
            {
                result = _m_item_model.deleteItem(_m_selected_item.entity_id.ToString(), EDeleteItemOperation.RESTORE);
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
