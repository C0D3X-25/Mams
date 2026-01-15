using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.productsShapes;

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


    private bool canSaveProduct(object? arg)
    {
        return !string.IsNullOrEmpty(m_product_shape.product_shape_name);
    }


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


    private void abortProduct(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}
