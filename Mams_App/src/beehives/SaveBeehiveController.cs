using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.regions;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.beehives;

/// <summary>
/// Controller for managing the creation and modification of beehive items.
/// </summary>
public class SaveBeehiveController : ABaseController, ICompareState
{

    private readonly BeehiveModel _m_beehive_model = new();
    private readonly RegionModel _m_region_model = new();

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


    private ObservableCollection<RegionItem> _m_list_region = new();
    public ObservableCollection<RegionItem> m_list_region
    {
        get { return _m_list_region; }
        set
        {
            _m_list_region = value;
            onPropertyChanged();
        }
    }


    private RegionItem _m_selected_region = new();
    public RegionItem m_selected_region
    {
        get { return _m_selected_region; }
        set
        {
            _m_selected_region = value;
            m_beehive.fk_region_id = _m_selected_region.region_id;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the SaveBeehiveController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing beehive to modify. If 0, creates a new beehive.</param>
    public SaveBeehiveController(int id_to_load = 0)
    {

        _m_list_region = _m_region_model.getActiveRegions();

        if (id_to_load != 0)
        {
            _m_beehive = _m_beehive_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_original_beehive = _m_beehive_model.getItemByID(id_to_load.ToString()).returned_item ?? new();

            // Find the matching region in the list and set it as selected
            _m_selected_region = _m_list_region.FirstOrDefault(r =>
                r.region_id == _m_beehive.fk_region_id) ?? new();
        }

        m_save_command = new RelayCommand(saveBeehive, canSaveBeehive);
        m_abort_command = new RelayCommand(abortBeehive);
    }


    /// <summary>
    /// Determines whether the current state matches the original state.
    /// </summary>
    /// <returns>True if no changes have been made; otherwise, false.</returns>
    public bool isStateOriginal()
    {
        if (!_m_original_beehive.beehive_id.Equals(_m_beehive.beehive_id)
            || !_m_original_beehive.beehive_name.Equals(_m_beehive.beehive_name, StringComparison.Ordinal)
            || !_m_original_beehive.beehive_number.Equals(_m_beehive.beehive_number, StringComparison.Ordinal)
            || !_m_original_beehive.beehive_archive.Equals(_m_beehive.beehive_archive, StringComparison.Ordinal)
            || !_m_original_beehive.fk_region_id.Equals(_m_beehive.fk_region_id)
            )
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Determines whether the beehive can be saved.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if the beehive has a valid name; otherwise, false.</returns>
    private bool canSaveBeehive(object? arg)
    {
        return !string.IsNullOrEmpty(m_beehive.beehive_name);
    }


    /// <summary>
    /// Saves the current beehive to the database.
    /// If successful, navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
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


    /// <summary>
    /// Cancels the current operation and navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void abortBeehive(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}