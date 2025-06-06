using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.navigations;
using Mams.src.products;
using Mams.src.productsLots;
using Mams.src.receipts;
using Mams.src.suppliers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.fees; 
public class SaveFeeController : ABaseController {

    private readonly ProductModel _m_product_model;
    private readonly EntityModel _m_entity_model;
    private readonly ProductLotModel _m_product_lot_model;
    private readonly ReceiptFeeDetailedModel _m_receipt_fee_detailed_model;

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }
    public ICommand m_add_fee_item_command { get; set; }
    public ICommand m_delete_fee_item_command { get; set; }


    // Hold the receipt ID, supplier, date of all the items in m_list_fee
    private ReceiptFeeDetailedItem _m_fee_receipt_detail;
    public ReceiptFeeDetailedItem m_fee_receipt_detail {
        get => _m_fee_receipt_detail;
        set {
            _m_fee_receipt_detail = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<ReceiptProductItem> _m_list_fee;
    public ObservableCollection<ReceiptProductItem> m_list_fee {
        get => _m_list_fee;
        set {
            _m_list_fee = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<ProductItem> _m_list_product;
    public ObservableCollection<ProductItem> m_list_product {
        get { return _m_list_product; }
        set {
            _m_list_product = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<EntityItem> _m_list_entity;
    public ObservableCollection<EntityItem> m_list_entity {
        get { return _m_list_entity; }
        set {
            _m_list_entity = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<ProductLotItem> _m_list_product_lot;
    public ObservableCollection<ProductLotItem> m_list_product_lot {
        get { return _m_list_product_lot; }
        set {
            _m_list_product_lot = value;
            onPropertyChanged();
        }
    }


    private ProductItem? _m_selected_product;
    public ProductItem? m_selected_product {
        get { return _m_selected_product; }
        set {
            _m_selected_product = value;
            onPropertyChanged();
        }
    }


    private EntityItem? _m_selected_supplier;
    public EntityItem? m_selected_entity {
        get { return _m_selected_supplier; }
        set {
            _m_selected_supplier = value;
            onPropertyChanged();
        }
    }


    private ProductLotItem? _m_selected_product_lot;
    public ProductLotItem? m_selected_product_lot {
        get { return _m_selected_product_lot; }
        set {
            _m_selected_product_lot = value;
            onPropertyChanged();
        }
    }

    public SaveFeeController(int id_to_load = 0) {

        _m_product_model = new();
        _m_product_lot_model = new();
        _m_entity_model = new();
        _m_receipt_fee_detailed_model = new();

        _m_list_product = _m_product_model.getTable();
        _m_list_product_lot = _m_product_lot_model.getTable();
        _m_list_entity = _m_entity_model.getTable();

        _m_list_fee = new();
        _m_fee_receipt_detail = new();

        if (id_to_load != 0) {
            //m_fee_receipt_detail.receipt_products

            m_fee_receipt_detail = _m_receipt_fee_detailed_model.getItemByID(id_to_load.ToString()) ?? new ReceiptFeeDetailedItem();
            m_selected_entity = m_fee_receipt_detail.entity;
            m_list_fee = new ObservableCollection<ReceiptProductItem>(m_fee_receipt_detail.receipt_products.Select(product => new ReceiptProductItem {
                product_item = product.product_item,
                receipt_product_unity_price = product.receipt_product_unity_price,
                receipt_product_quantity = product.receipt_product_quantity,
                fk_product_id = product.fk_product_id,
                fk_receipt_id = product.fk_receipt_id,
                receipt_product_id = product.receipt_product_id,
                fk_product_lot_id = product.fk_product_lot_id
            }));
        }
        else {
            m_list_fee.Add(new ReceiptProductItem());
        }

        m_save_command = new RelayCommand(saveFee, canSaveFee);
        m_abort_command = new RelayCommand(abortFee);
        m_add_fee_item_command = new RelayCommand(addFeeItem);
        m_delete_fee_item_command = new RelayCommand(DeleteFeeItem);
    }


    private bool canSaveFee(object? arg) {

        return m_selected_entity != null
            && SDateValidation.isDateValidFormatEU(m_fee_receipt_detail.receipt.receipt_date_created)
            && m_list_fee.Count > 0
            && m_list_fee.All(item =>
                item.receipt_product_unity_price >= 0
                && item.receipt_product_quantity > 0
                && !string.IsNullOrWhiteSpace(item.product_item.product_name)
            );
    }


    private void saveFee(object? obj) {

        //if (_m_receipt_cascade_model.saveItem()) {
        //    SPageNavigationController.navigateTo(new ListFeePage());
        //}
        //else {
        //    MessageBox.Show("Error saving the fee");
        //}
    }


    private void abortFee(object? obj) {
        if (m_list_fee.Count > 1) {
            MessageBoxResult result = MessageBox.Show("En quittant la page, toutes les données seront perdues. Voulez-vous continuer?",
                "Annuler", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No) {
                return;
            }
        }
        SPageNavigationController.navigateTo(new ListFeePage());
    }


    private void addFeeItem(object? obj) {
        m_list_fee.Add(new ReceiptProductItem());
    }


    private void DeleteFeeItem(object? parameter) {
        if (parameter is ReceiptProductItem item) {
            m_list_fee.Remove(item);
        }
    }
}
