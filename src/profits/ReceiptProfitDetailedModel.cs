using Mams.src.clients;
using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.productsLots;
using Mams.src.receipts;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

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

    // Table names
    private const string _m_TBL_RECEIPTS = "receipts";
    private const string _m_TBL_RECEIPTS_CLIENTS = "receipts_clients";
    private const string _m_TBL_RECEIPTS_SUPPLIERS = "receipts_suppliers";
    private const string _m_TBL_RECEIPTS_PRODUCTS = "receipts_products";
    private const string _m_TBL_CLIENTS = "clients";
    private const string _m_TBL_ENTITIES = "entities";
    private const string _m_TBL_PRODUCTS = "products";
    private const string _m_TBL_PRODUCTS_LOTS = "products_lots";
    private const string _m_TBL_PRODUCTS_TYPES = "products_types";
    private const string _m_TBL_PRODUCTS_CATEGORIES = "products_categories";
    private const string _m_TBL_PRODUCTS_SHAPES = "products_shapes";
    private const string _m_TBL_BEEHIVES = "beehives";

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
    public ReceiptProfitDetailedItem? getItemByID(string id)
    {
        if (!SDataValidation.isIdValid(id))
        {
            return null;
        }

        return executeWithConnection<ReceiptProfitDetailedItem?>(connection =>
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
                        rc.fk_client_id,
                        rc.fk_receipt_id AS rc_fk_receipt_id,
                        c.client_id,
                        c.fk_entity_id,
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
                        ps.product_shape_name,
                        pl.product_lot_id,
                        pl.product_lot_name,
                        pl.product_lot_year,
                        pl.fk_beehive_id,
                        pl.product_lot_archive,
                        b.beehive_name
                    FROM {_m_TBL_RECEIPTS} r
                    INNER JOIN {_m_TBL_RECEIPTS_CLIENTS} rc ON r.receipt_id = rc.fk_receipt_id
                    INNER JOIN {_m_TBL_CLIENTS} c ON rc.fk_client_id = c.client_id
                    INNER JOIN {_m_TBL_ENTITIES} e ON c.fk_entity_id = e.entity_id
                    LEFT JOIN {_m_TBL_RECEIPTS_SUPPLIERS} rs ON r.receipt_id = rs.fk_receipt_id
                    LEFT JOIN {_m_TBL_RECEIPTS_PRODUCTS} rp ON r.receipt_id = rp.fk_receipt_id
                    LEFT JOIN {_m_TBL_PRODUCTS} p ON rp.fk_product_id = p.product_id
                    LEFT JOIN {_m_TBL_PRODUCTS_TYPES} pt ON p.fk_product_type_id = pt.product_type_id
                    LEFT JOIN {_m_TBL_PRODUCTS_CATEGORIES} pc ON p.fk_product_category_id = pc.product_category_id
                    LEFT JOIN {_m_TBL_PRODUCTS_SHAPES} ps ON p.fk_product_shape_id = ps.product_shape_id
                    LEFT JOIN {_m_TBL_PRODUCTS_LOTS} pl ON rp.fk_product_lot_id = pl.product_lot_id
                    LEFT JOIN {_m_TBL_BEEHIVES} b ON pl.fk_beehive_id = b.beehive_id
                    WHERE r.receipt_id = @id
                    AND rs.fk_supplier_id IS NULL
                    ORDER BY rp.receipt_product_id;";

                using MySqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@id", id);

                using MySqlDataReader reader = cmd.ExecuteReader();

                ReceiptProfitDetailedItem? item = null;

                while (reader.Read()) {
                    // First row: create the item with receipt, client, entity data
                    item ??= new ReceiptProfitDetailedItem 
                    {
                        receipt = new ReceiptItem 
                        {
                            receipt_id = reader.getSafeValue<int>("receipt_id"),
                            receipt_number = reader.getSafeValue("receipt_number", string.Empty),
                            receipt_total_price = reader.getSafeValue<decimal>("receipt_total_price"),
                            receipt_date_created = reader.getSafeValue("receipt_date_created", DateOnly.MinValue).ToString(globals.SGlobals.g_EU_DATE_FORMAT)
                        },
                        receipt_client = new ReceiptClientItem
                        {
                            fk_client_id = reader.getSafeValue<int>("fk_client_id"),
                            fk_receipt_id = reader.getSafeValue<int>("rc_fk_receipt_id")
                        },
                        client = new ClientItem 
                        {
                            client_id = reader.getSafeValue<int>("client_id"),
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
                            },
                            product_lot_item = new ProductLotItem
                            {
                                product_lot_id = reader.getSafeValue<int>("product_lot_id"),
                                product_lot_name = reader.getSafeValue("product_lot_name", string.Empty),
                                product_lot_year = reader.getSafeValue<int>("product_lot_year"),
                                fk_beehive_id = reader.getSafeValue<int>("fk_beehive_id"),
                                beehive_name = reader.getSafeValue("beehive_name", string.Empty),
                                product_lot_archive = reader.getSafeValue("product_lot_archive", DateOnly.MinValue).ToString()
                            }
                        });
                    }
                }

                return item;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return null;
            }
        });
    }

    /// <summary>
    /// Retrieves a collection of detailed receipt profit items.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{ReceiptProfitDetailedItem}"/> containing detailed receipt profit items.  The collection will be empty
    /// if no valid receipts are found.</returns>
    public ObservableCollection<ReceiptProfitDetailedItem> getTable() {
        return executeWithConnection(connection =>
        {
            var items = new ObservableCollection<ReceiptProfitDetailedItem>();

            try
            {
                // Single query to get all receipt data with products (excluding supplier receipts)
                string query = $@"
                    SELECT 
                        r.receipt_id,
                        r.receipt_number,
                        r.receipt_total_price,
                        r.receipt_date_created,
                        rc.fk_client_id,
                        rc.fk_receipt_id AS rc_fk_receipt_id,
                        c.client_id,
                        c.fk_entity_id,
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
                        ps.product_shape_name,
                        pl.product_lot_id,
                        pl.product_lot_name,
                        pl.product_lot_year,
                        pl.fk_beehive_id,
                        pl.product_lot_archive,
                        b.beehive_name
                    FROM {_m_TBL_RECEIPTS} r
                    INNER JOIN {_m_TBL_RECEIPTS_CLIENTS} rc ON r.receipt_id = rc.fk_receipt_id
                    INNER JOIN {_m_TBL_CLIENTS} c ON rc.fk_client_id = c.client_id
                    INNER JOIN {_m_TBL_ENTITIES} e ON c.fk_entity_id = e.entity_id
                    LEFT JOIN {_m_TBL_RECEIPTS_SUPPLIERS} rs ON r.receipt_id = rs.fk_receipt_id
                    LEFT JOIN {_m_TBL_RECEIPTS_PRODUCTS} rp ON r.receipt_id = rp.fk_receipt_id
                    LEFT JOIN {_m_TBL_PRODUCTS} p ON rp.fk_product_id = p.product_id
                    LEFT JOIN {_m_TBL_PRODUCTS_TYPES} pt ON p.fk_product_type_id = pt.product_type_id
                    LEFT JOIN {_m_TBL_PRODUCTS_CATEGORIES} pc ON p.fk_product_category_id = pc.product_category_id
                    LEFT JOIN {_m_TBL_PRODUCTS_SHAPES} ps ON p.fk_product_shape_id = ps.product_shape_id
                    LEFT JOIN {_m_TBL_PRODUCTS_LOTS} pl ON rp.fk_product_lot_id = pl.product_lot_id
                    LEFT JOIN {_m_TBL_BEEHIVES} b ON pl.fk_beehive_id = b.beehive_id
                    WHERE rs.fk_supplier_id IS NULL
                    ORDER BY r.receipt_id, rp.receipt_product_id;";

                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                // Dictionary to track receipts by ID for grouping products
                var receiptDict = new Dictionary<int, ReceiptProfitDetailedItem>();

                while (reader.Read())
                {
                    int receiptId = reader.getSafeValue<int>("receipt_id");

                    // Create new receipt item if not already tracked
                    if (!receiptDict.TryGetValue(receiptId, out var item))
                    {
                        item = new ReceiptProfitDetailedItem
                        {
                            receipt = new ReceiptItem
                            {
                                receipt_id = receiptId,
                                receipt_number = reader.getSafeValue("receipt_number", string.Empty),
                                receipt_total_price = reader.getSafeValue<decimal>("receipt_total_price"),
                                receipt_date_created = reader.getSafeValue("receipt_date_created", DateOnly.MinValue).ToString(globals.SGlobals.g_EU_DATE_FORMAT)
                            },
                            receipt_client = new ReceiptClientItem
                            {
                                fk_client_id = reader.getSafeValue<int>("fk_client_id"),
                                fk_receipt_id = reader.getSafeValue<int>("rc_fk_receipt_id")
                            },
                            client = new ClientItem
                            {
                                client_id = reader.getSafeValue<int>("client_id"),
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
                            },
                            product_lot_item = new ProductLotItem
                            {
                                product_lot_id = reader.getSafeValue<int>("product_lot_id"),
                                product_lot_name = reader.getSafeValue("product_lot_name", string.Empty),
                                product_lot_year = reader.getSafeValue<int>("product_lot_year"),
                                fk_beehive_id = reader.getSafeValue<int>("fk_beehive_id"),
                                beehive_name = reader.getSafeValue("beehive_name", string.Empty),
                                product_lot_archive = reader.getSafeValue("product_lot_archive", DateOnly.MinValue).ToString()
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
    /// Saves the specified receipt item and its associated data to the database.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptProfitDetailedItem"/> object containing receipt details, products, client information, 
    /// and associated entities. The object must not be <see langword="null"/>, must contain at least one product,  and
    /// the associated entity must have a valid <c>entity_id</c>.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the result of the save operation and any error message.
    /// Returns a response with ID 0 if the input is invalid or the operation fails.</returns>
    public ResponseSaveItem saveItem(ReceiptProfitDetailedItem item) {
        if (item == null || item.receipt_products.Count < 1 || item.entity.entity_id == 0) {
            return ResponseSaveItem.Failure("Invalid profit receipt data.");
        }

        startTransaction();

        try
        {
            int client_id = findClientIdOrCreateNew(item.entity.entity_id);
            if (client_id == 0) {
                throw new InvalidOperationException("Failed to find or create client for the given entity.");
            }
            
            item.receipt_client.fk_client_id = client_id;
            item.client.client_id = client_id;

            UpdateReceiptTotalPrice(item);

            var handlerItem = new ReceiptHandlerItem {
                receipt_item = item.receipt,
                receipt_product_items = item.receipt_products,
                receipt_client_item = item.receipt_client
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
        var result = _m_client_model.saveItem(new_client);
        return result.returned_id;
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

        