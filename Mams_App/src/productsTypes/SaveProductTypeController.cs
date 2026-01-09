using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.productsTypes;

public class SaveProductTypeController : ABaseController, ICompareState
{

    private readonly ProductTypeModel _m_product_type_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductTypeItem _m_original_product_type = new();
    private ProductTypeItem _m_product_type = new();
    public ProductTypeItem m_product_type
    {
        get => _m_product_type;
        set
        {
            _m_product_type = value;
            onPropertyChanged();
        }
    }


    public SaveProductTypeController(int id_to_load = 0)
    {

        if (id_to_load != 0)
        {
            _m_original_product_type = _m_product_type_model.getItemByID(id_to_load.ToString()).returned_item ?? new ProductTypeItem();
            _m_product_type = _m_product_type_model.getItemByID(id_to_load.ToString()).returned_item ?? new ProductTypeItem();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }

    public bool isStateOriginal()
    {
        if (!_m_original_product_type.product_type_id.Equals(_m_product_type.product_type_id)
            || !_m_original_product_type.product_type_name.Equals(_m_product_type.product_type_name, StringComparison.Ordinal)
            || !_m_original_product_type.product_type_archive.Equals(_m_product_type.product_type_archive, StringComparison.Ordinal))
        {
            return false;
        }
        return true;
    }


    private bool canSaveProduct(object? arg)
    {
        return !string.IsNullOrEmpty(m_product_type.product_type_name);
    }


    private void saveProduct(object? obj)
    {
        var result = _m_product_type_model.saveItem(m_product_type);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error, m_product_type.product_type_name);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }
}
