using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.entities;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.invoices;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.products;
using Mams_App.src.productsLots;
using Mams_App.src.receipts;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.fees;

/// <summary>
/// Controller for managing the creation and modification of fee receipts.
/// </summary>
public class SaveFeeController : ABaseController, ICompareState
{

    private readonly ProductModel _m_product_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly ProductLotModel _m_product_lot_model = new();
    private readonly ReceiptFeeDetailedModel _m_receipt_fee_detailed_model = new();

    public ICommand m_save_command { get; set; }
    public ICommand m_abort_command { get; set; }
    public ICommand m_add_fee_item_command { get; set; }
    public ICommand m_delete_fee_item_command { get; set; }
    public ICommand m_generate_invoice_pdf_command { get; set; }

    private ReceiptFeeDetailedItem _m_original_fee_receipt_detail = new();
    // Hold the receipt ID, supplier, date of all the items in m_list_receipt_product
    private ReceiptFeeDetailedItem _m_fee_receipt_detail = new();
    public ReceiptFeeDetailedItem m_fee_receipt_detail
    {
        get => _m_fee_receipt_detail;
        set
        {
            _m_fee_receipt_detail = value;
            onPropertyChanged();
        }
    }

    // This in the list of all the products in the receipt, each item is a line in the receipt
    private ObservableCollection<ReceiptProductItem> _m_list_receipt_product = new();
    public ObservableCollection<ReceiptProductItem> m_list_receipt_product
    {
        get => _m_list_receipt_product;
        set
        {
            _m_list_receipt_product = value;
            onPropertyChanged();
        }
    }

    // This is the list of all products available in the database, used to select a product in the receipt
    private ObservableCollection<ProductItem> _m_list_product = new();
    public ObservableCollection<ProductItem> m_list_product
    {
        get { return _m_list_product; }
        set
        {
            _m_list_product = value;
            onPropertyChanged();
        }
    }

    // This is the list of all entities available in the database, used to select an entity in the receipt
    private ObservableCollection<EntityItem> _m_list_entity = new();
    public ObservableCollection<EntityItem> m_list_entity
    {
        get { return _m_list_entity; }
        set
        {
            _m_list_entity = value;
            onPropertyChanged();
        }
    }

    // This is the list of all product lots available in the database, used to select a product lot in the receipt
    private ObservableCollection<ProductLotItem> _m_list_product_lot = new();
    public ObservableCollection<ProductLotItem> m_list_product_lot
    {
        get { return _m_list_product_lot; }
        set
        {
            _m_list_product_lot = value;
            onPropertyChanged();
        }
    }

    // Selected product in 1 line of the receipt, used to bind the product name in the UI
    private ProductItem _m_selected_product = new();
    public ProductItem m_selected_product
    {
        get { return _m_selected_product; }
        set
        {
            _m_selected_product = value;
            onPropertyChanged();
        }
    }

    // Selected entity in the header of the receipt, used to bind the entity name in the UI
    private EntityItem _m_selected_entity = new();
    public EntityItem m_selected_entity
    {
        get { return _m_selected_entity; }
        set
        {
            _m_selected_entity = value;
            m_fee_receipt_detail.entity = _m_selected_entity;
            onPropertyChanged();
        }
    }

    // Selected product lot in 1 line of the receipt, used to bind the product lot name in the UI
    private ProductLotItem _m_selected_product_lot = new();
    public ProductLotItem m_selected_product_lot
    {
        get { return _m_selected_product_lot; }
        set
        {
            _m_selected_product_lot = value;
            onPropertyChanged();
        }
    }


    /// <summary>
    /// Initializes a new instance of the SaveFeeController class.
    /// </summary>
    /// <param name="id_to_load">Optional ID of an existing fee receipt to modify. If 0, creates a new fee.</param>
    public SaveFeeController(int id_to_load = 0)
    {

        _m_list_product = _m_product_model.getActiveProducts();
        _m_list_product_lot = _m_product_lot_model.getActiveProductLots();
        _m_list_entity = _m_entity_model.getActiveEntities();

        initializeFee(id_to_load);

        m_save_command = new RelayCommand(saveFee, canSaveFee);
        m_abort_command = new RelayCommand(abortFee);
        m_add_fee_item_command = new RelayCommand(addFeeItem);
        m_delete_fee_item_command = new RelayCommand(DeleteFeeItem);
        m_generate_invoice_pdf_command = new RelayCommand(exportInvoicePdf);
    }

    /// <summary>
    /// Determines whether the current state of the fee receipt details matches the original state.
    /// </summary>
    /// <remarks>This method compares the receipt properties, associated entities, and receipt products
    /// between the original and current fee receipt details. If any discrepancies are found, the method returns <see
    /// langword="false"/>.</remarks>
    /// <returns><see langword="true"/> if the current fee receipt details are identical to the original fee receipt details;
    /// otherwise, <see langword="false"/>.</returns>
    public bool isStateOriginal()
    {

        // Compare receipt properties
        if (_m_original_fee_receipt_detail.receipt.receipt_number != _m_fee_receipt_detail.receipt.receipt_number ||
            _m_original_fee_receipt_detail.receipt.receipt_date_created != _m_fee_receipt_detail.receipt.receipt_date_created ||
            _m_original_fee_receipt_detail.entity.entity_id != _m_fee_receipt_detail.entity.entity_id ||
            _m_original_fee_receipt_detail.receipt_products.Count != _m_fee_receipt_detail.receipt_products.Count)
        {

            return false;
        }

        // Compare each receipt product
        for (int i = 0; i < _m_original_fee_receipt_detail.receipt_products.Count; i++)
        {
            var original = _m_original_fee_receipt_detail.receipt_products[i];
            var current = _m_fee_receipt_detail.receipt_products[i];

            if (original.receipt_product_quantity != current.receipt_product_quantity ||
                original.receipt_product_unity_price != current.receipt_product_unity_price ||
                original.product_item.product_id != current.product_item.product_id ||
                original.product_lot_item.product_lot_id != current.product_lot_item.product_lot_id)
            {

                return false;
            }
        }
        return true;
    }


