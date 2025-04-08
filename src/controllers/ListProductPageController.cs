using Mams.src.commands;
using Mams.src.enums;
using Mams.src.items;
using Mams.src.models;
using Mams.src.views.pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mams.src.controllers;

internal class ListProductPageController : ABaseController {

    private readonly PageNavigationController _m_page_navigation;
    private readonly ProductModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }


    private bool _m_is_checkbox_show_archived_clicked = false;
    public bool m_is_checkbox_show_archived_clicked {
        get { return _m_is_checkbox_show_archived_clicked; }
        set {
            _m_is_checkbox_show_archived_clicked = value;
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


    public ListProductPageController(PageNavigationController page_navigation) {
        _m_page_navigation = page_navigation;
        _m_delete_button_text = "";
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteRestoreItem, isItemSelected);
    }


    private void updateListItems() {
        var all_items = _m_item_model.getTable();

        if (!_m_is_checkbox_show_archived_clicked) {
            m_list_items = new ObservableCollection<ProductItem>(
                all_items.Where(item => item.product_archive == String.Empty)
            );
            m_delete_button_text = "Supprimer";
        }
        else {
            m_list_items = new ObservableCollection<ProductItem>(
                all_items.Where(item => item.product_archive != String.Empty)
            );
            m_delete_button_text = "Restaurer";
        }
    }


    private void navigateToSavePage(object? obj) {
        _m_page_navigation.navigateTo(new SaveProductPage());
    }


    private bool isItemSelected(object? arg) {
        return m_selected_item != null;
    }


    private void navigateToModifyPage(object? obj) {
        if (_m_selected_item != null) {
            _m_page_navigation.navigateTo(new SaveClientPage(_m_selected_item.product_id));
        }
    }


    private void deleteRestoreItem(object? obj) {
        if (_m_selected_item != null) {
            if (_m_is_checkbox_show_archived_clicked) {
                _m_item_model.deleteItem(_m_selected_item.product_id, EDatabaseDeleteItem.RESTORE);
            }
            else {
                _m_item_model.deleteItem(_m_selected_item.product_id);
            }
            updateListItems();
        }
    }
}

