using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.navigations;
using Mams_App.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.fees; 
public class ListFeeController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ReceiptFeeDetailedModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }
    public ICommand m_double_click_command { get; set; }



    private ObservableCollection<ReceiptFeeDetailedItem>? _m_list_items;
    public ObservableCollection<ReceiptFeeDetailedItem>? m_list_items {
        get { return _m_list_items; }
        set {
            _m_list_items = value;
            onPropertyChanged();
        }
    }


    private ReceiptFeeDetailedItem? _m_selected_item;
    public ReceiptFeeDetailedItem? m_selected_item {
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
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
    }


    private void updateListItems() {
        m_list_items = _m_item_model.getAllItems().returned_items;
    }


    private void navigateToSavePage(object? obj) {
        SPageNavigationController.navigateTo(new SaveFeePage());
    }


    private bool isItemSelected(object? arg) {
        return m_selected_item != null;
    }


    public void navigateToModifyPage(object? obj) {
        if (_m_selected_item != null) {
            SPageNavigationController.navigateTo(new SaveFeePage(_m_selected_item.receipt.receipt_id));
        }
    }


    private void deleteItem(object? obj) {
        if (_m_selected_item != null) {
            MessageBoxResult result = MessageBox.Show("Supprimer cette facture définitivement?",
                "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) {
                return;
            }
            var deleteResult = _m_item_model.deleteItem(_m_selected_item.receipt.receipt_id.ToString());
            if (!deleteResult.is_success) {
                MessageBox.Show("Une erreur s'est produite lors de la suppression.", 
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            updateListItems();
        }
    }
}

