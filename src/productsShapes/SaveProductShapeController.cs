using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.productsShapes;

public class SaveProductShapeController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


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
        if (_m_product_shape_model.saveItem(m_product_shape) > 0) {
            SPageNavigationController.navigateBack();
        }
        else {
            MessageBox.Show("Une forme avec le même nom est déjà présente", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateBack();
    }
}
