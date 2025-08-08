using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.navigations;
using Mams.src.products;
using Mams.src.productsLots;
using Mams.src.receipts;
using Mams.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.profits; 
public class SaveProfitController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ProductModel _m_product_model;
    private readonly EntityModel _m_entity_model;
    private readonly ProductLotModel _m_product_lot_model;
    private readonly ReceiptProfitDetailedModel _m_receipt_profit_detailed_model;

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }
    public ICommand m_add_profit_item_command { get; set; }
    public ICommand m_delete_profit_item_command { get; set; }


    // Hold the receipt ID, supplier, date of all the items in m_list_receipt_product
    private ReceiptProfitDetailedItem _m_profit_receipt_detail;
    public ReceiptProfitDetailedItem m_profit_receipt_detail {
        get => _m_profit_receipt_detail;
        set {
            _m_profit_receipt_detail = value;
            onPropertyChanged();
        }
    }

    // This in the list of all the products in the receipt, each item is a line in the receipt
    private ObservableCollection<ReceiptProductItem> _m_list_receipt_product;
    public ObservableCollection<ReceiptProductItem> m_list_receipt_product {
        get => _m_list_receipt_product;
        set {
            _m_list_receipt_product = value;
            onPropertyChanged();
        }
    }

    // This is the list of all products available in the database, used to select a product in the receipt
    private ObservableCollection<ProductItem> _m_list_product;
    public ObservableCollection<ProductItem> m_list_product {
        get { return _m_list_product; }
        set {
            _m_list_product = value;
            onPropertyChanged();
        }
    }

    // This is the list of all entities available in the database, used to select an entity in the receipt
    private ObservableCollection<EntityItem> _m_list_entity;
    public ObservableCollection<EntityItem> m_list_entity {
        get { return _m_list_entity; }
        set {
            _m_list_entity = value;
            onPropertyChanged();
        }
    }

    // This is the list of all product lots available in the database, used to select a product lot in the receipt
    private ObservableCollection<ProductLotItem> _m_list_product_lot;
    public ObservableCollection<ProductLotItem> m_list_product_lot {
        get { return _m_list_product_lot; }
        set {
            _m_list_product_lot = value;
            onPropertyChanged();
        }
    }

    // Selected product in 1 line of the receipt, used to bind the product name in the UI
    private ProductItem? _m_selected_product;
    public ProductItem? m_selected_product {
        get { return _m_selected_product; }
        set {
            _m_selected_product = value;
            onPropertyChanged();
        }
    }

    // Selected entity in the header of the receipt, used to bind the entity name in the UI
    private EntityItem? _m_selected_entity;
    public EntityItem? m_selected_entity {
        get { return _m_selected_entity; }
        set {
            _m_selected_entity = value;
            onPropertyChanged();
        }
    }

    // Selected product lot in 1 line of the receipt, used to bind the product lot name in the UI
    private ProductLotItem? _m_selected_product_lot;
    public ProductLotItem? m_selected_product_lot {
        get { return _m_selected_product_lot; }
        set {
            _m_selected_product_lot = value;
            onPropertyChanged();
        }
    }


    public SaveProfitController(int id_to_load = 0) {

        _m_product_model = new();
        _m_product_lot_model = new();
        _m_entity_model = new();
        _m_receipt_profit_detailed_model = new();

        _m_list_product = _m_product_model.getTable();
        _m_list_product_lot = _m_product_lot_model.getTable();
        _m_list_entity = _m_entity_model.getTable();

        _m_list_receipt_product = new();
        _m_profit_receipt_detail = new();

        if (id_to_load > 0) {

            _m_profit_receipt_detail = _m_receipt_profit_detailed_model.getItemByID(id_to_load.ToString()) ?? new ReceiptProfitDetailedItem();
            _m_list_receipt_product = m_profit_receipt_detail.receipt_products;

            _m_selected_entity = _m_list_entity.FirstOrDefault(b =>
                b.entity_id == m_profit_receipt_detail.entity.entity_id) ?? new();

            // Find the product and product lot for each receipt product and set them
            // to the receipt product item
            foreach (var receipt_product in m_profit_receipt_detail.receipt_products) {
                ProductItem? product = _m_list_product.FirstOrDefault(b =>
                    b.product_id == receipt_product.product_item.product_id) ?? new();
                receipt_product.product_item = product;
                ProductLotItem? product_lot = _m_list_product_lot.FirstOrDefault(b =>
                    b.product_lot_id == receipt_product.product_lot_item.product_lot_id) ?? new();
                receipt_product.product_lot_item = product_lot;
            }
        }
        else {
            _m_list_receipt_product.Add(new());
        }

        m_save_command = new RelayCommand(saveProfit, canSaveProfit);
        m_abort_command = new RelayCommand(abortProfit);
        m_add_profit_item_command = new RelayCommand(addProfitItem);
        m_delete_profit_item_command = new RelayCommand(DeleteProfitItem);
    }


    private bool canSaveProfit(object? arg) {

        return m_selected_entity != null
            && SDataValidation.isDateValidFormatEU(m_profit_receipt_detail.receipt.receipt_date_created)
            && m_profit_receipt_detail.receipt.receipt_number != string.Empty // A method check if this receipt number is existing in the DB when saving
            && m_list_receipt_product.Count > 0
            && m_list_receipt_product.All(item =>
                item.receipt_product_unity_price >= 0
                && item.receipt_product_quantity > 0
                && !string.IsNullOrWhiteSpace(item.product_item.product_name)
            );
    }


    private void saveProfit(object? obj) {

        if (m_selected_entity == null) {
            return;
        }

        m_profit_receipt_detail.entity = m_selected_entity;

        // Link product and product lot data for each receipt product
        foreach (var receipt_product in m_list_receipt_product) {

            var product = receipt_product.product_item;
            var product_lot = receipt_product.product_lot_item;

            if (product != null) {
                receipt_product.product_item.product_id = product.product_id;
            }
            if (product_lot != null) {
                receipt_product.product_lot_item.product_lot_id = product_lot.product_lot_id;
            }
        }

        // Update the receipt products collection
        m_profit_receipt_detail.receipt_products = m_list_receipt_product;

        if (_m_receipt_profit_detailed_model.saveItem(m_profit_receipt_detail) > 0) {
            SPageNavigationController.navigateTo(new ListProfitPage());
        }
        else {
            MessageBox.Show($"Le numéro de facture donné ({m_profit_receipt_detail.receipt.receipt_number}) est déjà utilisé.", "Erreur de saisie", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }


    private void abortProfit(object? obj) {
        if (_m_profit_receipt_detail.receipt.receipt_id == 0) {
            if (m_list_receipt_product.Count > 1) {
                MessageBoxResult result = MessageBox.Show("En quittant la page, toutes les données modifiées seront perdues. Voulez-vous continuer?",
                    "Annuler", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No) {
                    return;
                }
            }
        }
        SPageNavigationController.navigateTo(new ListProfitPage());
    }


    private void addProfitItem(object? obj) {
        m_list_receipt_product.Add(new ReceiptProductItem());
    }


    private void DeleteProfitItem(object? parameter) {
        if (parameter is ReceiptProductItem item) {
            m_list_receipt_product.Remove(item);
        }
    }
}

