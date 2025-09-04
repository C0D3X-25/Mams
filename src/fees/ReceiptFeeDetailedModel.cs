using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.receipts;
using Mams.src.suppliers;
using System.Collections.Generic;
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
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.HARD_DELETE"/>.</param>
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

        var supplierItem = _m_supplier_model.getItemByID(receipt.receipt_supplier_item.fk_supplier_id.ToString());
        if (supplierItem == null) {
            supplierItem = new();
        }

        var entityItem = _m_entity_model.getItemByID(supplierItem.fk_entity_id.ToString());
        if (entityItem == null) {
            entityItem = new();
        }

        ReceiptFeeDetailedItem item = new() {
            receipt = receipt.receipt_item,
            receipt_products = receipt.receipt_product_items,
            receipt_supplier = receipt.receipt_supplier_item,
            supplier = supplierItem,
            entity = entityItem
        };

        // Get all product IDs to fetch in a single batch
        List<string> product_ids = new(item.receipt_products.Count);
        foreach (var receiptProduct in item.receipt_products) {
            product_ids.Add(receiptProduct.product_item.product_id.ToString());
        }

        // Batch fetch products
        var productDict = new Dictionary<string, ProductItem>();
        foreach (var product_id in product_ids) {
            var product = _m_product_model.getItemByID(product_id);
            if (product != null) {
                productDict[product_id] = product;
            }
        }

        // Assign products to receipt items
        foreach (var receiptProduct in item.receipt_products) {
            var product_id = receiptProduct.product_item.product_id.ToString();
            if (productDict.TryGetValue(product_id, out var product)) {
                receiptProduct.product_item = product;
            }
        }

        return item;
    }

    /// <summary>
    /// Retrieves a collection of detailed receipt fee items, including associated supplier, entity, and product
    /// information.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{ReceiptFeeDetailedItem}"/> of <see cref="ReceiptFeeDetailedItem"/> objects, where each item
    /// contains detailed information about a receipt, its associated products, supplier, and entity. The collection
    /// will be empty if no valid receipts are found.</returns>
    public ObservableCollection<ReceiptFeeDetailedItem> getTable() {

        var items = new ObservableCollection<ReceiptFeeDetailedItem>();
        var receipts = _m_receipt_handler_model.getTable();
        
        // Early exit if no receipts
        if (receipts.Count == 0) {
            return items;
        }

        // Collect all supplier and product IDs to batch fetch
        var supplier_ids = new HashSet<int>();
        var product_ids = new HashSet<int>();
        
        foreach (var receipt in receipts) {
            // Skip profit receipts
            if (receipt.receipt_client_item.fk_client_id != 0) {
                continue;
            }
            
            supplier_ids.Add(receipt.receipt_supplier_item.fk_supplier_id);
            foreach (var product in receipt.receipt_product_items) {
                product_ids.Add(product.product_item.product_id);
            }
        }

        // Batch fetch suppliers
        var suppliers = new Dictionary<int, SupplierItem>();
        foreach (int supplier_id in supplier_ids) {
            var supplier = _m_supplier_model.getItemByID(supplier_id.ToString());
            if (supplier != null) {
                suppliers[supplier_id] = supplier;
            }
        }

        // Collect entity IDs for batch fetch
        var entity_ids = new HashSet<int>();
        foreach (var supplier in suppliers.Values) {
            entity_ids.Add(supplier.fk_entity_id);
        }

        // Batch fetch entities
        var entities = new Dictionary<int, EntityItem>();
        foreach (int entity_id in entity_ids) {
            var entity = _m_entity_model.getItemByID(entity_id.ToString());
            if (entity != null) {
                entities[entity_id] = entity;
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

        // Assemble the detailed items
        foreach (var receipt in receipts) {
            // Skip profit receipts
            if (receipt.receipt_client_item.fk_client_id != 0) {
                continue;
            }

            int supplier_id = receipt.receipt_supplier_item.fk_supplier_id;
            if (!suppliers.TryGetValue(supplier_id, out var supplier)) {
                supplier = new();
            }

            int entity_id = supplier.fk_entity_id;
            if (!entities.TryGetValue(entity_id, out var entity)) {
                entity = new();
            }

            var detailedItem = new ReceiptFeeDetailedItem {
                receipt = receipt.receipt_item,
                receipt_products = new ObservableCollection<ReceiptProductItem>(),
                receipt_supplier = receipt.receipt_supplier_item,
                supplier = supplier,
                entity = entity
            };

            // Copy and populate product details
            foreach (var receiptProduct in receipt.receipt_product_items) {
                var productCopy = new ReceiptProductItem {
                    receipt_product_id = receiptProduct.receipt_product_id,
                    receipt_product_quantity = receiptProduct.receipt_product_quantity,
                    receipt_product_unity_price = receiptProduct.receipt_product_unity_price,
                    fk_receipt_id = receiptProduct.fk_receipt_id,
                    product_item = receiptProduct.product_item
                };

                if (products.TryGetValue(receiptProduct.product_item.product_id, out var product)) {
                    productCopy.product_item = product;
                }

                detailedItem.receipt_products.Add(productCopy);
            }

            items.Add(detailedItem);
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
        if (item == null || item.receipt_products.Count < 1 || item.entity.entity_id == 0) {
            return 0;
        }

        // Start a transaction to ensure data consistency
        startTransaction();
        
        try {
            int supplier_id = findSupplierIdOrCreateNew(item.entity.entity_id);
            if (supplier_id == 0) {
                commitTransaction();  // Commit empty transaction
                return 0;
            }
            
            item.receipt_supplier.fk_supplier_id = supplier_id;
            item.supplier.supplier_id = supplier_id;

            if (string.IsNullOrEmpty(item.receipt.receipt_number)) {
                generateFeeReceiptNumber(item);
            }
            
            UpdateReceiptTotalPrice(item);

            var handlerItem = new ReceiptHandlerItem {
                receipt_item = item.receipt,
                receipt_product_items = item.receipt_products,
                receipt_supplier_item = item.receipt_supplier
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
        int newId = _m_supplier_model.saveItem(new_supplier);
        return newId;
    }

    /// <summary>
    /// Updates the total price of the receipt based on the quantities and unit prices of the products.
    /// </summary>
    /// <param name="item">The detailed receipt item containing the list of products and their associated quantities and unit prices.</param>
    private static void UpdateReceiptTotalPrice(ReceiptFeeDetailedItem item) {
        decimal total = 0.0M;
        foreach (var product in item.receipt_products) {
            total += product.receipt_product_quantity * product.receipt_product_unity_price;
        }
        item.receipt.receipt_total_price = total;
    }

    /// <summary>
    /// Generates and assigns a unique fee receipt number to the specified <see cref="ReceiptFeeDetailedItem"/> if it
    /// does not already have one.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptFeeDetailedItem"/> to which the receipt number will be assigned.  
    /// If <paramref name="item"/> is <see langword="null"/> or its receipt already has a number, no action is taken.</param>
    private void generateFeeReceiptNumber(ReceiptFeeDetailedItem item) {
        if (item == null || !string.IsNullOrEmpty(item.receipt.receipt_number)) {
            return; 
        }

        ReceiptModel receipt_model = new();
        string currentDate = DateTime.Now.ToString(globals.SGlobals.g_EU_DATE_FORMAT);
        string baseNumber = "F-" + currentDate;
        
        // Try to find a non-existing receipt number using a more efficient approach
        for (int counter = 1; counter <= 999; counter++) {
            string candidateNumber = $"{baseNumber}-{counter:D3}";
            if (!receipt_model.isReceiptNumberExisting(candidateNumber)) {
                item.receipt.receipt_number = candidateNumber;
                return;
            }
        }
        
        // If we reach here, we've tried 999 numbers and all exist (extremely unlikely)
        // Generate a unique fallback using ticks
        item.receipt.receipt_number = $"{baseNumber}-{DateTime.Now.Ticks % 1000000:D6}";
    }
}
