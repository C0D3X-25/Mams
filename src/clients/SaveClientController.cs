using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.entities;
using Mams.src.navigations;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.clients;

/// <summary>
/// Controller for managing the creation and modification of client entities
/// </summary>
public class SaveClientController : ABaseController {

    
    private readonly EntityModel _m_entity_model;

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private EntityItem _m_client;
    public EntityItem m_client {
        get => _m_client;
        set {
            _m_client = value;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the SaveClientController
    /// </summary>
    /// <param name="page_navigation">The navigation controller for managing page transitions</param>
    /// <param name="id_to_load">Optional ID of an existing client to modify. If 0, creates a new client</param>
    public SaveClientController(int id_to_load = 0) {
        
        _m_entity_model = new();
        _m_client = new EntityItem();
        if (id_to_load != 0) {
            _m_client = _m_entity_model.getItemByID(id_to_load.ToString()) ?? new EntityItem();
        }
        m_save_command = new RelayCommand(saveClient, canSaveClient);
        m_abort_command = new RelayCommand(abortClient);
    }


    /// <summary>
    /// Determines if the current client can be saved
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    /// <returns>True if the client has a name, false otherwise</returns>
    private bool canSaveClient(object? obj) {
        return !string.IsNullOrEmpty(m_client.entity_name);
    }


    /// <summary>
    /// Saves the current client to the database
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    /// <remarks>
    /// If successful, navigates back to the client list page.
    /// If a client with the same name exists, shows an error message.
    /// </remarks>
    private void saveClient(object? obj) {
        if (_m_entity_model.saveItem(m_client)) {
            SPageNavigationController.navigateTo(new ListClientPage());
        }
        else { MessageBox.Show("Un client avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); }
    }


    /// <summary>
    /// Cancels the current operation and returns to the client list page
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void abortClient(object? obj) {
        SPageNavigationController.navigateTo(new ListClientPage());
    }
}
