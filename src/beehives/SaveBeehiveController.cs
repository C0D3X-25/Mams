using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.beehives;

public class SaveBeehiveController : ABaseController, ICompareState {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly BeehiveModel _m_beehive_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }



    private BeehiveItem _m_beehive = new();
    private BeehiveItem _m_original_beehive = new();
    public BeehiveItem m_beehive {
        get => _m_beehive;
        set {
            _m_beehive = value;
            onPropertyChanged();
        }
    }

    
    public SaveBeehiveController(int id_to_load = 0) {

        if (id_to_load != 0) {
            _m_beehive = _m_beehive_model.getItemByID(id_to_load.ToString()) ?? new();
            _m_original_beehive = _m_beehive_model.getItemByID(id_to_load.ToString()) ?? new();
        }

        m_save_command = new RelayCommand(saveBeehive, canSaveBeehive);
        m_abort_command = new RelayCommand(abortBeehive);
    }


    public bool isStateOriginal() {

        if (_m_original_beehive.beehive_id.Equals(_m_beehive.beehive_id)
            || !_m_original_beehive.beehive_name.Equals(_m_beehive.beehive_name, StringComparison.Ordinal)
            || !_m_original_beehive.beehive_archive.Equals(_m_beehive.beehive_archive, StringComparison.Ordinal)
            ) {
            return false;
        }
        return true;
    }


    private bool canSaveBeehive(object? arg) {
        return !string.IsNullOrEmpty(m_beehive.beehive_name);
    }


    private void saveBeehive(object? obj) {
        if (_m_beehive_model.saveItem(m_beehive) > 0) {
            SPageNavigationController.navigateBack();
        }
        else {
            MessageBox.Show("Un rucher avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortBeehive(object? obj) {
        SPageNavigationController.navigateBack(true);
    }
}