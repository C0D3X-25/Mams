using Mams_App.src.beehives;
using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.doseUnits;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.products;
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
    private readonly ProductModel _m_product_model = new();
    private readonly DoseUnitModel _m_dose_unit_model = new();

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

    private ObservableCollection<ProductItem> _m_list_product = new();
    public ObservableCollection<ProductItem> m_list_product
    {
        get { return _m_list_product; }
        set
        {
            _m_list_product = value;
            onPropertyChanged();
        }
    }

    private ProductItem _m_selected_product = new();
    public ProductItem m_selected_product
    {
        get { return _m_selected_product; }
        set
        {
            _m_selected_product = value ?? new();
            m_treatment.fk_product_id = _m_selected_product.product_id;
            onPropertyChanged();
        }
    }

    private ObservableCollection<DoseUnitItem> _m_list_dose_unit = new();
    public ObservableCollection<DoseUnitItem> m_list_dose_unit
    {
        get { return _m_list_dose_unit; }
        set
        {
            _m_list_dose_unit = value;
            onPropertyChanged();
        }
    }

    private DoseUnitItem _m_selected_dose_unit = new();
    public DoseUnitItem m_selected_dose_unit
    {
        get { return _m_selected_dose_unit; }
        set
        {
            _m_selected_dose_unit = value ?? new();
            m_treatment.fk_dose_unit_id = _m_selected_dose_unit.dose_unit_id;
            onPropertyChanged();
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

    /// <summary>
    /// Initializes a new instance of the SaveTreatmentController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing treatment to modify. If 0, creates a new treatment.</param>
    public SaveTreatmentController(int id_to_load = 0)
    {
        _m_list_beehive = _m_beehive_model.getActiveBeehives();
        _m_list_product = _m_product_model.getActiveProducts();
        _m_list_dose_unit = _m_dose_unit_model.getActiveDoseUnits();

        if (id_to_load != 0)
        {
            _m_treatment = _m_treatment_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_original_treatment = _m_treatment_model.getItemByID(id_to_load.ToString()).returned_item ?? new();

            _m_selected_beehive = _m_list_beehive.FirstOrDefault(b =>
                b.beehive_id == _m_treatment.fk_beehive_id) ?? new();

            _m_selected_product = _m_list_product.FirstOrDefault(p =>
                p.product_id == _m_treatment.fk_product_id) ?? new();

            _m_selected_dose_unit = _m_list_dose_unit.FirstOrDefault(d =>
                d.dose_unit_id == _m_treatment.fk_dose_unit_id) ?? new();
        }
        else
        {
            // Default to first dose unit (ml) for new treatments
            _m_selected_dose_unit = _m_list_dose_unit.FirstOrDefault() ?? new();
            _m_treatment.fk_dose_unit_id = _m_selected_dose_unit.dose_unit_id;
            _m_original_treatment.fk_dose_unit_id = _m_selected_dose_unit.dose_unit_id;
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
            || !_m_original_treatment.fk_product_id.Equals(_m_treatment.fk_product_id)
            || !_m_original_treatment.fk_dose_unit_id.Equals(_m_treatment.fk_dose_unit_id)
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
            && m_treatment.fk_product_id > 0
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
