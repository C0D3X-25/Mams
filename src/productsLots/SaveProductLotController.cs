using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.navigations;
using Mams.src.products;
using System.Windows.Input;
using System.Windows;
using Mams.src.beehives;
using System.Collections.ObjectModel;
using Mams.src.helpers;

namespace Mams.src.productsLots;

public class SaveProductLotController : ABaseController {

    
    private readonly ProductLotModel _m_product_lot_model;
    private readonly BeehiveModel _m_beehive_model;

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


    private ObservableCollection<BeehiveItem> _m_list_beehive;
    public ObservableCollection<BeehiveItem> m_list_beehive {
        get { return _m_list_beehive; }
        set { 
            _m_list_beehive = value;
            onPropertyChanged();
        }
    }


    private BeehiveItem? _m_selected_beehive;
    public BeehiveItem? m_selected_beehive {
        get { return _m_selected_beehive; }
        set { 
            _m_selected_beehive = value; 
            onPropertyChanged();
        }
    }


    public SaveProductLotController(int id_to_load = 0) {
        
        _m_product_lot_model = new();
        _m_beehive_model = new();
        _m_product_lot = new ProductLotItem();
        _m_list_beehive = _m_beehive_model.getTable();

        if (id_to_load != 0) {
            _m_product_lot = _m_product_lot_model.getItemByID(id_to_load.ToString()) ?? new ProductLotItem();
            // Find the matching beehive in the list and set it as selected
            _m_selected_beehive = _m_list_beehive.FirstOrDefault(b => b.beehive_id == _m_product_lot.fk_beehive_id) ?? new BeehiveItem();
        }

        m_save_command = new RelayCommand(saveProduct, canSaveProduct);
        m_abort_command = new RelayCommand(abortProduct);
    }


    private bool canSaveProduct(object? arg) {
        return !string.IsNullOrEmpty(m_product_lot.product_lot_name)
            && SDateValidation.isYearInRange(m_product_lot.product_lot_year);
    }


    private void saveProduct(object? obj) {
        if (m_selected_beehive != null) {
            m_product_lot.fk_beehive_id = m_selected_beehive.beehive_id;
            m_product_lot.beehive_name = m_selected_beehive.beehive_name;
        }

        if (_m_product_lot_model.saveItem(m_product_lot)) {
            PageNavigationController.navigateTo(new ListProductLotPage());
        }
        else {
            MessageBox.Show("Un lot avec le même nom est déjà présent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProduct(object? obj) {
        PageNavigationController.navigateTo(new ListProductLotPage());
    }
}