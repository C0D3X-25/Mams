using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.productsShapes;

/// <summary>
/// Controller for managing the creation and modification of product shape items.
/// </summary>
public class SaveProductShapeController : ABaseController, ICompareState
{

    private readonly ProductShapeModel _m_product_shape_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductShapeItem _m_original_product_shape = new();
    private ProductShapeItem _m_product_shape = new();
    public ProductShapeItem m_product_shape
    {
        get => _m_product_shape;
        set
        {
            _m_product_shape = value;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the SaveProductShapeController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing product shape to modify. If 0, creates a new item.</param>
    public SaveProductShapeController(int id_to_load = 0)
    {

        if (id_to_load != 0)
        {
            _m_original_product_shape = _m_product_shape_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_product_shape = _m_product_shape_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
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
        if (!_m_original_product_shape.product_shape_id.Equals(_m_product_shape.product_shape_id)
            || !_m_original_product_shape.product_shape_name.Equals(_m_product_shape.product_shape_name, StringComparison.Ordinal)
            || !_m_original_product_shape.product_shape_archive.Equals(_m_product_shape.product_shape_archive, StringComparison.Ordinal)
            )
        {
            return false;
        }
        return true;
    }


    /// <summary>
    /// Determines whether the product shape can be saved.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if the product shape has a valid name; otherwise, false.</returns>
    private bool canSaveProduct(object? arg)
    {
        return !string.IsNullOrEmpty(m_product_shape.product_shape_name);
    }


    /// <summary>
    /// Saves the current product shape to the database.
    /// If successful, navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void saveProduct(object? obj)
    {
        var result = _m_product_shape_model.saveItem(m_product_shape);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error, m_product_shape.product_shape_name);
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
