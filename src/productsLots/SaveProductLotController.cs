using Mams.src.beehives;
using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.helpers;
using Mams.src.navigations;
using Mams.src.products;
using Mams.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.productsLots;

public class SaveProductLotController : ABaseController, ICompareState {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ProductLotModel _m_product_lot_model = new();
    private readonly BeehiveModel _m_beehive_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }


    private ProductLotItem _m_original_product_lot = new();
    private ProductLotItem _m_product_lot = new();
    public ProductLotItem m_product_lot {
        get => _m_product_lot;
        set {
            _m_product_lot = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<BeehiveItem> _m_list_beehive = new();
    public ObservableCollection<BeehiveItem> m_list_beehive {
        get { return _m_list_beehive; }
        set { 
            _m_list_beehive = value;
            onPropertyChanged();
        }
    }


    private BeehiveItem _m_selected_beehive = new();
    public BeehiveItem m_selected_beehive {
        get { return _m_selected_beehive; }
        set { 
            _m_selected_beehive = value;
            m_product_lot.fk_beehive_id = _m_selected_beehive.beehive_id;
            onPropertyChanged();
        }
    }


    public SaveProductLotController(int id_to_load = 0) {
        
        _m_list_beehive = _m_beehive_model.getAllItems().returned_items;

        if (id_to_load != 0) {
            _m_original_product_lot = _m_product_lot_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_product_lot = _m_product_lot_model.getItemByID(id_to_load.ToString()).returned_item ?? new();

            // Find the matching beehive in the list and set it as selected
            _m_selected_beehive = _m_list_beehive.FirstOrDefault(b =>
                b.beehive_id == _m_product_lot.fk_beehive_id) ?? new();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }

    public bool isStateOriginal() {
        if (!_m_original_product_lot.product_lot_id.Equals(_m_product_lot.product_lot_id)
            || !_m_original_product_lot.product_lot_name.Equals(_m_product_lot.product_lot_name, StringComparison.Ordinal)
            || !_m_original_product_lot.product_lot_year.Equals(_m_product_lot.product_lot_year)
            || !_m_original_product_lot.fk_beehive_id.Equals(_m_product_lot.fk_beehive_id)
            || !_m_original_product_lot.beehive_name.Equals(_m_product_lot.beehive_name, StringComparison.Ordinal)
            ) {
            return false;
        }
        return true;

    }

    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_lot.product_lot_name)
            && SDataValidation.isYearInRange(m_product_lot.product_lot_year);
    }


    private void saveProduct(object? obj) {
        if (m_selected_beehive != null) {
            m_product_lot.fk_beehive_id = m_selected_beehive.beehive_id;
            m_product_lot.beehive_name = m_selected_beehive.beehive_name;
        }

        var result = _m_product_lot_model.saveItem(m_product_lot);
        if (result.is_success) {
            SPageNavigationController.navigateBack();
        }
        else {
            MessageBox.Show("Un lot avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        SPageNavigationController.navigateBack(true);
    }
}