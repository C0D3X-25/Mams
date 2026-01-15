using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.beehives;

public class SaveBeehiveController : ABaseController, ICompareState
{

    private readonly BeehiveModel _m_beehive_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }



    private BeehiveItem _m_beehive = new();
    private BeehiveItem _m_original_beehive = new();
    public BeehiveItem m_beehive
    {
        get => _m_beehive;
        set
        {
            _m_beehive = value;
            onPropertyChanged();
        }
    }

    public SaveBeehiveController(int id_to_load = 0)
    {

        if (id_to_load != 0)
        {
            _m_beehive = _m_beehive_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_original_beehive = _m_beehive_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
        }

        m_save_command = new RelayCommand(saveBeehive, canSaveBeehive);
        m_abort_command = new RelayCommand(abortBeehive);
    }


    public bool isStateOriginal()
    {
        if (!_m_original_beehive.beehive_id.Equals(_m_beehive.beehive_id)
            || !_m_original_beehive.beehive_name.Equals(_m_beehive.beehive_name, StringComparison.Ordinal)
            || !_m_original_beehive.beehive_archive.Equals(_m_beehive.beehive_archive, StringComparison.Ordinal)
            )
        {
            return false;
        }
        return true;
    }


    private bool canSaveBeehive(object? arg)
    {
        return !string.IsNullOrEmpty(m_beehive.beehive_name);
    }


    private void saveBeehive(object? obj)
    {
        var result = _m_beehive_model.saveItem(m_beehive);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error, m_beehive.beehive_name);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortBeehive(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}