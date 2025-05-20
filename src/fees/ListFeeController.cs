using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.databaseOperations;
using Mams.src.navigations;
using Mams.src.products;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mams.src.fees; 
public class ListFeeController : ABaseController {

    private readonly FeeModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }



    private ObservableCollection<FeeItem>? _m_list_items;
    public ObservableCollection<FeeItem>? m_list_items {
        get { return _m_list_items; }
        set {
            _m_list_items = value;
            onPropertyChanged();
        }
    }


    private FeeItem? _m_selected_item;
    public FeeItem? m_selected_item {
        get { return _m_selected_item; }
        set {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }


    public ListFeeController() {

        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteItem, isItemSelected);
    }


    private void updateListItems() {
        var all_items = _m_item_model.getTable();
    }


    private void navigateToSavePage(object? obj) {
        SPageNavigationController.navigateTo(new SaveFeePage());
    }


    private bool isItemSelected(object? arg) {
        return m_selected_item != null;
    }


    private void navigateToModifyPage(object? obj) {
        if (_m_selected_item != null) {
            SPageNavigationController.navigateTo(new SaveFeePage(_m_selected_item.product_id));
        }
    }


    private void deleteItem(object? obj) {
        if (_m_selected_item != null) {
            _m_item_model.deleteItem(_m_selected_item.product_id);
            updateListItems();
        }
    }
}

