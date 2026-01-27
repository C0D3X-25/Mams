using Mams_App.src.beehives;
using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.productsLots;

/// <summary>
/// Controller for managing the creation and modification of product lot items.
/// </summary>
public class SaveProductLotController : ABaseController, ICompareState
{

    private readonly ProductLotModel _m_product_lot_model = new();
    private readonly BeehiveModel _m_beehive_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductLotItem _m_original_product_lot = new();
    private ProductLotItem _m_product_lot = new();
    public ProductLotItem m_product_lot
    {
        get => _m_product_lot;
        set
        {
            _m_product_lot = value;
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
            _m_selected_beehive = value;
            m_product_lot.fk_beehive_id = _m_selected_beehive.beehive_id;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the SaveProductLotController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing product lot to modify. If 0, creates a new item.</param>
    public SaveProductLotController(int id_to_load = 0)
    {

        _m_list_beehive = _m_beehive_model.getAllItems().returned_items;

        if (id_to_load != 0)
        {
            _m_original_product_lot = _m_product_lot_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_product_lot = _m_product_lot_model.getItemByID(id_to_load.ToString()).returned_item ?? new();

            // Find the matching beehive in the list and set it as selected
            _m_selected_beehive = _m_list_beehive.FirstOrDefault(b =>
                b.beehive_id == _m_product_lot.fk_beehive_id) ?? new();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }

    /// <summary>
    /// Determines whether the current state matches the original state.
    /// </summary>
    /// <returns>True if no changes have been made; otherwise, false.</returns>
    public bool isStateOriginal()
    {
        if (!_m_original_product_lot.product_lot_id.Equals(_m_product_lot.product_lot_id)
            || !_m_original_product_lot.product_lot_name.Equals(_m_product_lot.product_lot_name, StringComparison.Ordinal)
            || !_m_original_product_lot.product_lot_year.Equals(_m_product_lot.product_lot_year)
            || !_m_original_product_lot.fk_beehive_id.Equals(_m_product_lot.fk_beehive_id)
            || !_m_original_product_lot.beehive_name.Equals(_m_product_lot.beehive_name, StringComparison.Ordinal)
            )
        {
            return false;
        }
        return true;

    }

    /// <summary>
    /// Determines whether the product lot can be saved.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if the product lot has valid data; otherwise, false.</returns>
    private bool canSaveProduct(object? arg)
    {
        return !string.IsNullOrEmpty(m_product_lot.product_lot_name)
            && SDataValidation.isYearInRange(m_product_lot.product_lot_year);
    }


    /// <summary>
    /// Saves the current product lot to the database.
    /// If successful, navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void saveProduct(object? obj)
    {
        if (m_selected_beehive != null)
        {
            m_product_lot.fk_beehive_id = m_selected_beehive.beehive_id;
            m_product_lot.beehive_name = m_selected_beehive.beehive_name;
        }

        var result = _m_product_lot_model.saveItem(m_product_lot);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error, m_product_lot.product_lot_name);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    /// <summary>
    /// Cancels the current operation and navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void abortProduct(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}