using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using System.Windows.Input;
using System.Windows;

namespace Mams.src.productsTypes;

public class SaveProductTypeController : ABaseController {

    private readonly PageNavigationController _m_page_navigation;
    private readonly ProductTypeModel _m_product_type_model;

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductTypeItem _m_product_type;
    public ProductTypeItem m_product_type {
        get => _m_product_type;
        set {
            _m_product_type = value;
            onPropertyChanged();
        }
    }


    public SaveProductTypeController(PageNavigationController page_navigation, int id_to_load = 0) {
        _m_page_navigation = page_navigation;
        _m_product_type_model = new();
        _m_product_type = new ProductTypeItem();

        if (id_to_load != 0) {
            _m_product_type = _m_product_type_model.getItemByID(id_to_load.ToString()) ?? new ProductTypeItem();
            // Find the matching beehive in the list and set it as selected
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_type.product_type_name);
    }


    private void saveProduct(object? obj) {
        if (_m_product_type_model.saveItem(m_product_type)) {
            _m_page_navigation.navigateTo(new ListProductTypePage());
        }
        else {
            MessageBox.Show("Un produit avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        _m_page_navigation.navigateTo(new ListProductTypePage());
    }
}