    /// <summary>
    /// Initializes the fee receipt data from the database or creates a new empty receipt.
    /// </summary>
    /// <param name="id_to_load">The ID of the receipt to load, or 0 for a new receipt.</param>
    private void initializeFee(int id_to_load)
    {

        if (id_to_load > 0)
        {

            _m_fee_receipt_detail = _m_receipt_fee_detailed_model.getItemByID(id_to_load.ToString()).returned_item ?? new();
            _m_original_fee_receipt_detail = _m_receipt_fee_detailed_model.getItemByID(id_to_load.ToString()).returned_item ?? new();

            _m_list_receipt_product = m_fee_receipt_detail.receipt_products;

            _m_selected_entity = _m_list_entity.FirstOrDefault(b =>
                b.entity_id == m_fee_receipt_detail.entity.entity_id) ?? new();

            // Find the product and product lot for each receipt product and set them
            // to the receipt product item
            foreach (var receipt_product in m_fee_receipt_detail.receipt_products)
            {
                ProductItem? product = _m_list_product.FirstOrDefault(b =>
                    b.product_id == receipt_product.product_item.product_id) ?? new();
                receipt_product.product_item = product;
                ProductLotItem? product_lot = _m_list_product_lot.FirstOrDefault(b =>
                    b.product_lot_id == receipt_product.product_lot_item.product_lot_id) ?? new();
                receipt_product.product_lot_item = product_lot;
            }
        }
        else
        {
            _m_fee_receipt_detail = new();
            _m_original_fee_receipt_detail = new();
            _m_list_receipt_product.Add(new());
        }
    }


    /// <summary>
    /// Determines whether the fee can be saved based on validation rules.
    /// </summary>
    /// <param name="arg">Command parameter (not used).</param>
    /// <returns>True if the fee has valid data; otherwise, false.</returns>
    private bool canSaveFee(object? arg)
    {

        return m_selected_entity != null
            && m_selected_entity.entity_id > 0
            && SDataValidation.isDateValidFormatEU(m_fee_receipt_detail.receipt.receipt_date_created)
            && m_fee_receipt_detail.receipt.receipt_number != string.Empty // A method check if this receipt number is existing in the DB when saving
            && m_list_receipt_product.Count > 0
            && m_list_receipt_product.All(item =>
                item.receipt_product_unity_price >= 0
                && item.receipt_product_quantity > 0
                && !string.IsNullOrWhiteSpace(item.product_item.product_name)
            );
    }


    /// <summary>
    /// Saves the current fee receipt to the database.
    /// If successful, navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void saveFee(object? obj)
    {

        if (m_selected_entity == null)
        {
            return;
        }

        m_fee_receipt_detail.entity = m_selected_entity;

        // Link product and product lot data for each receipt product
        foreach (var receipt_product in m_list_receipt_product)
        {

            var product = receipt_product.product_item;
            var product_lot = receipt_product.product_lot_item;

            if (product != null)
            {
                receipt_product.product_item.product_id = product.product_id;
            }
            if (product_lot != null)
            {
                receipt_product.product_lot_item.product_lot_id = product_lot.product_lot_id;
            }
        }

        // Update the receipt products collection
        m_fee_receipt_detail.receipt_products = m_list_receipt_product;

        var result = _m_receipt_fee_detailed_model.saveItem(m_fee_receipt_detail);
        if (result.is_success)
        {
            SPageNavigationController.navigateBack();
        }
        else
        {
            string userMessage = SErrorMessageHelper.GetSaveErrorMessage(result.error);
            string fullMessage = SErrorMessageHelper.BuildFullMessage(userMessage, result.error_message_detail);
            MessageBox.Show(fullMessage, Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    /// <summary>
    /// Cancels the current operation and navigates back to the previous page.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void abortFee(object? obj)
    {
        SPageNavigationController.navigateBack(true);
    }


    /// <summary>
    /// Adds a new empty fee item line to the receipt.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void addFeeItem(object? obj)
    {
        m_list_receipt_product.Add(new ReceiptProductItem());
    }


    /// <summary>
    /// Deletes the specified fee item from the receipt.
    /// </summary>
    /// <param name="parameter">The ReceiptProductItem to delete.</param>
    private void DeleteFeeItem(object? parameter)
    {
        if (parameter is ReceiptProductItem item)
        {
            m_list_receipt_product.Remove(item);
        }
    }


    /// <summary>
    /// Exports the current fee receipt as a PDF invoice.
    /// </summary>
    /// <param name="obj">Command parameter (not used).</param>
    private void exportInvoicePdf(object? obj)
    {
        SInvoicePdfService.generateAndOpenFeeInvoice(m_fee_receipt_detail.receipt.receipt_id.ToString());
    }
}
