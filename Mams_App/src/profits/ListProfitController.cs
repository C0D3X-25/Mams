using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.navigations;
using Mams_App.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.profits;

public class ListProfitController : ABaseController
{


    public SolidColorBrush m_delete_button_color { get; } = SGlobalView.DELETE_BUTTON_COLOR;
    public SolidColorBrush m_delete_button_text_color { get; } = SGlobalView.DELETE_BUTTON_TEXT_COLOR;


    private readonly ReceiptProfitDetailedModel _m_item_model = new();

    public ICommand m_add_new_item_command { get; set; }
    public ICommand m_modify_item_command { get; set; }
    public ICommand m_delete_item_command { get; set; }
    public ICommand m_double_click_command { get; set; }



    private ObservableCollection<ReceiptProfitDetailedItem>? _m_list_items;
    public ObservableCollection<ReceiptProfitDetailedItem>? m_list_items
    {
        get { return _m_list_items; }
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }


    private ReceiptProfitDetailedItem? _m_selected_item;
    public ReceiptProfitDetailedItem? m_selected_item
    {
        get { return _m_selected_item; }
        set
        {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }


    public ListProfitController()
    {
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteItem, isItemSelected);
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
    }

    public void navigateToModifyPage(object? obj)
    {
        if (_m_selected_item != null)
        {
            SPageNavigationController.navigateTo(new SaveProfitPage(_m_selected_item.receipt.receipt_id));
        }
    }

    private void updateListItems()
    {
        m_list_items = _m_item_model.getAllItems().returned_items;
    }


    private void navigateToSavePage(object? obj)
    {
        SPageNavigationController.navigateTo(new SaveProfitPage());
    }


    private bool isItemSelected(object? arg)
    {
        return m_selected_item != null;
    }


    private void deleteItem(object? obj)
    {
        if (_m_selected_item != null)
        {
            MessageBoxResult result = MessageBox.Show("Supprimer cette facture définitivement?",
                "Suppression", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                return;
            }
            var deleteResult = _m_item_model.deleteItem(_m_selected_item.receipt.receipt_id.ToString());
            if (!deleteResult.is_success)
            {
                MessageBox.Show("Une erreur s'est produite lors de la suppression.",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            updateListItems();
        }
    }
}


