using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.crudOperations;
using Mams.src.navigations;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mams.src.productsLots;

public class ListLotController : ABaseController {

    private readonly PageNavigationController _m_page_navigation;
    private readonly ProductLotModel _m_item_model = new();

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

    private ObservableCollection<ProductLotItem>? _m_list_items;
    public ObservableCollection<ProductLotItem>? m_list_items {
        get { return _m_list_items; }
        set {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    private ProductLotItem? _m_selected_item;
    public ProductLotItem? m_selected_item {
        get { return _m_selected_item; }
        set {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }


    public ListLotController(PageNavigationController page_navigation) {
        _m_page_navigation = page_navigation;
        _m_delete_button_text = "";
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteOrRestoreItem, isItemSelected);
    }


    private void updateListItems() {
        var all_items = _m_item_model.getTable();

        if (!_m_is_show_archived_checked) {
            m_list_items = new ObservableCollection<ProductLotItem>(
                all_items.Where(item => item.product_lot_archive == string.Empty)
            );
            m_delete_button_text = "Supprimer";
        }
        else {
            m_list_items = new ObservableCollection<ProductLotItem>(
                all_items.Where(item => item.product_lot_archive != string.Empty)
            );
            m_delete_button_text = "Restaurer";
        }
    }


    private void navigateToSavePage(object? obj) {
        _m_page_navigation.navigateTo(new SaveProductLotPage());
    }


    private bool isItemSelected(object? arg) {
        return m_selected_item != null;
    }


    private void navigateToModifyPage(object? obj) {
        if (_m_selected_item != null) {
            _m_page_navigation.navigateTo(new SaveProductLotPage(_m_selected_item.product_lot_id));
        }
    }


    private void deleteOrRestoreItem(object? obj) {
        if (_m_selected_item != null) {
            if (_m_is_show_archived_checked) {
                _m_item_model.deleteItem(_m_selected_item.product_lot_id, EDatabaseDeleteItem.RESTORE);
            }
            else {
                _m_item_model.deleteItem(_m_selected_item.product_lot_id);
            }
            updateListItems();
        }
    }
}
