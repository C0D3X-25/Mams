using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.productsCategories;

public class SaveProductCategoryController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


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


    public SaveProductCategoryController(int id_to_load = 0) {
        _m_product_category_model = new();
        _m_product_category = new ProductCategoryItem();

        if (id_to_load != 0) {
            _m_product_category = _m_product_category_model.getItemByID(id_to_load.ToString()) ?? new ProductCategoryItem();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_category.product_category_name);
    }


    private void saveProduct(object? obj) {
        if (_m_product_category_model.saveItem(m_product_category) > 0) {
            SPageNavigationController.navigateBack();
        }
        else {
            MessageBox.Show("Une catégorie de produit avec le même nom est déjà présente", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateBack();
    }
}
