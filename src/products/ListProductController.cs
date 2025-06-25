using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.databaseOperations;
using Mams.src.navigations;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mams.src.products;

public class ListProductController : ABaseController {
    
    private readonly ProductModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }


    private bool _m_is_show_archived_checked = false;
    public bool m_is_show_archived_checked {
        get { return _m_is_show_archived_checked; }
        set {
            _m_is_show_archived_checked = value;
            onPropertyChanged();
            updateListItems();
        }
    }

    private string _m_delete_button_text;
    public string m_delete_button_text {
        get { return _m_delete_button_text; }
        set {
            _m_delete_button_text = value;
            onPropertyChanged();
        }
    }

    private ObservableCollection<ProductItem>? _m_list_items;
    public ObservableCollection<ProductItem>? m_list_items {
        get { return _m_list_items; }
        set {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    private ProductItem? _m_selected_item;
    public ProductItem? m_selected_item {
        get { return _m_selected_item; }
        set {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }


    public ListProductController() {
        
        _m_delete_button_text = string.Empty;
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteOrRestoreItem, isItemSelected);
    }


    private void updateListItems() {
        var all_items = _m_item_model.getTable();

        if (!_m_is_show_archived_checked) {
            m_list_items = new ObservableCollection<ProductItem>(
                all_items.Where(item => item.product_archive == string.Empty)
            );
            m_delete_button_text = "Supprimer";
        }
        else {
            m_list_items = new ObservableCollection<ProductItem>(
                all_items.Where(item => item.product_archive != string.Empty)
            );
            m_delete_button_text = "Restaurer";
        }
    }


    private void navigateToSavePage(object? obj) {
        SPageNavigationController.navigateTo(new SaveProductPage());
    }


    private bool isItemSelected(object? arg) {
        return m_selected_item != null;
    }


    private void navigateToModifyPage(object? obj) {
        if (_m_selected_item != null) {
            SPageNavigationController.navigateTo(new SaveProductPage(_m_selected_item.product_id));
        }
    }


    private void deleteOrRestoreItem(object? obj) {
        if (_m_selected_item != null) {
            if (_m_is_show_archived_checked) {
                _m_item_model.deleteItem(_m_selected_item.product_id.ToString(), EDeleteItemOperation.RESTORE);
            }
            else {
                _m_item_model.deleteItem(_m_selected_item.product_id.ToString());
            }
            updateListItems();
        }
    }
}

