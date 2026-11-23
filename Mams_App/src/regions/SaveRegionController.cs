using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.regions;

/// <summary>
/// Controller for managing the creation and modification of region items.
/// </summary>
public class SaveRegionController : ABaseController, ICompareState
{

    private readonly RegionModel _m_region_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private RegionItem _m_original_region = new();
    private RegionItem _m_region = new();
    public RegionItem m_region
    {
        get => _m_region;
        set
        {
            _m_region = value;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the SaveRegionController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing region to modify. If 0, creates a new item.</param>
    public SaveRegionController(int id_to_load = 0)
    {

        if (id_to_load != 0)
        {
            _m_original_region = _m_region_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_region = _m_region_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
        }

        m_save_command = new RelayCommand(saveRegion, canSaveRegion);
        m_abort_command = new RelayCommand(abortRegion);
    }

    /// <summary>
    /// Determines whether the current state matches the original state.
    /// </summary>
    /// <returns>True if no changes have been made; otherwise, false.</returns>
    public bool isStateOriginal()
    {
        if (!_m_original_region.region_id.Equals(_m_region.region_id)
            || !_m_original_region.region_name.Equals(_m_region.region_name, StringComparison.Ordinal)
            || !_m_original_region.region_archive.Equals(_m_region.region_archive, StringComparison.Ordinal)
            )
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Determines whether the region can be saved.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if the region has a valid name; otherwise, false.</returns>
    private bool canSaveRegion(object? arg)
    {
        return !string.IsNullOrEmpty(m_region.region_name);
    }


    /// <summary>
    /// Saves the current region to the database.
    /// If successful, navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void saveRegion(object? obj)
    {
        var result = _m_region_model.saveItem(m_region);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error, m_region.region_name);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    /// <summary>
    /// Cancels the current operation and navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void abortRegion(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}
