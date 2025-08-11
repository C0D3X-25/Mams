using Mams.src.clients;
using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.databaseOperations;
using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Mams.src.entities;

/// <summary>
/// Controller for managing the list of clients page, handling client-related operations and UI interactions.
/// </summary>
public class ListEntityController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly EntityModel _m_item_model = new();

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


    private ObservableCollection<EntityItem>? _m_list_items;
    public ObservableCollection<EntityItem>? m_list_items {
        get { return _m_list_items; }
        set {
            _m_list_items = value;
            onPropertyChanged();
        }
    }


    private EntityItem? _m_selected_item;
    public EntityItem? m_selected_item {
        get { return _m_selected_item; }
        set {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the ListClientPageController
    /// </summary>
    /// <param name="page_navigation">The navigation controller for managing page transitions</param>
    public ListEntityController() {
        
        _m_delete_button_text = string.Empty;
        updateListItems();
        m_add_new_item_command = new RelayCommand(navigateToSavePage);
        m_modify_item_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_item_command = new RelayCommand(deleteOrRestoreItem, isItemSelected);
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
    }


    /// <summary>
    /// Updates the list of clients based on the archive status filter
    /// </summary>
    private void updateListItems() {
        var all_items = _m_item_model.getTable();

        if (!_m_is_show_archived_checked) {
            m_list_items = new ObservableCollection<EntityItem>(
                all_items.Where(item => item.entity_archive == string.Empty)
            );
            m_delete_button_text = "Supprimer";
        }
        else {
            m_list_items = new ObservableCollection<EntityItem>(
                all_items.Where(item => item.entity_archive != string.Empty)
            );
            m_delete_button_text = "Restaurer";
        }
    }


    /// <summary>
    /// Navigates to the save client page for creating a new client
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void navigateToSavePage(object? obj) {
        SPageNavigationController.navigateTo(new SaveEntityPage());
    }


    /// <summary>
    /// Determines if a client is currently selected
    /// </summary>
    /// <param name="arg">Command parameter (not used)</param>
    /// <returns>True if a client is selected, false otherwise</returns>
    private bool isItemSelected(object? arg) {
        return m_selected_item != null;
    }


    /// <summary>
    /// Navigates to the save client page for modifying an existing client
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void navigateToModifyPage(object? obj) {
        if (_m_selected_item != null) {
            SPageNavigationController.navigateTo(new SaveEntityPage(_m_selected_item.entity_id));
        }
    }


    /// <summary>
    /// Deletes or restores the selected client based on current archive status
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void deleteOrRestoreItem(object? obj) {
        if (_m_selected_item != null) {
            if (!_m_is_show_archived_checked) {
                _m_item_model.deleteItem(_m_selected_item.entity_id.ToString());
            }
            else {
                _m_item_model.deleteItem(_m_selected_item.entity_id.ToString(), EDeleteItemOperation.RESTORE);
            }
            updateListItems();
        }
    }
}
