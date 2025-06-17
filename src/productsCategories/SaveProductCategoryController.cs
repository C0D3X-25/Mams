using Mams.src.commands;
using Mams.src.navigations;
using System.Windows.Input;
using System.Windows;
using Mams.src.controllers;

namespace Mams.src.productsCategories;

public class SaveProductCategoryController : ABaseController {

    //
    private readonly ProductCategoryModel _m_product_category_model;

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductCategoryItem _m_product_category;
    public ProductCategoryItem m_product_category {
        get => _m_product_category;
        set {
            _m_product_category = value;
            onPropertyChanged();
        }
    }


    public SaveProductCategoryController(/*PageNavigationController page_navigation, */int id_to_load = 0) {
        //
        _m_product_category_model = new();
        _m_product_category = new ProductCategoryItem();

        if (id_to_load != 0) {
            _m_product_category = _m_product_category_model.getItemByID(id_to_load.ToString()) ?? new ProductCategoryItem();
            // Find the matching beehive in the list and set it as selected
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_category.product_category_name);
    }


    private void saveProduct(object? obj) {
        if (_m_product_category_model.saveItem(m_product_category) > 0) {
            SPageNavigationController.navigateTo(new ListProductCategoryPage());
        }
        else {
            MessageBox.Show("Un produit avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateTo(new ListProductCategoryPage());
    }
}
