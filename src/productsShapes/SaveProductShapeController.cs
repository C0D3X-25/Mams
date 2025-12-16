using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.productsShapes;

public class SaveProductShapeController : ABaseController, ICompareState {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ProductShapeModel _m_product_shape_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductShapeItem _m_original_product_shape = new();
    private ProductShapeItem _m_product_shape = new();
    public ProductShapeItem m_product_shape {
        get => _m_product_shape;
        set {
            _m_product_shape = value;
            onPropertyChanged();
        }
    }


    public SaveProductShapeController(int id_to_load = 0) {

        if (id_to_load != 0) {
            _m_original_product_shape = _m_product_shape_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_product_shape = _m_product_shape_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }

    public bool isStateOriginal() {
        if (!_m_original_product_shape.product_shape_id.Equals(_m_product_shape.product_shape_id)
            || !_m_original_product_shape.product_shape_name.Equals(_m_product_shape.product_shape_name, StringComparison.Ordinal)
            || !_m_original_product_shape.product_shape_archive.Equals(_m_product_shape.product_shape_archive, StringComparison.Ordinal)
            ) {
            return false;
        }
        return true;
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_shape.product_shape_name);
    }


    private void saveProduct(object? obj) {
        var result = _m_product_shape_model.saveItem(m_product_shape);
        if (result.is_success) {
            SPageNavigationController.navigateBack();
        }
        else {
            MessageBox.Show("Une forme avec le même nom est déjà présente", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateBack(true);
    }
}
