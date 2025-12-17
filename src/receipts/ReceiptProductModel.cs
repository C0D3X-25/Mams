using Mams.src.databaseOperations;
using Mams.src.errors;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.productsLots;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

/// <summary>
/// Represents a model for managing receipt product data.
/// </summary>
/// <remarks>One instance is one line in the receipt.</remarks>
public class ReceiptProductModel : ABaseModel,
    ICrudOperation<ReceiptProductItem> {

    private const string _m_TBL_NAME = "receipts_products";
    private const string _m_COL_ID = "receipt_product_id";
    private const string _m_COL_QUANTITY = "receipt_product_quantity";
    private const string _m_COL_UNITY_PRICE = "receipt_product_unity_price";
    private const string _m_COL_FK_PRODUCT = "fk_product_id";
    private const string _m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string _m_COL_FK_PRODUCT_LOT = "fk_product_lot_id";

    // This default data are directly inserted in the database when she is created.
    // They are used because a FK can't be null, so we need to have a default value.
    private const int       _m_DEFAULT_FK_PRODUCT_LOT = 1;
    private const string    _m_DEFAULT_ARCHIVE = "1901-01-01";

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_FK_RECEIPT, string.Empty, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ReceiptProductItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt product item to retrieve. Cannot be null or empty.</param>
    /// <returns>A <see cref="ResponseGetItem{ReceiptProductItem}"/> containing the receipt product item and any error message.</returns>
    public ResponseGetItem<ReceiptProductItem> getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return ResponseGetItem<ReceiptProductItem>.Failure(EErrors.INVALID_INPUT,
                $"ReceiptProductModel.getItemByID: Invalid ID provided '{id}'");
        }

        return executeWithConnection(connection => {
            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                    $"{_m_COL_FK_PRODUCT}, {_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT_LOT} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = @id;",
                    connection
                );

                cmd.Parameters.AddWithValue("@id", id);
                
                ReceiptProductItem? item = null;
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    item = readDataAndBuildItem(reader);
                }

                if (item != null) {
                    // Complete the item with the product and product lot data
                    completeData(item);
                    return ResponseGetItem<ReceiptProductItem>.Success(item);
                }
                
                return ResponseGetItem<ReceiptProductItem>.NotFound();
            }
            catch (MySqlException ex) {
                return ResponseGetItem<ReceiptProductItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all receipt product items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{ReceiptProductItem}"/> containing all receipt product items and any error message.</returns>
    public ResponseGetAllItems<ReceiptProductItem> getAllItems() {
        return executeWithConnection(connection => {
            ObservableCollection<ReceiptProductItem> items = new();

            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                    $"{_m_COL_FK_PRODUCT}, {_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT_LOT} " +
                    $"FROM {_m_TBL_NAME};",
                    connection
                );

                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    items.Add(readDataAndBuildItem(reader));
                }

                // Complete the items with the product and product lot data
                foreach (var item in items) {
                    completeData(item);
                }
                return ResponseGetAllItems<ReceiptProductItem>.Success(items);
            }
            catch (MySqlException ex) {
                return ResponseGetAllItems<ReceiptProductItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Saves a receipt product item to the database and returns a response containing the result.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptProductItem"/> instance containing the details of the receipt product to be saved.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the unique identifier of the saved item and any error message.
    /// Returns a response with ID 0 if the operation fails.</returns>
    public ResponseSaveItem saveItem(ReceiptProductItem item) {
        if (!ValidateReceiptProduct(item)) { 
            return ResponseSaveItem.Failure(EErrors.INVALID_INPUT,
                $"ReceiptProductModel.saveItem: Invalid input - item is null or has invalid values (quantity: {item?.receipt_product_quantity}, price: {item?.receipt_product_unity_price}, receipt_id: {item?.fk_receipt_id}, product_id: {item?.product_item?.product_id})");
        }
        
        // Check if the item already exists in the database, if not insert the item into the database
        string query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT}, {_m_COL_FK_PRODUCT_LOT}) " +
                $"VALUES (@quantity, @unity_price, @fk_receipt, @fk_product, @fk_product_lot); " +
                $"SELECT LAST_INSERT_ID();";

        startTransaction();

        try {
            // Ensure product lot ID is valid
            if (item.product_lot_item.product_lot_id == 0) {
                item.product_lot_item.product_lot_id = _m_DEFAULT_FK_PRODUCT_LOT;
            }

            int item_id = executeWithConnection(connection => {
                using MySqlCommand cmd = new(query, connection, m_transaction);
                cmd.Parameters.AddWithValue("@quantity", item.receipt_product_quantity);
                cmd.Parameters.AddWithValue("@unity_price", item.receipt_product_unity_price);
                cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
                cmd.Parameters.AddWithValue("@fk_product", item.product_item.product_id);
                cmd.Parameters.AddWithValue("@fk_product_lot", item.product_lot_item.product_lot_id);

                return Convert.ToInt32(cmd.ExecuteScalar());
            });

            commitTransaction();
            return ResponseSaveItem.Success(item_id);
        }
        catch (MySqlException ex) {
            rollbackTransaction();
            return ResponseSaveItem.MySqlFailure(ex.ErrorCode, ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a collection of <see cref="ReceiptProductItem"/> objects associated with the specified receipt ID.
    /// </summary>
    /// <param name="fk_receipt">The foreign key identifier of the receipt. Must not be null or empty.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing <see cref="ReceiptProductItem"/> objects associated with the
    /// specified receipt ID. If the <paramref name="fk_receipt"/> is null or empty, or if no matching items are found,
    /// an empty collection is returned.</returns>
    public ObservableCollection<ReceiptProductItem> getListItemWithReceiptID(string fk_receipt) {
        if (!SDataValidation.isIdValid(fk_receipt)) {
            return new ObservableCollection<ReceiptProductItem>();
        }

        return executeWithConnection(connection => {
            ObservableCollection<ReceiptProductItem> items = new();

            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                    $"{_m_COL_FK_PRODUCT}, {_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT_LOT} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_FK_RECEIPT} = @fk_receipt;",
                    connection
                );

                cmd.Parameters.AddWithValue("@fk_receipt", fk_receipt);
                
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    items.Add(readDataAndBuildItem(reader));
                }

                // Complete the items with the product and product lot data
                foreach (var item in items) {
                    completeData(item);
                }
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            }

            return items;
        });
    }

    /// <summary>
    /// Validates the specified receipt product item to ensure it meets required conditions.
    /// </summary>
    /// <param name="item">The receipt product item to validate. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the receipt product item is valid; otherwise, <see langword="false"/>. A valid item
    /// must have a positive quantity, a non-negative unit price, a valid receipt ID, and a valid product ID.</returns>
    private bool ValidateReceiptProduct(ReceiptProductItem item) {
        return item != null
            && item.receipt_product_quantity > 0
            && item.receipt_product_unity_price >= 0
            && item.fk_receipt_id > 0
            && item.product_item.product_id > 0;
    }

    /// <summary>
    /// Retrieve data from the MySQL reader and convert it to a <see cref="ReceiptProductItem"/>.
    /// </summary>
    /// <remarks>Still need to get the data of the <see cref="ProductItem"/> and <see cref="ProductLotItem"/>.</remarks>
    /// <param name="reader"><see cref="MySqlDataReader"/>.</param>
    /// <returns><see cref="ReceiptProductItem"/>.</returns>
    private ReceiptProductItem readDataAndBuildItem(MySqlDataReader reader) {
        ReceiptProductItem receipt_item = new();
        
        receipt_item.receipt_product_id = reader.getSafeValue<int>(_m_COL_ID);
        receipt_item.receipt_product_quantity = reader.getSafeValue<int>(_m_COL_QUANTITY);
        receipt_item.receipt_product_unity_price = reader.getSafeValue<decimal>(_m_COL_UNITY_PRICE);
        receipt_item.fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT);

        receipt_item.product_item.product_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT);
        receipt_item.product_lot_item.product_lot_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT_LOT);

        return receipt_item;
    }

    /// <summary>
    /// Get the data of the <see cref="ProductItem"/> and <see cref="ProductLotItem"/> for the specified
    /// </summary>
    /// <param name="item">A <see cref="ReceiptProductItem"/>.</param>
    /// <returns>A <see cref="ReceiptProductItem"/>.</returns>
    private ReceiptProductItem completeData(ReceiptProductItem item) {
        ProductModel product_model = new();
        ProductLotModel product_lot_model = new();

        item.product_item = product_model.getItemByID(item.product_item.product_id.ToString()).returned_item ?? new ProductItem();
        item.product_lot_item = product_lot_model.getItemByID(item.product_lot_item.product_lot_id.ToString()).returned_item ?? new ProductLotItem();

        return item;
    }
}
