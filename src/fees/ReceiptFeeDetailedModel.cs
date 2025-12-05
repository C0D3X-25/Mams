using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.receipts;
using Mams.src.suppliers;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

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

    // Table names
    private const string _m_TBL_RECEIPTS = "receipts";
    private const string _m_TBL_RECEIPTS_CLIENTS = "receipts_clients";
    private const string _m_TBL_RECEIPTS_SUPPLIERS = "receipts_suppliers";
    private const string _m_TBL_RECEIPTS_PRODUCTS = "receipts_products";
    private const string _m_TBL_SUPPLIERS = "suppliers";
    private const string _m_TBL_ENTITIES = "entities";
    private const string _m_TBL_PRODUCTS = "products";
    private const string _m_TBL_PRODUCTS_TYPES = "products_types";
    private const string _m_TBL_PRODUCTS_CATEGORIES = "products_categories";
    private const string _m_TBL_PRODUCTS_SHAPES = "products_shapes";

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
    public ReceiptFeeDetailedItem? getItemByID(string id)
    {
        if (!SDataValidation.isIdValid(id))
        {
            return null;
        }

        return executeWithConnection<ReceiptFeeDetailedItem?>(connection =>
        {
            try
            {
                // Single query to get all receipt data with products
                string query = $@"
                    SELECT 
                        r.receipt_id,
                        r.receipt_number,
                        r.receipt_total_price,
                        r.receipt_date_created,
                        rs.fk_supplier_id,
                        rs.fk_receipt_id AS rs_fk_receipt_id,
                        s.supplier_id,
                        s.fk_entity_id,
                        e.entity_id,
                        e.entity_name,
                        e.entity_phone,
                        e.entity_email,
                        e.entity_city,
                        e.entity_address,
                        e.entity_archive,
                        rp.receipt_product_id,
                        rp.receipt_product_quantity,
                        rp.receipt_product_unity_price,
                        rp.fk_receipt_id AS rp_fk_receipt_id,
                        p.product_id,
                        p.product_name,
                        p.product_weight,
                        p.product_archive,
                        p.fk_product_type_id,
                        p.fk_product_category_id,
                        p.fk_product_shape_id,
                        pt.product_type_name,
                        pc.product_category_name,
                        ps.product_shape_name
                    FROM {_m_TBL_RECEIPTS} r
                    INNER JOIN {_m_TBL_RECEIPTS_SUPPLIERS} rs ON r.receipt_id = rs.fk_receipt_id
                    INNER JOIN {_m_TBL_SUPPLIERS} s ON rs.fk_supplier_id = s.supplier_id
                    INNER JOIN {_m_TBL_ENTITIES} e ON s.fk_entity_id = e.entity_id
                    LEFT JOIN {_m_TBL_RECEIPTS_CLIENTS} rc ON r.receipt_id = rc.fk_receipt_id
                    LEFT JOIN {_m_TBL_RECEIPTS_PRODUCTS} rp ON r.receipt_id = rp.fk_receipt_id
                    LEFT JOIN {_m_TBL_PRODUCTS} p ON rp.fk_product_id = p.product_id
                    LEFT JOIN {_m_TBL_PRODUCTS_TYPES} pt ON p.fk_product_type_id = pt.product_type_id
                    LEFT JOIN {_m_TBL_PRODUCTS_CATEGORIES} pc ON p.fk_product_category_id = pc.product_category_id
                    LEFT JOIN {_m_TBL_PRODUCTS_SHAPES} ps ON p.fk_product_shape_id = ps.product_shape_id
                    WHERE r.receipt_id = @id
                    AND rc.fk_client_id IS NULL
                    ORDER BY rp.receipt_product_id;";

                using MySqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@id", id);

                using MySqlDataReader reader = cmd.ExecuteReader();

                ReceiptFeeDetailedItem? item = null;

                while (reader.Read())
                {
                    // First row: create the item with receipt, supplier, entity data
                    item ??= new ReceiptFeeDetailedItem
                    {
                        receipt = new ReceiptItem
                        {
                            receipt_id = reader.getSafeValue<int>("receipt_id"),
                            receipt_number = reader.getSafeValue("receipt_number", string.Empty),
                            receipt_total_price = reader.getSafeValue<decimal>("receipt_total_price"),
                            receipt_date_created = reader.getSafeValue("receipt_date_created", DateOnly.MinValue).ToString(globals.SGlobals.g_EU_DATE_FORMAT)
                        },
                        receipt_supplier = new ReceiptSupplierItem
                        {
                            fk_supplier_id = reader.getSafeValue<int>("fk_supplier_id"),
                            fk_receipt_id = reader.getSafeValue<int>("rs_fk_receipt_id")
                        },
                        supplier = new SupplierItem
                        {
                            supplier_id = reader.getSafeValue<int>("supplier_id"),
                            fk_entity_id = reader.getSafeValue<int>("fk_entity_id")
                        },
                        entity = new EntityItem
                        {
                            entity_id = reader.getSafeValue<int>("entity_id"),
                            entity_name = reader.getSafeValue("entity_name", string.Empty),
                            entity_phone = reader.getSafeValue("entity_phone", string.Empty),
                            entity_email = reader.getSafeValue("entity_email", string.Empty),
                            entity_city = reader.getSafeValue("entity_city", string.Empty),
                            entity_address = reader.getSafeValue("entity_address", string.Empty),
                            entity_archive = reader.getSafeValue("entity_archive", DateOnly.MinValue).ToString()
                        },
                        receipt_products = []
                    };

                    // Add product if present (receipt_product_id is not null)
                    if (!reader.IsDBNull(reader.GetOrdinal("receipt_product_id")))
                    {
                        item.receipt_products.Add(new ReceiptProductItem
                        {
                            receipt_product_id = reader.getSafeValue<int>("receipt_product_id"),
                            receipt_product_quantity = reader.getSafeValue<int>("receipt_product_quantity"),
                            receipt_product_unity_price = reader.getSafeValue<decimal>("receipt_product_unity_price"),
                            fk_receipt_id = reader.getSafeValue<int>("rp_fk_receipt_id"),
                            product_item = new ProductItem
                            {
                                product_id = reader.getSafeValue<int>("product_id"),
                                product_name = reader.getSafeValue("product_name", string.Empty),
                                product_weight = reader.getSafeValue<int>("product_weight"),
                                product_archive = reader.getSafeValue("product_archive", DateOnly.MinValue).ToString(),
                                fk_product_type_id = reader.getSafeValue<int>("fk_product_type_id"),
                                fk_product_category_id = reader.getSafeValue<int>("fk_product_category_id"),
                                fk_product_shape_id = reader.getSafeValue<int>("fk_product_shape_id"),
                                product_type_name = reader.getSafeValue("product_type_name", string.Empty),
                                product_category_name = reader.getSafeValue("product_category_name", string.Empty),
                                product_shape_name = reader.getSafeValue("product_shape_name", string.Empty)
                            }
                        });
                    }
                }

                return item;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return null;
            }
        });
    }

    /// <summary>
    /// Retrieves a collection of detailed receipt fee items, including associated supplier, entity, and product
    /// information.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{ReceiptFeeDetailedItem}"/> of <see cref="ReceiptFeeDetailedItem"/> objects, where each item
    /// contains detailed information about a receipt, its associated products, supplier, and entity. The collection
    /// will be empty if no valid receipts are found.</returns>
    public ObservableCollection<ReceiptFeeDetailedItem> getTable()
    {
        return executeWithConnection(connection =>
        {
            var items = new ObservableCollection<ReceiptFeeDetailedItem>();

            try
            {
                // Single query to get all receipt data with products (excluding client receipts)
                string query = $@"
                    SELECT 
                        r.receipt_id,
                        r.receipt_number,
                        r.receipt_total_price,
                        r.receipt_date_created,
                        rs.fk_supplier_id,
                        rs.fk_receipt_id AS rs_fk_receipt_id,
                        s.supplier_id,
                        s.fk_entity_id,
                        e.entity_id,
                        e.entity_name,
                        e.entity_phone,
                        e.entity_email,
                        e.entity_city,
                        e.entity_address,
                        e.entity_archive,
                        rp.receipt_product_id,
                        rp.receipt_product_quantity,
                        rp.receipt_product_unity_price,
                        rp.fk_receipt_id AS rp_fk_receipt_id,
                        p.product_id,
                        p.product_name,
                        p.product_weight,
                        p.product_archive,
                        p.fk_product_type_id,
                        p.fk_product_category_id,
                        p.fk_product_shape_id,
                        pt.product_type_name,
                        pc.product_category_name,
                        ps.product_shape_name
                    FROM {_m_TBL_RECEIPTS} r
                    INNER JOIN {_m_TBL_RECEIPTS_SUPPLIERS} rs ON r.receipt_id = rs.fk_receipt_id
                    INNER JOIN {_m_TBL_SUPPLIERS} s ON rs.fk_supplier_id = s.supplier_id
                    INNER JOIN {_m_TBL_ENTITIES} e ON s.fk_entity_id = e.entity_id
                    LEFT JOIN {_m_TBL_RECEIPTS_CLIENTS} rc ON r.receipt_id = rc.fk_receipt_id
                    LEFT JOIN {_m_TBL_RECEIPTS_PRODUCTS} rp ON r.receipt_id = rp.fk_receipt_id
                    LEFT JOIN {_m_TBL_PRODUCTS} p ON rp.fk_product_id = p.product_id
                    LEFT JOIN {_m_TBL_PRODUCTS_TYPES} pt ON p.fk_product_type_id = pt.product_type_id
                    LEFT JOIN {_m_TBL_PRODUCTS_CATEGORIES} pc ON p.fk_product_category_id = pc.product_category_id
                    LEFT JOIN {_m_TBL_PRODUCTS_SHAPES} ps ON p.fk_product_shape_id = ps.product_shape_id
                    WHERE rc.fk_client_id IS NULL
                    ORDER BY r.receipt_id, rp.receipt_product_id;";

                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                // Dictionary to track receipts by ID for grouping products
                var receiptDict = new Dictionary<int, ReceiptFeeDetailedItem>();

                while (reader.Read())
                {
                    int receiptId = reader.getSafeValue<int>("receipt_id");

                    // Create new receipt item if not already tracked
                    if (!receiptDict.TryGetValue(receiptId, out var item))
                    {
                        item = new ReceiptFeeDetailedItem
                        {
                            receipt = new ReceiptItem
                            {
                                receipt_id = receiptId,
                                receipt_number = reader.getSafeValue("receipt_number", string.Empty),
                                receipt_total_price = reader.getSafeValue<decimal>("receipt_total_price"),
                                receipt_date_created = reader.getSafeValue("receipt_date_created", DateOnly.MinValue).ToString(globals.SGlobals.g_EU_DATE_FORMAT)
                            },
                            receipt_supplier = new ReceiptSupplierItem
                            {
                                fk_supplier_id = reader.getSafeValue<int>("fk_supplier_id"),
                                fk_receipt_id = reader.getSafeValue<int>("rs_fk_receipt_id")
                            },
                            supplier = new SupplierItem
                            {
                                supplier_id = reader.getSafeValue<int>("supplier_id"),
                                fk_entity_id = reader.getSafeValue<int>("fk_entity_id")
                            },
                            entity = new EntityItem
                            {
                                entity_id = reader.getSafeValue<int>("entity_id"),
                                entity_name = reader.getSafeValue("entity_name", string.Empty),
                                entity_phone = reader.getSafeValue("entity_phone", string.Empty),
                                entity_email = reader.getSafeValue("entity_email", string.Empty),
                                entity_city = reader.getSafeValue("entity_city", string.Empty),
                                entity_address = reader.getSafeValue("entity_address", string.Empty),
                                entity_archive = reader.getSafeValue("entity_archive", DateOnly.MinValue).ToString()
                            },
                            receipt_products = []
                        };

                        receiptDict[receiptId] = item;
                        items.Add(item);
                    }

                    // Add product if present (receipt_product_id is not null)
                    if (!reader.IsDBNull(reader.GetOrdinal("receipt_product_id")))
                    {
                        item.receipt_products.Add(new ReceiptProductItem
                        {
                            receipt_product_id = reader.getSafeValue<int>("receipt_product_id"),
                            receipt_product_quantity = reader.getSafeValue<int>("receipt_product_quantity"),
                            receipt_product_unity_price = reader.getSafeValue<decimal>("receipt_product_unity_price"),
                            fk_receipt_id = reader.getSafeValue<int>("rp_fk_receipt_id"),
                            product_item = new ProductItem
                            {
                                product_id = reader.getSafeValue<int>("product_id"),
                                product_name = reader.getSafeValue("product_name", string.Empty),
                                product_weight = reader.getSafeValue<int>("product_weight"),
                                product_archive = reader.getSafeValue("product_archive", DateOnly.MinValue).ToString(),
                                fk_product_type_id = reader.getSafeValue<int>("fk_product_type_id"),
                                fk_product_category_id = reader.getSafeValue<int>("fk_product_category_id"),
                                fk_product_shape_id = reader.getSafeValue<int>("fk_product_shape_id"),
                                product_type_name = reader.getSafeValue("product_type_name", string.Empty),
                                product_category_name = reader.getSafeValue("product_category_name", string.Empty),
                                product_shape_name = reader.getSafeValue("product_shape_name", string.Empty)
                            }
                        });
                    }
                }

                return items;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return items;
            }
        });
    }

    /// <summary>
    /// Saves the specified receipt item and its associated details to the database.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptFeeDetailedItem"/> object containing the receipt, products, supplier, and entity details.
    /// The object must not be <see langword="null"/>, must contain at least one product, and the entity ID must be
    /// greater than zero.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the result of the save operation and any error message.
    /// Returns a response with ID 0 if the input is invalid or the save operation fails.</returns>
    public ResponseSaveItem saveItem(ReceiptFeeDetailedItem item) {
        if (item == null || item.receipt_products.Count < 1 || item.entity.entity_id == 0) {
            return ResponseSaveItem.Failure("Invalid fee receipt data.");
        }

        startTransaction();

        try
        {
            int supplier_id = findSupplierIdOrCreateNew(item.entity.entity_id);
            if (supplier_id == 0) {
                throw new InvalidOperationException("Failed to find or create supplier for the given entity.");
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

            var result = _m_receipt_handler_model.saveItem(handlerItem);
            if (!result.is_success)
            {
                throw new InvalidOperationException(result.error_message ?? "Failed to save the receipt.");
            }

            commitTransaction();
            return result;
        }
        catch (Exception ex)
        {
            rollbackTransaction();
            return ResponseSaveItem.Failure(ex.Message);
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
        var result = _m_supplier_model.saveItem(new_supplier);
        return result.returned_id;
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
