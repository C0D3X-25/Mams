using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.databaseOperations;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.globals;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.productsShapes;

public class ListProductShapeController : ABaseController
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


    private readonly ProductShapeModel _m_item_model;

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

    private ObservableCollection<ProductShapeItem>? _m_list_items;
    public ObservableCollection<ProductShapeItem>? m_list_items
    {
        get { return _m_list_items; }
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    private ProductShapeItem? _m_selected_item;
    public ProductShapeItem? m_selected_item
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


    public ListProductShapeController()
    {

        _m_item_model = new();
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


    private void updateListItems()
    {
        var all_items = _m_item_model.getAllItems().returned_items;

        if (!_m_is_show_archived_checked)
        {
            m_list_items = new ObservableCollection<ProductShapeItem>(
                all_items.Where(item => item.product_shape_archive == string.Empty)
            );
            m_delete_button_color = SGlobalView.DELETE_BUTTON_COLOR;
            m_delete_button_text_color = SGlobalView.DELETE_BUTTON_TEXT_COLOR;
        }
        else
        {
            m_list_items = new ObservableCollection<ProductShapeItem>(
                all_items.Where(item => item.product_shape_archive != string.Empty)
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
            var selectedId = _m_selected_item.product_shape_id;
            bool canHardDelete = await Task.Run(() => _m_item_model.canBeHardDeleted(selectedId.ToString()));
            
            if (_m_selected_item?.product_shape_id == selectedId)
            {
                m_delete_button_text = canHardDelete ? Loc.Get("Common.Delete") : Loc.Get("Common.Archive");
            }
        }
        else
        {
            m_delete_button_text = Loc.Get("Common.Delete");
        }
    }


    private void navigateToSavePage(object? obj)
    {
        SPageNavigationController.navigateTo(new SaveProductShapePage());
    }


    private bool isItemSelected(object? arg)
    {
        return m_selected_item != null;
    }


    public void navigateToModifyPage(object? obj)
    {
        if (_m_selected_item != null)
        {
            SPageNavigationController.navigateTo(new SaveProductShapePage(_m_selected_item.product_shape_id));
        }
    }


    private void deleteOrRestoreItem(object? obj)
    {
        if (_m_selected_item != null)
        {
            ResponseDeleteItem result;
            if (!_m_is_show_archived_checked)
            {
                result = _m_item_model.deleteItem(_m_selected_item.product_shape_id.ToString());
            }
            else
            {
                result = _m_item_model.deleteItem(_m_selected_item.product_shape_id.ToString(), EDeleteItemOperation.RESTORE);
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
