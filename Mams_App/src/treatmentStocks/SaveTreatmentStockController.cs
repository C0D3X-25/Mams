using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.doseUnits;
using Mams_App.src.entities;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.products;
using Mams_App.src.suppliers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.treatmentStocks;

/// <summary>
/// Controller for managing the creation and modification of treatment stock items.
/// </summary>
public class SaveTreatmentStockController : ABaseController, ICompareState
{
    private readonly TreatmentStockModel _m_treatment_stock_model = new();
    private readonly ProductModel _m_product_model = new();
    private readonly DoseUnitModel _m_dose_unit_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly SupplierModel _m_supplier_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }

    private TreatmentStockItem _m_treatment_stock = new();
    private TreatmentStockItem _m_original_treatment_stock = new();
    public TreatmentStockItem m_treatment_stock
    {
        get => _m_treatment_stock;
        set
        {
            _m_treatment_stock = value;
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
            m_treatment_stock.fk_product_id = _m_selected_product.product_id;
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
            m_treatment_stock.fk_dose_unit_id = _m_selected_dose_unit.dose_unit_id;
            onPropertyChanged();
        }
    }

    private ObservableCollection<EntityItem> _m_list_entity = new();
    public ObservableCollection<EntityItem> m_list_entity
    {
        get { return _m_list_entity; }
        set
        {
            _m_list_entity = value;
            onPropertyChanged();
        }
    }

    private EntityItem _m_selected_entity = new();
    public EntityItem m_selected_entity
    {
        get { return _m_selected_entity; }
        set
        {
            _m_selected_entity = value ?? new();
            // Resolve supplier_id from entity
            if (_m_selected_entity.entity_id > 0)
            {
                var supplier = _m_supplier_model.getSupplierWithEntityFK(_m_selected_entity.entity_id.ToString());
                m_treatment_stock.fk_supplier_id = supplier?.supplier_id ?? 0;
            }
            else
            {
                m_treatment_stock.fk_supplier_id = 0;
            }
            onPropertyChanged();
        }
    }

    public decimal m_treatment_stock_initial_quantity
    {
        get => _m_treatment_stock.treatment_stock_initial_quantity;
        set
        {
            _m_treatment_stock.treatment_stock_initial_quantity = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the SaveTreatmentStockController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing treatment stock to modify. If 0, creates a new one.</param>
    public SaveTreatmentStockController(int id_to_load = 0)
    {
        _m_list_product = _m_product_model.getActiveProductsByCategoryName(
            globals.SGlobals.g_TREATMENT_CATEGORY_NAME);
        _m_list_dose_unit = _m_dose_unit_model.getActiveDoseUnits();
        _m_list_entity = _m_entity_model.getActiveEntities();

        if (id_to_load != 0)
        {
            _m_treatment_stock = _m_treatment_stock_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_original_treatment_stock = _m_treatment_stock_model.getItemByID(id_to_load.ToString()).returned_item ?? new();

            _m_selected_product = _m_list_product.FirstOrDefault(p =>
                p.product_id == _m_treatment_stock.fk_product_id) ?? new();

            _m_selected_dose_unit = _m_list_dose_unit.FirstOrDefault(d =>
                d.dose_unit_id == _m_treatment_stock.fk_dose_unit_id) ?? new();

            // Resolve entity from supplier
            if (_m_treatment_stock.fk_supplier_id > 0)
            {
                var supplierResult = _m_supplier_model.getItemByID(_m_treatment_stock.fk_supplier_id.ToString());
                if (supplierResult.is_success && supplierResult.returned_item != null)
                {
                    _m_selected_entity = _m_list_entity.FirstOrDefault(e =>
                        e.entity_id == supplierResult.returned_item.fk_entity_id) ?? new();
                }
            }
        }
        else
        {
            _m_selected_dose_unit = _m_list_dose_unit.FirstOrDefault() ?? new();
            _m_treatment_stock.fk_dose_unit_id = _m_selected_dose_unit.dose_unit_id;
            _m_original_treatment_stock.fk_dose_unit_id = _m_selected_dose_unit.dose_unit_id;
        }

        m_save_command = new RelayCommand(saveStock, canSaveStock);
        m_abort_command = new RelayCommand(abortStock);
    }

    /// <summary>
    /// Determines whether the current state matches the original state.
    /// </summary>
    public bool isStateOriginal()
    {
        if (!_m_original_treatment_stock.treatment_stock_id.Equals(_m_treatment_stock.treatment_stock_id)
            || !_m_original_treatment_stock.treatment_stock_purchase_date.Equals(_m_treatment_stock.treatment_stock_purchase_date, StringComparison.Ordinal)
            || !_m_original_treatment_stock.treatment_stock_initial_quantity.Equals(_m_treatment_stock.treatment_stock_initial_quantity)
            || !_m_original_treatment_stock.fk_product_id.Equals(_m_treatment_stock.fk_product_id)
            || !_m_original_treatment_stock.fk_dose_unit_id.Equals(_m_treatment_stock.fk_dose_unit_id)
            || !_m_original_treatment_stock.fk_supplier_id.Equals(_m_treatment_stock.fk_supplier_id)
            )
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Determines whether the treatment stock can be saved.
    /// </summary>
    private bool canSaveStock(object? arg)
    {
        return !string.IsNullOrEmpty(m_treatment_stock.treatment_stock_purchase_date)
            && m_treatment_stock.fk_product_id > 0
            && m_treatment_stock.treatment_stock_initial_quantity > 0;
    }

    /// <summary>
    /// Saves the current treatment stock to the database.
    /// </summary>
    private void saveStock(object? obj)
    {
        var result = _m_treatment_stock_model.saveItem(m_treatment_stock);
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
    private void abortStock(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}
