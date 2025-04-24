using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.views.pages;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.products;


public class SaveProductController : ABaseController {

    private readonly PageNavigationController _m_page_navigation;
    private readonly ProductModel _m_product_model;


    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductItem _m_product;
    public ProductItem m_product {
        get => _m_product;
        set {
            _m_product = value;
            onPropertyChanged();
        }
    }


    public SaveProductController(PageNavigationController page_navigation, int id_to_load = 0) {
        _m_page_navigation = page_navigation;
        _m_product_model = new();
        _m_product = new ProductItem();
        if (id_to_load != 0) {
            _m_product = _m_product_model.getItemByID(id_to_load.ToString()) ?? new ProductItem();
        }
        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product.product_name) 
            && m_product.fk_product_type_id != 0
            && m_product.fk_product_category_id != 0;
    }


    private void saveProduct(object? obj) {
        if (_m_product_model.saveItem(m_product)) {
            _m_page_navigation.navigateTo(new ListProductPage());
        }
        else { MessageBox.Show("Un produit avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); }
    }


    private void abortProduct(object? obj) {
        _m_page_navigation.navigateTo(new ListProductPage());
    }
}
