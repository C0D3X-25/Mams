using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.receipts;
using Mams.src.suppliers;
using System.Collections.ObjectModel;

namespace Mams.src.fees;

/// <summary>
/// Represents a detailed model for managing receipt fees, including operations for CRUD functionality.
/// </summary>
public class ReceiptFeeDetailedModel : ABaseModel,
    ICrudOperation<ReceiptFeeDetailedItem> {

    private readonly ReceiptHandlerModel _m_receipt_handler_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly SupplierModel _m_supplier_model = new();
    private readonly ProductModel _m_product_model = new();

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns><see langword="true"/> if the item was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return _m_receipt_handler_model.deleteItem(id);
    }

    /// <summary>
    /// Retrieves a detailed receipt fee item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt to retrieve.</param>
    /// <returns>A <see cref="ReceiptFeeDetailedItem"/> containing detailed information about the receipt,  including associated
    /// products, supplier, and entity data. Returns <see langword="null"/>  if no receipt is found for the specified
    /// identifier.</returns>
    public ReceiptFeeDetailedItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        var receipt = _m_receipt_handler_model.getItemByID(id);
        if (receipt == null) {
            return null;
        }

        ReceiptFeeDetailedItem item = new() {
            receipt = receipt.receipt_item,
            receipt_products = receipt.receipt_product_items,
            receipt_supplier = receipt.receipt_supplier_item,
            supplier = _m_supplier_model.getItemByID(receipt.receipt_supplier_item.fk_supplier_id.ToString()) ?? new(),
        };

        item.entity = _m_entity_model.getItemByID(item.supplier.fk_entity_id.ToString()) ?? new();

        foreach (var receipt_product in item.receipt_products) {
            ProductItem? product = _m_product_model.getItemByID(receipt_product.product_item.product_id.ToString());
            if (product != null) {
                receipt_product.product_item = product;
            }
        }
        return item;
    }

    /// <summary>
    /// Retrieves a collection of detailed receipt fee items, including associated supplier, entity, and product
    /// information.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="ReceiptFeeDetailedItem"/> objects, where each item
    /// contains detailed information about a receipt, its associated products, supplier, and entity. The collection
    /// will be empty if no valid receipts are found.</returns>
    public ObservableCollection<ReceiptFeeDetailedItem> getTable() {

        ObservableCollection<ReceiptFeeDetailedItem> items = new();

        foreach (var receipt in _m_receipt_handler_model.getTable()) {

            // Skip because it's a profit receipt
            if (receipt.receipt_client_item.fk_client_id != 0) {
                continue;
            }

            ReceiptFeeDetailedItem item = new() {
                receipt = receipt.receipt_item,
                receipt_products = receipt.receipt_product_items,
                receipt_supplier = receipt.receipt_supplier_item,
                supplier = _m_supplier_model.getItemByID(receipt.receipt_supplier_item.fk_supplier_id.ToString()) ?? new(),
            };

            item.entity = _m_entity_model.getItemByID(item.supplier.fk_entity_id.ToString()) ?? new();

            foreach (var receipt_product in item.receipt_products) {
                ProductItem? product = _m_product_model.getItemByID(receipt_product.product_item.product_id.ToString());
                if (product != null) {
                    receipt_product.product_item = product;
                }
            }

            items.Add(item);
        }
        return items;
    }

    /// <summary>
    /// Saves the specified receipt item and its associated details to the database.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptFeeDetailedItem"/> object containing the receipt, products, supplier, and entity details.
    /// The object must not be <see langword="null"/>, must contain at least one product, and the entity ID must be
    /// greater than zero.</param>
    /// <returns>An integer representing the result of the save operation. Returns 0 if the input is invalid or the save
    /// operation fails.</returns>
    public int saveItem(ReceiptFeeDetailedItem item) {

        if (item == null 
            || item.receipt_products.Count() < 1 
            || item.entity.entity_id == 0
            ) {
            return 0;
        }

        int supplier_id = findSupplierIdOrCreateNew(item.entity.entity_id);
        item.receipt_supplier.fk_supplier_id = supplier_id;
        item.supplier.supplier_id = supplier_id;

        UpdateReceiptTotalPrice(item);

        var handlerItem = new ReceiptHandlerItem {
            receipt_item = item.receipt,
            receipt_product_items = item.receipt_products,
            receipt_supplier_item = item.receipt_supplier
        };

        return _m_receipt_handler_model.saveItem(handlerItem); 
    }

    /// <summary>
    /// Retrieves the supplier ID associated with the specified entity ID, or creates a new supplier if none exists.
    /// </summary>
    /// <param name="entity_id">The ID of the entity for which the supplier ID is to be retrieved or created. Must be greater than 0.</param>
    /// <returns>The supplier ID associated with the specified entity ID. Returns 0 if the <paramref name="entity_id"/> is
    /// invalid or if the operation fails to create or retrieve a supplier.</returns>
    private int findSupplierIdOrCreateNew(int entity_id) {

        if (!SDataValidation.isIdValid(entity_id)) {
            return 0;
        }

        var supplier = _m_supplier_model.getSupplierWithEntityFK(entity_id.ToString());
        if (supplier != null && supplier.supplier_id > 0) {
            return supplier.supplier_id;
        }

        var new_supplier = new SupplierItem {
            fk_entity_id = entity_id 
        };
        _m_supplier_model.saveItem(new_supplier);

        supplier = _m_supplier_model.getSupplierWithEntityFK(entity_id.ToString());
        return supplier?.supplier_id ?? 0;
    }

    /// <summary>
    /// Updates the total price of the receipt based on the quantities and unit prices of the products.
    /// </summary>
    /// <param name="item">The detailed receipt item containing the list of products and their associated quantities and unit prices.</param>
    private void UpdateReceiptTotalPrice(ReceiptFeeDetailedItem item) {
        decimal total = 0.0M;
        foreach (var product in item.receipt_products) {
            total += product.receipt_product_quantity * product.receipt_product_unity_price;
        }
        item.receipt.receipt_total_price = total;
    }
}
