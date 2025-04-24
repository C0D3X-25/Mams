using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.products;
using System.Windows.Input;
using System.Windows;

namespace Mams.src.productsLots;

public class SaveProductLotController : ABaseController {

    private readonly PageNavigationController _m_page_navigation;
    private readonly ProductLotModel _m_product_lot_model;


    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductLotItem _m_product_lot;
    public ProductLotItem m_product_lot {
        get => _m_product_lot;
        set {
            _m_product_lot = value;
            onPropertyChanged();
        }
    }


    public SaveProductLotController(PageNavigationController page_navigation, int id_to_load = 0) {
        _m_page_navigation = page_navigation;
        _m_product_lot_model = new();
        _m_product_lot = new ProductLotItem();
        if (id_to_load != 0) {
            _m_product_lot = _m_product_lot_model.getItemByID(id_to_load.ToString()) ?? new ProductLotItem();
        }
        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_lot.product_lot_name)
            && m_product_lot.product_lot_year != 0;
    }


    private void saveProduct(object? obj) {
        if (_m_product_lot_model.saveItem(m_product_lot)) {
            _m_page_navigation.navigateTo(new ListProductLotPage());
        }
        else { MessageBox.Show("Un lot avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error); }
    }


    private void abortProduct(object? obj) {
        _m_page_navigation.navigateTo(new ListProductLotPage());
    }
}