using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using System.Windows.Input;
using System.Windows;

namespace Mams.src.beehives;

public class SaveBeehiveController : ABaseController {

    
    private readonly BeehiveModel _m_beehive_model;

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private BeehiveItem _m_beehive;
    public BeehiveItem m_beehive {
        get => _m_beehive;
        set {
            _m_beehive = value;
            onPropertyChanged();
        }
    }


    public SaveBeehiveController(int id_to_load = 0) {

        
        _m_beehive_model = new();
        _m_beehive = new BeehiveItem();

        if (id_to_load != 0) {
            _m_beehive = _m_beehive_model.getItemByID(id_to_load.ToString()) ?? new BeehiveItem();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_beehive.beehive_name);
    }


    private void saveProduct(object? obj) {
        if (_m_beehive_model.saveItem(m_beehive) > 0) {
            SPageNavigationController.navigateTo(new ListBeehivePage());
        }
        else {
            MessageBox.Show("Un ruchcer avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateTo(new ListBeehivePage());
    }
}