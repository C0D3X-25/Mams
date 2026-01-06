using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.databaseOperations;
using Mams_App.src.navigations;
using Mams_App.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.productsCategories;

public class ListProductCategoryController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ProductCategoryModel _m_item_model = new();

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

    private ObservableCollection<ProductCategoryItem>? _m_list_items;
    public ObservableCollection<ProductCategoryItem>? m_list_items {
        get { return _m_list_items; }
        set {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    private ProductCategoryItem? _m_selected_item;
    public ProductCategoryItem? m_selected_item {
        get { return _m_selected_item; }
        set {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }


    public ListProductCategoryController() {
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
            m_list_items = new ObservableCollection<ProductCategoryItem>(
                all_items.Where(item => item.product_category_archive == string.Empty)
            );
            m_delete_button_text = "Supprimer";
        }
        else {
            m_list_items = new ObservableCollection<ProductCategoryItem>(
                all_items.Where(item => item.product_category_archive != string.Empty)
            );
            m_delete_button_text = "Restaurer";
        }
    }


    private void navigateToSavePage(object? obj) {
        SPageNavigationController.navigateTo(new SaveProductCategoryPage());
    }


    private bool isItemSelected(object? arg) {
        return m_selected_item != null;
    }


    public void navigateToModifyPage(object? obj) {
        if (_m_selected_item != null) {
            SPageNavigationController.navigateTo(new SaveProductCategoryPage(_m_selected_item.product_category_id));
        }
    }


    private void deleteOrRestoreItem(object? obj) {
        if (_m_selected_item != null) {
            ResponseDeleteItem result;
            if (!_m_is_show_archived_checked) {
                result = _m_item_model.deleteItem(_m_selected_item.product_category_id.ToString());
            }
            else {
                result = _m_item_model.deleteItem(_m_selected_item.product_category_id.ToString(), EDeleteItemOperation.RESTORE);
            }
            
            if (!result.is_success) {
                MessageBox.Show("Une erreur s'est produite lors de la suppression.", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            updateListItems();
        }
    }
}
