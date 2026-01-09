using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.databaseOperations;
using Mams_App.src.navigations;
using Mams_App.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.productsTypes;

public class ListProductTypeController : ABaseController
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


    private readonly ProductTypeModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }
    public ICommand m_double_click_command { get; set; }


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

    private ObservableCollection<ProductTypeItem>? _m_list_items;
    public ObservableCollection<ProductTypeItem>? m_list_items
    {
        get { return _m_list_items; }
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    private ProductTypeItem? _m_selected_item;
    public ProductTypeItem? m_selected_item
    {
        get { return _m_selected_item; }
        set
        {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }


    public ListProductTypeController()
    {

        _m_delete_button_text = string.Empty;
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteOrRestoreItem, isItemSelected);
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
    }


    private void updateListItems()
    {
        var all_items = _m_item_model.getAllItems().returned_items;

        if (!_m_is_show_archived_checked)
        {
            m_list_items = new ObservableCollection<ProductTypeItem>(
                all_items.Where(item => item.product_type_archive == string.Empty)
            );
            m_delete_button_text = "Supprimer";
            m_delete_button_color = SGlobalView.DELETE_BUTTON_COLOR;
            m_delete_button_text_color = SGlobalView.DELETE_BUTTON_TEXT_COLOR;
        }
        else
        {
            m_list_items = new ObservableCollection<ProductTypeItem>(
                all_items.Where(item => item.product_type_archive != string.Empty)
            );
            m_delete_button_text = "Restaurer";
            m_delete_button_color = SGlobalView.RESTORE_BUTTON_COLOR;
            m_delete_button_text_color = SGlobalView.DEFAULT_TEXT_COLOR;
        }
    }


    private void navigateToSavePage(object? obj)
    {
        SPageNavigationController.navigateTo(new SaveProductTypePage());
    }


    private bool isItemSelected(object? arg)
    {
        return m_selected_item != null;
    }


    public void navigateToModifyPage(object? obj)
    {
        if (_m_selected_item != null)
        {
            SPageNavigationController.navigateTo(new SaveProductTypePage(_m_selected_item.product_type_id));
        }
    }


    private void deleteOrRestoreItem(object? obj)
    {
        if (_m_selected_item != null)
        {
            ResponseDeleteItem result;
            if (!_m_is_show_archived_checked)
            {
                result = _m_item_model.deleteItem(_m_selected_item.product_type_id.ToString());
            }
            else
            {
                result = _m_item_model.deleteItem(_m_selected_item.product_type_id.ToString(), EDeleteItemOperation.RESTORE);
            }

            if (!result.is_success)
            {
                MessageBox.Show("Une erreur s'est produite lors de la suppression.",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            updateListItems();
        }
    }
}
