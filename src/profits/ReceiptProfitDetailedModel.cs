using Mams.src.clients;
using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.receipts;
using System.Collections.ObjectModel;

namespace Mams.src.profits;

/// <summary>
/// Represents a detailed model for managing receipt profit data, including operations for CRUD functionality.
/// </summary>
public class ReceiptProfitDetailedModel : ABaseModel,
    ICrudOperation<ReceiptProfitDetailedItem> {

    private readonly ReceiptHandlerModel _m_receipt_handler_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly ClientModel _m_client_model = new();
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
    /// Retrieves a detailed receipt item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt item. Must be a valid identifier.</param>
    /// <returns>A <see cref="ReceiptProfitDetailedItem"/> object containing detailed information about the receipt,  including
    /// associated products, client, and entity data, or <see langword="null"/> if the identifier is invalid  or no
    /// matching receipt is found.</returns>
    public ReceiptProfitDetailedItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        var receipt = _m_receipt_handler_model.getItemByID(id);

        if (receipt == null) {
            return null;
        }

        ReceiptProfitDetailedItem item = new() {
            receipt = receipt.receipt_item,
            receipt_products = receipt.receipt_product_items,
            receipt_client = receipt.receipt_client_item,
            client = _m_client_model.getItemByID(receipt.receipt_client_item.fk_client_id.ToString()) ?? new(),
        };

        item.entity = _m_entity_model.getItemByID(item.client.fk_entity_id.ToString()) ?? new();

        foreach (var receipt_product in item.receipt_products) {
            ProductItem? product = _m_product_model.getItemByID(receipt_product.product_item.product_id.ToString());
            if (product != null) {
                receipt_product.product_item = product;
            }
        }
        return item;
    }

    /// <summary>
    /// Retrieves a collection of detailed receipt profit items.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing detailed receipt profit items.  The collection will be empty
    /// if no valid receipts are found.</returns>
    public ObservableCollection<ReceiptProfitDetailedItem> getTable() {

        ObservableCollection<ReceiptProfitDetailedItem> items = new();

        foreach (var receipt in _m_receipt_handler_model.getTable()) {

            // Skip because it's a fee receipt
            if (receipt.receipt_supplier_item.fk_supplier_id != 0) {
                continue;
            }

            ReceiptProfitDetailedItem item = new() {
                receipt = receipt.receipt_item,
                receipt_products = receipt.receipt_product_items,
                receipt_client = receipt.receipt_client_item,
                client = _m_client_model.getItemByID(receipt.receipt_client_item.fk_client_id.ToString()) ?? new(),
            };

            item.entity = _m_entity_model.getItemByID(item.client.fk_entity_id.ToString()) ?? new();

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
    /// Saves the specified receipt item and its associated data to the database.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptProfitDetailedItem"/> object containing receipt details, products, client information, 
    /// and associated entities. The object must not be <see langword="null"/>, must contain at least one product,  and
    /// the associated entity must have a valid <c>entity_id</c>.</param>
    /// <returns>An integer representing the result of the save operation. Returns <c>0</c> if the input is invalid;  otherwise,
    /// returns the identifier of the saved receipt.</returns>
    public int saveItem(ReceiptProfitDetailedItem item) {

        if (item == null 
            || item.receipt_products.Count() < 1 
            || item.entity.entity_id == 0) 
            {
            return 0;
        }

        int client_id = findClientIdOrCreateNew(item.entity.entity_id);
        item.receipt_client.fk_client_id = client_id;
        item.client.client_id = client_id;

        UpdateReceiptTotalPrice(item);

        var handlerItem = new ReceiptHandlerItem {
            receipt_item = item.receipt,
            receipt_product_items = item.receipt_products,
            receipt_client_item = item.receipt_client
        };

        return _m_receipt_handler_model.saveItem(handlerItem);
    }

    /// <summary>
    /// Retrieves the client ID associated with the specified entity ID, or creates a new client record if none exists.
    /// </summary>
    /// <param name="entity_id">The ID of the entity for which the client ID is being retrieved or created. Must be greater than 0.</param>
    /// <returns>The client ID associated with the specified entity ID. Returns 0 if the <paramref name="entity_id"/> is invalid
    /// or if the operation fails to create or retrieve a client record.</returns>
    private int findClientIdOrCreateNew(int entity_id) {

        if (entity_id <= 0) {
            return 0;
        }

        var client = _m_client_model.getClientWithEntityFK(entity_id.ToString());
        if (client != null && client.client_id > 0) {
            return client.client_id;
        }

        var new_client = new ClientItem {
            fk_entity_id = entity_id
        };
        _m_client_model.saveItem(new_client);

        client = _m_client_model.getClientWithEntityFK(entity_id.ToString());
        return client?.client_id ?? 0;
    }

    /// <summary>
    /// Updates the total price of the receipt associated with the specified item.
    /// </summary>
    /// <param name="item">The detailed receipt item containing the list of products and their quantities and unit prices.</param>
    private void UpdateReceiptTotalPrice(ReceiptProfitDetailedItem item) {

        if (item == null 
            || item.receipt_products == null 
            || item.receipt_products.Count == 0)
            {
            return;
        }

        decimal total = 0;
        foreach (var product in item.receipt_products) {
            total += product.receipt_product_quantity * product.receipt_product_unity_price;
        }
        item.receipt.receipt_total_price = total;
    }
}

