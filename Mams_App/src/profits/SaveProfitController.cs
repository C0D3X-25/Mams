using Mams_App.src.clients;
using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.entities;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.navigations;
using Mams_App.src.products;
using Mams_App.src.productsLots;
using Mams_App.src.receipts;
using Mams_App.src.services;
using Mams_App.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.profits; 
public class SaveProfitController : ABaseController, ICompareState {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_button_color { get; set; } = SGlobalView.m_page_button_color_1;
    public string m_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;
    public string m_delete_button_color { get; set; } = SGlobalView.m_page_button_color_2;
    public string m_delete_button_text_color { get; set; } = SGlobalView.m_page_button_text_color;


    private readonly ProductModel _m_product_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly ProductLotModel _m_product_lot_model = new();
    private readonly ReceiptProfitDetailedModel _m_receipt_profit_detailed_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }
    public ICommand m_add_profit_item_command { get; set; }
    public ICommand m_delete_profit_item_command { get; set; }
    public ICommand m_generate_invoice_pdf_command { get; set; }


    private ReceiptProfitDetailedItem _m_original_profit_receipt_detail = new();
    // Hold the receipt ID, supplier, date of all the items in m_list_receipt_product
    private ReceiptProfitDetailedItem _m_profit_receipt_detail = new();
    public ReceiptProfitDetailedItem m_profit_receipt_detail {
        get => _m_profit_receipt_detail;
        set {
            _m_profit_receipt_detail = value;
            onPropertyChanged();
        }
    }

    // This in the list of all the products in the receipt, each item is a line in the receipt
    private ObservableCollection<ReceiptProductItem> _m_list_receipt_product = new();
    public ObservableCollection<ReceiptProductItem> m_list_receipt_product {
        get => _m_list_receipt_product;
        set {
            _m_list_receipt_product = value;
            onPropertyChanged();
        }
    }

    // This is the list of all products available in the database, used to select a product in the receipt
    private ObservableCollection<ProductItem> _m_list_product = new();
    public ObservableCollection<ProductItem> m_list_product {
        get { return _m_list_product; }
        set {
            _m_list_product = value;
            onPropertyChanged();
        }
    }

    // This is the list of all entities available in the database, used to select an entity in the receipt
    private ObservableCollection<EntityItem> _m_list_entity = new();
    public ObservableCollection<EntityItem> m_list_entity {
        get { return _m_list_entity; }
        set {
            _m_list_entity = value;
            onPropertyChanged();
        }
    }

    // This is the list of all product lots available in the database, used to select a product lot in the receipt
    private ObservableCollection<ProductLotItem> _m_list_product_lot = new();
    public ObservableCollection<ProductLotItem> m_list_product_lot {
        get { return _m_list_product_lot; }
        set {
            _m_list_product_lot = value;
            onPropertyChanged();
        }
    }

    // Selected product in 1 line of the receipt, used to bind the product name in the UI
    private ProductItem _m_selected_product = new();
    public ProductItem m_selected_product {
        get { return _m_selected_product; }
        set {
            _m_selected_product = value;
            onPropertyChanged();
        }
    }

    // Selected entity in the header of the receipt, used to bind the entity name in the UI
    private EntityItem _m_selected_entity = new();
    public EntityItem m_selected_entity {
        get { return _m_selected_entity; }
        set {
            _m_selected_entity = value;
            m_profit_receipt_detail.entity = _m_selected_entity;
            onPropertyChanged();
        }
    }

    // Selected product lot in 1 line of the receipt, used to bind the product lot name in the UI
    private ProductLotItem _m_selected_product_lot = new();
    public ProductLotItem m_selected_product_lot {
        get { return _m_selected_product_lot; }
        set {
            _m_selected_product_lot = value;
            onPropertyChanged();
        }
    }


    public SaveProfitController(int id_to_load = 0) {

        _m_list_product = _m_product_model.getAllItems().returned_items;
        _m_list_product_lot = _m_product_lot_model.getAllItems().returned_items;
        _m_list_entity = _m_entity_model.getNonArchivedEntities();

        initializeProfit(id_to_load);

        m_save_command = new RelayCommand(saveProfit, canSaveProfit);
        m_abort_command = new RelayCommand(abortProfit);
        m_add_profit_item_command = new RelayCommand(addProfitItem);
        m_delete_profit_item_command = new RelayCommand(deleteProfitItem);
        m_generate_invoice_pdf_command = new RelayCommand(exportInvoicePdf);
    }
    
    /// <summary>
    /// Determines whether the current state of the profit receipt details matches the original state.
    /// </summary>
    /// <returns><see langword="true"/> if the current profit receipt details are identical to the original details;  otherwise,
    /// <see langword="false"/>.</returns>
    public bool isStateOriginal() {

        // Compare receipt properties
        if (_m_original_profit_receipt_detail.receipt.receipt_number != _m_profit_receipt_detail.receipt.receipt_number ||
            _m_original_profit_receipt_detail.receipt.receipt_date_created != _m_profit_receipt_detail.receipt.receipt_date_created ||
            _m_original_profit_receipt_detail.entity.entity_id != _m_profit_receipt_detail.entity.entity_id ||
            _m_original_profit_receipt_detail.receipt_products.Count != _m_profit_receipt_detail.receipt_products.Count) {

            return false;
        }
        
        // Compare each receipt product
        for (int i = 0; i < _m_original_profit_receipt_detail.receipt_products.Count; i++) {
            var original = _m_original_profit_receipt_detail.receipt_products[i];
            var current = _m_profit_receipt_detail.receipt_products[i];
            
            if (original.receipt_product_quantity != current.receipt_product_quantity ||
                original.receipt_product_unity_price != current.receipt_product_unity_price ||
                original.product_item.product_id != current.product_item.product_id ||
                original.product_lot_item.product_lot_id != current.product_lot_item.product_lot_id) {

                return false;
            }
        }
        return true;
    }


    private void initializeProfit(int id_to_load) {

        if (id_to_load > 0) {

            _m_profit_receipt_detail = _m_receipt_profit_detailed_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_original_profit_receipt_detail = _m_receipt_profit_detailed_model.getItemByID(id_to_load.ToString()).returned_item ?? new();

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
    }


    private bool canSaveProfit(object? arg) {

        return m_selected_entity != null
            && m_selected_entity.entity_id > 0
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

        var result = _m_receipt_profit_detailed_model.saveItem(m_profit_receipt_detail);
        if (result.is_success) {
            SPageNavigationController.navigateBack();
        }
        else {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    private void abortProfit(object? obj) {
        SPageNavigationController.navigateBack(true);
    }


    private void addProfitItem(object? obj) {
        m_list_receipt_product.Add(new ReceiptProductItem());
    }


    private void deleteProfitItem(object? obj) {
        if (obj is ReceiptProductItem item) {
            m_list_receipt_product.Remove(item);
        }
    }


    private void exportInvoicePdf(object? obj) {

        ServicePDF service_pdf = new();
        service_pdf.generateAndOpenInvoice(m_profit_receipt_detail.receipt.receipt_id.ToString());
    }
}

