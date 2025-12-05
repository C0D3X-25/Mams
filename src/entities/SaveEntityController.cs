using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.entities;

/// <summary>
/// Controller for managing the creation and modification of client entities
/// </summary>
public class SaveEntityController : ABaseController, ICompareState {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly EntityModel _m_entity_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private EntityItem _m_original_entity = new();
    private EntityItem _m_entity = new();
    public EntityItem m_entity {
        get => _m_entity;
        set {
            _m_entity = value;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the SaveClientController
    /// </summary>
    /// <param name="page_navigation">The navigation controller for managing page transitions</param>
    /// <param name="id_to_load">Optional ID of an existing client to modify. If 0, creates a new client</param>
    public SaveEntityController(int id_to_load = 0) {
        
        if (id_to_load != 0) {
            _m_entity = _m_entity_model.getItemByID(id_to_load.ToString()) ?? new EntityItem();
            _m_original_entity = _m_entity_model.getItemByID(id_to_load.ToString()) ?? new EntityItem();
        }

        m_save_command = new RelayCommand(saveClient, canSaveClient);
        m_abort_command = new RelayCommand(abortClient);
    }

    public bool isStateOriginal() {

        if (!_m_original_entity.entity_id.Equals(_m_entity.entity_id)
            || !_m_original_entity.entity_name.Equals(_m_entity.entity_name, StringComparison.Ordinal)
            || !_m_original_entity.entity_archive.Equals(_m_entity.entity_archive, StringComparison.Ordinal)
            || !_m_original_entity.entity_phone.Equals(_m_entity.entity_phone, StringComparison.Ordinal)
            || !_m_original_entity.entity_email.Equals(_m_entity.entity_email, StringComparison.Ordinal)
            || !_m_original_entity.entity_city.Equals(_m_entity.entity_city, StringComparison.Ordinal)
            || !_m_original_entity.entity_address.Equals(_m_entity.entity_address, StringComparison.Ordinal)
            ) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Determines if the current client can be saved
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    /// <returns>True if the client has a name, false otherwise</returns>
    private bool canSaveClient(object? obj) {
        return !string.IsNullOrEmpty(m_entity.entity_name);
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
        var result = _m_entity_model.saveItem(m_entity);
        if (result.is_success) {
            SPageNavigationController.navigateBack();
        }
        else { 
            MessageBox.Show(result.error_message ?? $"Un contact avec le même nom ({_m_entity.entity_name}) est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); 
        }
    }


    /// <summary>
    /// Cancels the current operation and returns to the client list page
    /// </summary>
    /// <param name="obj">Command parameter (not used)</param>
    private void abortClient(object? obj) {
        SPageNavigationController.navigateBack(true);
    }
}
