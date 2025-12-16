using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.databaseOperations;
using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.productsLots;

public class ListProductLotController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ProductLotModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }
    public ICommand m_double_click_command { get; set; }


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


    public ListProductLotController() {
        _m_delete_button_text = string.Empty;
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteOrRestoreItem, isItemSelected);
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
    }


    private void updateListItems() {
        var all_items = _m_item_model.getAllItems().returned_items;

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
        SPageNavigationController.navigateTo(new SaveProductLotPage());
    }


    private bool isItemSelected(object? arg) {
        return m_selected_item != null;
    }


    public void navigateToModifyPage(object? obj) {
        if (_m_selected_item != null) {
            SPageNavigationController.navigateTo(new SaveProductLotPage(_m_selected_item.product_lot_id));
        }
    }


    private void deleteOrRestoreItem(object? obj) {
        if (_m_selected_item != null) {
            ResponseDeleteItem result;
            if (_m_is_show_archived_checked) {
                result = _m_item_model.deleteItem(_m_selected_item.product_lot_id.ToString(), EDeleteItemOperation.RESTORE);
            }
            else {
                result = _m_item_model.deleteItem(_m_selected_item.product_lot_id.ToString());
            }
            
            if (!result.is_success) {
                MessageBox.Show("Une erreur s'est produite lors de la suppression.", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            updateListItems();
        }
    }
}
