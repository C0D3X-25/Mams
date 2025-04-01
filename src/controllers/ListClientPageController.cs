using Mams.src.commands;
using Mams.src.items;
using Mams.src.models;
using Mams.src.views.pages;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.controllers;

public class ListClientPageController : ABaseController {

    private readonly PageNavigationController _m_page_navigation;
    private readonly EntityModel _m_client_model = new();

    public ICommand m_add_new_client_command { get; set; }
    public ICommand m_modify_client_command { get; set; }
    public ICommand m_delete_client_command { get; set; }


    private bool _m_are_clients_archived = false;
    public bool m_are_clients_archived {
        get { return _m_are_clients_archived; }
        set { 
            _m_are_clients_archived = value;
            onPropertyChanged();
            updateClients();
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


    public ListClientPageController(PageNavigationController page_navigation) {
        _m_page_navigation = page_navigation;
        updateClients();
        m_add_new_client_command = new RelayCommand(navigateToSaveClient);
        m_modify_client_command = new RelayCommand(navigateToModifyClient, isClientSelected);
        m_delete_client_command = new RelayCommand(deleteClient, isClientSelected);
    }

    private void updateClients() {

        var all_clients = _m_client_model.getTable();

        if (!_m_are_clients_archived) {
            m_clients = new ObservableCollection<EntityItem>(
                all_clients.Where(c => c.entity_archive == String.Empty)
            );
        }
        else {
            m_clients = new ObservableCollection<EntityItem>(
                all_clients.Where(c => c.entity_archive != String.Empty)
            );
        }
    }


    private void navigateToSaveClient(object? obj) {
        _m_page_navigation.navigateTo(new SaveClientPage());
    }


    private bool isClientSelected(object? arg) {
        return m_selected_client != null;
    }


    private void navigateToModifyClient(object? obj) {
        if (_m_selected_client != null) {
            _m_page_navigation.navigateTo(new SaveClientPage(_m_selected_client.entity_id));
        }
    }    
    

    private void deleteClient(object? obj) {
        if (_m_selected_client != null) {
            _m_client_model.deleteItem(_m_selected_client.entity_id);
            updateClients();
        }
    }
}
