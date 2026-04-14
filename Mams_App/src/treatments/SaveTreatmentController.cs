using Mams_App.src.beehives;
using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.treatmentStocks;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.treatments;

/// <summary>
/// Controller for managing the creation and modification of treatment items.
/// </summary>
public class SaveTreatmentController : ABaseController, ICompareState
{
    private readonly TreatmentModel _m_treatment_model = new();
    private readonly BeehiveModel _m_beehive_model = new();
    private readonly TreatmentStockModel _m_treatment_stock_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }

    private TreatmentItem _m_treatment = new();
    private TreatmentItem _m_original_treatment = new();
    public TreatmentItem m_treatment
    {
        get => _m_treatment;
        set
        {
            _m_treatment = value;
            onPropertyChanged();
        }
    }

    private ObservableCollection<BeehiveItem> _m_list_beehive = new();
    public ObservableCollection<BeehiveItem> m_list_beehive
    {
        get { return _m_list_beehive; }
        set
        {
            _m_list_beehive = value;
            onPropertyChanged();
        }
    }

    private BeehiveItem _m_selected_beehive = new();
    public BeehiveItem m_selected_beehive
    {
        get { return _m_selected_beehive; }
        set
        {
            _m_selected_beehive = value ?? new();
            m_treatment.fk_beehive_id = _m_selected_beehive.beehive_id;
            onPropertyChanged();
        }
    }

    private ObservableCollection<TreatmentStockItem> _m_list_treatment_stock = new();
    public ObservableCollection<TreatmentStockItem> m_list_treatment_stock
    {
        get { return _m_list_treatment_stock; }
        set
        {
            _m_list_treatment_stock = value;
            onPropertyChanged();
        }
    }

    private TreatmentStockItem _m_selected_treatment_stock = new();
    public TreatmentStockItem m_selected_treatment_stock
    {
        get { return _m_selected_treatment_stock; }
        set
        {
            _m_selected_treatment_stock = value ?? new();
            m_treatment.fk_treatment_stock_id = _m_selected_treatment_stock.treatment_stock_id;
            onPropertyChanged();
            onPropertyChanged(nameof(m_stock_remaining_ui));
        }
    }

    public int m_treatment_hive_count
    {
        get => _m_treatment.treatment_hive_count;
        set
        {
            _m_treatment.treatment_hive_count = value;
            onPropertyChanged();
            onPropertyChanged(nameof(m_treatment_dose_total_ui));
        }
    }

    public decimal m_treatment_dose_per_hive
    {
        get => _m_treatment.treatment_dose_per_hive;
        set
        {
            _m_treatment.treatment_dose_per_hive = value;
            onPropertyChanged();
            onPropertyChanged(nameof(m_treatment_dose_total_ui));
        }
    }

    public string m_treatment_dose_total_ui => $"{_m_treatment.treatment_dose_total:F2}";

    public string m_stock_remaining_ui =>
        _m_selected_treatment_stock.treatment_stock_id > 0
            ? $"{_m_selected_treatment_stock.treatment_stock_remaining_quantity:F2} {_m_selected_treatment_stock.dose_unit_name}"
            : string.Empty;

    /// <summary>
    /// Initializes a new instance of the SaveTreatmentController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing treatment to modify. If 0, creates a new treatment.</param>
    public SaveTreatmentController(int id_to_load = 0)
    {
        _m_list_beehive = _m_beehive_model.getActiveBeehives();
        _m_list_treatment_stock = _m_treatment_stock_model.getAvailableStocks();

        if (id_to_load != 0)
        {
            _m_treatment = _m_treatment_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_original_treatment = _m_treatment_model.getItemByID(id_to_load.ToString()).returned_item ?? new();

            _m_selected_beehive = _m_list_beehive.FirstOrDefault(b =>
                b.beehive_id == _m_treatment.fk_beehive_id) ?? new();

            // Ensure the current stock is in the list even if it's exhausted
            var currentStock = _m_list_treatment_stock.FirstOrDefault(s =>
                s.treatment_stock_id == _m_treatment.fk_treatment_stock_id);
            if (currentStock == null)
            {
                var stockResult = _m_treatment_stock_model.getItemByID(_m_treatment.fk_treatment_stock_id.ToString());
                if (stockResult.is_success && stockResult.returned_item != null)
                {
                    _m_list_treatment_stock.Insert(0, stockResult.returned_item);
                    currentStock = stockResult.returned_item;
                }
            }
            _m_selected_treatment_stock = currentStock ?? new();
        }

        m_save_command = new RelayCommand(saveTreatment, canSaveTreatment);
        m_abort_command = new RelayCommand(abortTreatment);
    }

    /// <summary>
    /// Determines whether the current state matches the original state.
    /// </summary>
    /// <returns>True if no changes have been made; otherwise, false.</returns>
    public bool isStateOriginal()
    {
        if (!_m_original_treatment.treatment_id.Equals(_m_treatment.treatment_id)
            || !_m_original_treatment.treatment_date.Equals(_m_treatment.treatment_date, StringComparison.Ordinal)
            || !_m_original_treatment.treatment_hive_count.Equals(_m_treatment.treatment_hive_count)
            || !_m_original_treatment.treatment_dose_per_hive.Equals(_m_treatment.treatment_dose_per_hive)
            || !_m_original_treatment.fk_beehive_id.Equals(_m_treatment.fk_beehive_id)
            || !_m_original_treatment.fk_treatment_stock_id.Equals(_m_treatment.fk_treatment_stock_id)
            )
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Determines whether the treatment can be saved.
    /// </summary>
    private bool canSaveTreatment(object? arg)
    {
        return !string.IsNullOrEmpty(m_treatment.treatment_date)
            && m_treatment.fk_beehive_id > 0
            && m_treatment.fk_treatment_stock_id > 0
            && m_treatment.treatment_hive_count > 0
            && m_treatment.treatment_dose_per_hive > 0;
    }

    /// <summary>
    /// Saves the current treatment to the database.
    /// </summary>
    private void saveTreatment(object? obj)
    {
        var result = _m_treatment_model.saveItem(m_treatment);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error, string.Empty);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Cancels the current operation and navigates back.
    /// </summary>
    private void abortTreatment(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}
