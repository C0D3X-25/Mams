using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.doseUnits;

/// <summary>
/// Controller for managing the creation and modification of dose unit items.
/// </summary>
public class SaveDoseUnitController : ABaseController, ICompareState
{

    private readonly DoseUnitModel _m_dose_unit_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private DoseUnitItem _m_original_dose_unit = new();
    private DoseUnitItem _m_dose_unit = new();
    public DoseUnitItem m_dose_unit
    {
        get => _m_dose_unit;
        set
        {
            _m_dose_unit = value;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the SaveDoseUnitController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing dose unit to modify. If 0, creates a new item.</param>
    public SaveDoseUnitController(int id_to_load = 0)
    {

        if (id_to_load != 0)
        {
            _m_original_dose_unit = _m_dose_unit_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_dose_unit = _m_dose_unit_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
        }

        m_save_command = new RelayCommand(saveDoseUnit, canSaveDoseUnit);
        m_abort_command = new RelayCommand(abortDoseUnit);
    }

    /// <summary>
    /// Determines whether the current state matches the original state.
    /// </summary>
    /// <returns>True if no changes have been made; otherwise, false.</returns>
    public bool isStateOriginal()
    {
        if (!_m_original_dose_unit.dose_unit_id.Equals(_m_dose_unit.dose_unit_id)
            || !_m_original_dose_unit.dose_unit_name.Equals(_m_dose_unit.dose_unit_name, StringComparison.Ordinal)
            || !_m_original_dose_unit.dose_unit_archive.Equals(_m_dose_unit.dose_unit_archive, StringComparison.Ordinal)
            )
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Determines whether the dose unit can be saved.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if the dose unit has a valid name; otherwise, false.</returns>
    private bool canSaveDoseUnit(object? arg)
    {
        return !string.IsNullOrEmpty(m_dose_unit.dose_unit_name);
    }


    /// <summary>
    /// Saves the current dose unit to the database.
    /// If successful, navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void saveDoseUnit(object? obj)
    {
        var result = _m_dose_unit_model.saveItem(m_dose_unit);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error, m_dose_unit.dose_unit_name);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    /// <summary>
    /// Cancels the current operation and navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void abortDoseUnit(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}
