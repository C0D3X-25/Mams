using Mams.src.commands;
using Mams.src.navigations;
using System.Windows.Input;
using System.Windows;
using Mams.src.controllers;

namespace Mams.src.productsShapes;

public class SaveProductShapeController : ABaseController {

    
    private readonly ProductShapeModel _m_product_shape_model;

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductShapeItem _m_product_shape;
    public ProductShapeItem m_product_shape {
        get => _m_product_shape;
        set {
            _m_product_shape = value;
            onPropertyChanged();
        }
    }


    public SaveProductShapeController(int id_to_load = 0) {
        
        _m_product_shape_model = new();
        _m_product_shape = new ProductShapeItem();

        if (id_to_load != 0) {
            _m_product_shape = _m_product_shape_model.getItemByID(id_to_load.ToString()) ?? new ProductShapeItem();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_shape.product_shape_name);
    }


    private void saveProduct(object? obj) {
        if (_m_product_shape_model.saveItem(m_product_shape)) {
            SPageNavigationController.navigateTo(new ListProductShapePage());
        }
        else {
            MessageBox.Show("Une forme avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateTo(new ListProductShapePage());
    }
}
