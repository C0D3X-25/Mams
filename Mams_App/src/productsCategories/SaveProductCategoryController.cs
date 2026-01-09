using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.errors;
using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.productsCategories;

public class SaveProductCategoryController : ABaseController, ICompareState
{

    private readonly ProductCategoryModel _m_product_category_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductCategoryItem _m_original_product_category = new();
    private ProductCategoryItem _m_product_category = new();
    public ProductCategoryItem m_product_category
    {
        get => _m_product_category;
        set
        {
            _m_product_category = value;
            onPropertyChanged();
        }
    }


    public SaveProductCategoryController(int id_to_load = 0)
    {

        if (id_to_load != 0)
        {
            _m_original_product_category = _m_product_category_model.getItemByID(id_to_load.ToString()).returned_item ?? new ProductCategoryItem();
            _m_product_category = _m_product_category_model.getItemByID(id_to_load.ToString()).returned_item ?? new ProductCategoryItem();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }

    public bool isStateOriginal()
    {
        if (!_m_original_product_category.product_category_id.Equals(_m_product_category.product_category_id)
            || !_m_original_product_category.product_category_name.Equals(_m_product_category.product_category_name, StringComparison.Ordinal)
            || !_m_original_product_category.product_category_archive.Equals(_m_product_category.product_category_archive, StringComparison.Ordinal))
        {
            return false;
        }
        return true;
    }

    private bool canSaveProduct(object? arg)
    {
        return !string.IsNullOrEmpty(m_product_category.product_category_name);
    }


    private void saveProduct(object? obj)
    {
        var result = _m_product_category_model.saveItem(m_product_category);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error, m_product_category.product_category_name);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }

}
