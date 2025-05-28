using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.navigations;
using Mams.src.products;
using Mams.src.receipts;
using Mams.src.suppliers;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.fees; 
public class SaveFeeController : ABaseController {

    private readonly ProductModel _m_product_model;
    private readonly SupplierModel _m_supplier_model;
    private readonly EntityModel _m_entity_model;
    //private readonly FeeProductModel _m_fee_model;
    private readonly ReceiptHandlerModel _m_receipt_handler_model;

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }
    public ICommand m_add_fee_item_command { get; set; }
    public ICommand m_delete_fee_item_command { get; set; }


    // Hold the receipt ID, supplier, date of all the items in m_list_fee
    private FeeReceiptItem _m_fee_receipt;
    public FeeReceiptItem m_fee_receipt {
        get => _m_fee_receipt;
        set {
            _m_fee_receipt = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<FeeProductItem> _m_list_fee;
    public ObservableCollection<FeeProductItem> m_list_fee {
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


    private ObservableCollection<EntityItem> _m_list_supplier;
    public ObservableCollection<EntityItem> m_list_supplier {
        get { return _m_list_supplier; }
        set {
            _m_list_supplier = value;
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
    public EntityItem? m_selected_supplier {
        get { return _m_selected_supplier; }
        set {
            _m_selected_supplier = value;
            onPropertyChanged();
        }
    }

    public SaveFeeController(int id_to_load = 0) {

        _m_product_model = new();
        _m_supplier_model = new();
        _m_entity_model = new();
        //_m_fee_model = new();
        _m_receipt_handler_model = new();


        _m_list_product = _m_product_model.getTable();
        _m_list_supplier = _m_entity_model.getTable();

        _m_list_fee = new();
        _m_fee_receipt = new();

        if (id_to_load != 0) {
            //    _m_fee = _m_product_model.getItemByID(id_to_load.ToString()) ?? new ProductItem();

            //    // Find the matching product and supplier in the list and set it as selected or create a new one
            //    _m_selected_supplier = _m_list_supplier.FirstOrDefault(b => b.product_category_id == _m_fee.fk_product_category_id) ?? new ProductCategoryItem();
            //    _m_selected_product = _m_list_product.FirstOrDefault(b => b.product_id == _m_fee.fk_product_id) ?? new ProductItem();
        }
        else {
            m_list_fee.Add(new FeeProductItem());
        }

        m_save_command = new RelayCommand(saveFee, canSaveFee);
        m_abort_command = new RelayCommand(abortFee);
        m_add_fee_item_command = new RelayCommand(addFeeItem);
        m_delete_fee_item_command = new RelayCommand(DeleteFeeItem);
    }


    private bool canSaveFee(object? arg) {

        return m_selected_supplier != null
            && SDateValidation.isDateValidFormatEU(m_fee_receipt.receipt_date_created)
            && m_list_fee.Count > 0
            && m_list_fee.All(item =>
                item.fee_price_unity >= 0
                && item.fee_quantity > 0
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
        m_list_fee.Add(new FeeProductItem());
    }


    private void DeleteFeeItem(object? parameter) {
        if (parameter is FeeProductItem item) {
            m_list_fee.Remove(item);
        }
    }
}
