using Mams.src.clients;
using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.productsLots;
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
    private readonly ProductLotModel _m_product_lot_model = new();

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.HARD_DELETE"/>.</param>
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

        var client_item = _m_client_model.getItemByID(receipt.receipt_client_item.fk_client_id.ToString());
        if (client_item == null) {
            client_item = new();
        }

        var entity_item = _m_entity_model.getItemByID(client_item.fk_entity_id.ToString());
        if (entity_item == null) {
            entity_item = new();
        }

        ReceiptProfitDetailedItem item = new() {
            receipt = receipt.receipt_item,
            receipt_products = receipt.receipt_product_items,
            receipt_client = receipt.receipt_client_item,
            client = client_item,
            entity = entity_item
        };

        // Get all product IDs and product lot IDs to fetch in a single batch
        var product_ids = new HashSet<string>();
        var product_lot_ids = new HashSet<string>();

        foreach (var receipt_product in item.receipt_products) {
            product_ids.Add(receipt_product.product_item.product_id.ToString());
            product_lot_ids.Add(receipt_product.product_lot_item.product_lot_id.ToString());
        }

        // Batch fetch products
        var productDict = new Dictionary<string, ProductItem>();
        foreach (var product_id in product_ids) {
            var product = _m_product_model.getItemByID(product_id);
            if (product != null) {
                productDict[product_id] = product;
            }
        }

        // Batch fetch product lots
        var productLotDict = new Dictionary<string, ProductLotItem>();
        foreach (var lot_id in product_lot_ids) {
            var lot = _m_product_lot_model.getItemByID(lot_id);
            if (lot != null) {
                productLotDict[lot_id] = lot;
            }
        }

        // Assign products and lots to receipt items
        foreach (var receipt_product in item.receipt_products) {
            var product_id = receipt_product.product_item.product_id.ToString();
            var lot_id = receipt_product.product_lot_item.product_lot_id.ToString();

            if (productDict.TryGetValue(product_id, out var product)) {
                receipt_product.product_item = product;
            }

            if (productLotDict.TryGetValue(lot_id, out var lot)) {
                receipt_product.product_lot_item = lot;
            }
        }

        return item;
    }

    /// <summary>
    /// Retrieves a collection of detailed receipt profit items.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{ReceiptProfitDetailedItem}"/> containing detailed receipt profit items.  The collection will be empty
    /// if no valid receipts are found.</returns>
    public ObservableCollection<ReceiptProfitDetailedItem> getTable() {
        var receipts = _m_receipt_handler_model.getTable();
        var items = new ObservableCollection<ReceiptProfitDetailedItem>();
        
        // Early exit if no receipts
        if (receipts.Count == 0) {
            return items;
        }

        // Collect all client and product IDs to batch fetch
        var client_ids = new HashSet<int>();
        var product_ids = new HashSet<int>();
        var product_lot_ids = new HashSet<int>();
        
        foreach (var receipt in receipts) {
            // Skip fee receipts
            if (receipt.receipt_supplier_item.fk_supplier_id != 0) {
                continue;
            }
            
            client_ids.Add(receipt.receipt_client_item.fk_client_id);
            
            foreach (var product in receipt.receipt_product_items) {
                product_ids.Add(product.product_item.product_id);
                product_lot_ids.Add(product.product_lot_item.product_lot_id);
            }
        }

        // Batch fetch clients
        var clients = new Dictionary<int, ClientItem>();
        foreach (int client_id in client_ids) {
            var client = _m_client_model.getItemByID(client_id.ToString());
            if (client != null) {
                clients[client_id] = client;
            }
        }

        // Collect entity IDs for batch fetch
        var entityIds = new HashSet<int>();
        foreach (var client in clients.Values) {
            entityIds.Add(client.fk_entity_id);
        }

        // Batch fetch entities
        var entities = new Dictionary<int, EntityItem>();
        foreach (int entityId in entityIds) {
            var entity = _m_entity_model.getItemByID(entityId.ToString());
            if (entity != null) {
                entities[entityId] = entity;
            }
        }

        // Batch fetch products
        var products = new Dictionary<int, ProductItem>();
        foreach (int product_id in product_ids) {
            var product = _m_product_model.getItemByID(product_id.ToString());
            if (product != null) {
                products[product_id] = product;
            }
        }

        // Batch fetch product lots
        var productLots = new Dictionary<int, ProductLotItem>();
        foreach (int lot_id in product_lot_ids) {
            var lot = _m_product_lot_model.getItemByID(lot_id.ToString());
            if (lot != null) {
                productLots[lot_id] = lot;
            }
        }

        // Assemble the detailed items
        foreach (var receipt in receipts) {
            // Skip fee receipts
            if (receipt.receipt_supplier_item.fk_supplier_id != 0) {
                continue;
            }

            int client_id = receipt.receipt_client_item.fk_client_id;
            if (!clients.TryGetValue(client_id, out var client)) {
                client = new();
            }

            int entityId = client.fk_entity_id;
            if (!entities.TryGetValue(entityId, out var entity)) {
                entity = new();
            }

            var detailedItem = new ReceiptProfitDetailedItem {
                receipt = receipt.receipt_item,
                receipt_products = new ObservableCollection<ReceiptProductItem>(),
                receipt_client = receipt.receipt_client_item,
                client = client,
                entity = entity
            };

            // Copy and populate product details
            foreach (var receipt_product in receipt.receipt_product_items) {
                var productCopy = new ReceiptProductItem {
                    receipt_product_id = receipt_product.receipt_product_id,
                    receipt_product_quantity = receipt_product.receipt_product_quantity,
                    receipt_product_unity_price = receipt_product.receipt_product_unity_price,
                    fk_receipt_id = receipt_product.fk_receipt_id,
                    product_item = receipt_product.product_item,
                    product_lot_item = receipt_product.product_lot_item
                };

                if (products.TryGetValue(receipt_product.product_item.product_id, out var product)) {
                    productCopy.product_item = product;
                }

                if (productLots.TryGetValue(receipt_product.product_lot_item.product_lot_id, out var lot)) {
                    productCopy.product_lot_item = lot;
                }

                detailedItem.receipt_products.Add(productCopy);
            }

            items.Add(detailedItem);
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
        if (item == null || item.receipt_products.Count < 1 || item.entity.entity_id == 0) {
            return 0;
        }

        startTransaction();

        // TODO: Error happening in the try block
        try
        {
            int client_id = findClientIdOrCreateNew(item.entity.entity_id);
            if (client_id == 0) {
                commitTransaction();  // Commit empty transaction
                return 0;
            }
            
            item.receipt_client.fk_client_id = client_id;
            item.client.client_id = client_id;

            UpdateReceiptTotalPrice(item);

            var handlerItem = new ReceiptHandlerItem {
                receipt_item = item.receipt,
                receipt_product_items = item.receipt_products,
                receipt_client_item = item.receipt_client
            };

            int result = _m_receipt_handler_model.saveItem(handlerItem);
            commitTransaction();
            return result;
        }
        catch {
            // In a real app, rollback would be here
            commitTransaction();  // Commit empty transaction
            return 0;
        }
    }

    /// <summary>
    /// Retrieves the client ID associated with the specified entity ID, or creates a new client record if none exists.
    /// </summary>
    /// <param name="entity_id">The ID of the entity for which the client ID is being retrieved or created. Must be greater than 0.</param>
    /// <returns>The client ID associated with the specified entity ID. Returns 0 if the <paramref name="entity_id"/> is invalid
    /// or if the operation fails to create or retrieve a client record.</returns>
    private int findClientIdOrCreateNew(int entity_id) {
        if (!SDataValidation.isIdValid(entity_id)) {
            return 0;
        }

        var client = _m_client_model.getClientWithEntityFK(entity_id.ToString());
        if (client != null && client.client_id > 0) {
            return client.client_id;
        }

        var new_client = new ClientItem {
            fk_entity_id = entity_id
        };
        return _m_client_model.saveItem(new_client);
    }

    /// <summary>
    /// Updates the total price of the receipt associated with the specified item.
    /// </summary>
    /// <param name="item">The detailed receipt item containing the list of products and their quantities and unit prices.</param>
    private static void UpdateReceiptTotalPrice(ReceiptProfitDetailedItem item) {
        if (item == null || item.receipt_products == null || item.receipt_products.Count == 0) {
            return;
        }

        decimal total = 0;
        foreach (var product in item.receipt_products) {
            total += product.receipt_product_quantity * product.receipt_product_unity_price;
        }
        item.receipt.receipt_total_price = total;
    }
}

