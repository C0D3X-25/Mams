using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.productsTypes;

public class SaveProductTypeController : ABaseController, ICompareState {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ProductTypeModel _m_product_type_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductTypeItem _m_original_product_type = new();
    private ProductTypeItem _m_product_type = new();
    public ProductTypeItem m_product_type {
        get => _m_product_type;
        set {
            _m_product_type = value;
            onPropertyChanged();
        }
    }


    public SaveProductTypeController(int id_to_load = 0) {

        if (id_to_load != 0) {
            _m_original_product_type = _m_product_type_model.getItemByID(id_to_load.ToString()).returned_item ?? new ProductTypeItem();
            _m_product_type = _m_product_type_model.getItemByID(id_to_load.ToString()).returned_item ?? new ProductTypeItem();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }

    public bool isStateOriginal() {
        if (!_m_original_product_type.product_type_id.Equals(_m_product_type.product_type_id)
            || !_m_original_product_type.product_type_name.Equals(_m_product_type.product_type_name, StringComparison.Ordinal)
            || !_m_original_product_type.product_type_archive.Equals(_m_product_type.product_type_archive, StringComparison.Ordinal)) {
            return false;
        }
        return true;
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_type.product_type_name);
    }


    private void saveProduct(object? obj) {
        var result = _m_product_type_model.saveItem(m_product_type);
        if (result.is_success) {
            SPageNavigationController.navigateBack();
        }
        else {
            MessageBox.Show("Un type de produit avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateBack(true);
    }
}
