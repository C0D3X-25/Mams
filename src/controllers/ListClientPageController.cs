using Mams.src.commands;
using Mams.src.enums;
using Mams.src.items;
using Mams.src.models;
using Mams.src.views.pages;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.controllers;

/// <summary>
/// Controller for managing the list of clients page, handling client-related operations and UI interactions.
/// </summary>
public class ListClientPageController : ABaseController {

    private readonly PageNavigationController _m_page_navigation;
    private readonly EntityModel _m_client_model = new();


    public ICommand m_add_new_client_command { get; set; }
    public ICommand m_modify_client_command { get; set; }
    public ICommand m_delete_client_command { get; set; }

    private bool _m_is_checkbox_show_archived_clicked = false;
    public bool m_is_checkbox_show_archived_clicked {
        get { return _m_is_checkbox_show_archived_clicked; }
        set {
            _m_is_checkbox_show_archived_clicked = value;
            onPropertyChanged();
            updateListClients();
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

    private ObservableCollection<EntityItem>? _m_clients;
    public ObservableCollection<EntityItem>? m_clients {
        get { return _m_clients; }
        set {
            _m_clients = value;
            onPropertyChanged();
        }
    }

    private EntityItem? _m_selected_client;
    public EntityItem? m_selected_client {
        get { return _m_selected_client; }
        set {
            _m_selected_client = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the ListClientPageController
    /// </summary>
    /// <param name="page_navigation">The navigation controller for managing page transitions</param>
    public ListClientPageController(PageNavigationController page_navigation) {
        _m_page_navigation = page_navigation;
        _m_delete_button_text = "";
        updateListClients();
        m_add_new_client_command = new RelayCommand(navigateToSaveClient);
        m_modify_client_command = new RelayCommand(navigateToModifyClient, isClientSelected);
        m_delete_client_command = new RelayCommand(deleteClient, isClientSelected);
    }

    /// <summary>
    /// Updates the list of clients based on the archive status filter
    /// </summary>
    private void updateListClients() {
        var all_clients = _m_client_model.getTable();

        if (!_m_is_checkbox_show_archived_clicked) {
            m_clients = new ObservableCollection<EntityItem>(
                all_clients.Where(client => client.entity_archive == String.Empty)
            );
            m_delete_button_text = "Supprimer";
        }
        else {
            m_clients = new ObservableCollection<EntityItem>(
                all_clients.Where(client => client.entity_archive != String.Empty)
            );
            m_delete_button_text = "Restaurer";
        }
    }

    /// <summary>
    /// Navigates to the save client page for creating a new client
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void navigateToSaveClient(object? obj) {
        _m_page_navigation.navigateTo(new SaveClientPage());
    }

    /// <summary>
    /// Determines if a client is currently selected
    /// </summary>
    /// <param name="arg">Command parameter (not used)</param>
    /// <returns>True if a client is selected, false otherwise</returns>
    private bool isClientSelected(object? arg) {
        return m_selected_client != null;
    }

    /// <summary>
    /// Navigates to the save client page for modifying an existing client
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void navigateToModifyClient(object? obj) {
        if (_m_selected_client != null) {
            _m_page_navigation.navigateTo(new SaveClientPage(_m_selected_client.entity_id));
        }
    }

    /// <summary>
    /// Deletes or restores the selected client based on current archive status
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void deleteClient(object? obj) {
        if (_m_selected_client != null) {
            if (_m_is_checkbox_show_archived_clicked) {
                _m_client_model.deleteItem(_m_selected_client.entity_id, EDatabaseDeleteItem.RESTORE);
            }
            else {
                _m_client_model.deleteItem(_m_selected_client.entity_id);
            }
            updateListClients();
        }
    }
}
