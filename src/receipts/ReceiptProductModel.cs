using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.productsLots;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
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
    /// <returns><see langword="true"/> if the item was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_FK_RECEIPT, string.Empty, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ReceiptProductItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt product item to retrieve. Cannot be null or empty.</param>
    /// <returns>A <see cref="ReceiptProductItem"/> object representing the receipt product item with the specified identifier, 
    /// or <see langword="null"/> if no matching item is found.</returns>
    public ReceiptProductItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        ReceiptProductItem item = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_PRODUCT}, {_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT_LOT} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            {
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    item = readDataAndBuildItem(reader);
                }
            }

            // Complete the items with the product and product lot data
            completeData(item);

            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Retrieves a collection of receipt product items from the database.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="ReceiptProductItem"/> objects representing the receipt
    /// product items retrieved from the database. If no items are found, the collection will be empty.</returns>
    public ObservableCollection<ReceiptProductItem> getTable() {

        ObservableCollection<ReceiptProductItem> items = new();

        ProductModel product_model = new();
        ProductLotModel product_lot_model = new();
        ReceiptProductItem reicept_item = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_PRODUCT}, {_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT_LOT} " +
                $"FROM {_m_TBL_NAME};",
                m_conn
            );

            {
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {

                    //reicept_item.receipt_product_id = reader.getSafeValue<int>(_m_COL_ID);
                    //reicept_item.receipt_product_quantity = reader.getSafeValue<int>(_m_COL_QUANTITY);
                    //reicept_item.receipt_product_unity_price = reader.getSafeValue<decimal>(_m_COL_UNITY_PRICE);
                    //reicept_item.fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT);

                    //reicept_item.product_item.product_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT);
                    //reicept_item.product_lot_item.product_lot_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT_LOT);

                    //items.Add(reicept_item);
                    items.Add(readDataAndBuildItem(reader));
                }
            }

            // Complete the items with the product and product lot data
            foreach (var item in items) {
                completeData(item);
            }

            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }

    /// <summary>
    /// Saves a receipt product item to the database and returns the unique identifier of the saved item.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptProductItem"/> instance containing the details of the receipt product to be saved.</param>
    /// <returns>The unique identifier of the saved item if the operation is successful; otherwise, <see langword="0"/>.</returns>
    public int saveItem(ReceiptProductItem item) {

        if (!ValidateReceiptProduct(item)) { 
            return 0;
        }
        
        // Check if the item already exists in the database, if not insert the item into the database
        string query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT}, {_m_COL_FK_PRODUCT_LOT}) " +
                $"VALUES (@quantity, @unity_price, @fk_receipt, @fk_product, @fk_product_lot); " +
                $"SELECT LAST_INSERT_ID();";

        bool transaction_needed = false;
        if (!isTransactionActive()) {
            startTransaction();
            transaction_needed = true;
        }

        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);
            cmd.Parameters.AddWithValue("@quantity", item.receipt_product_quantity);
            cmd.Parameters.AddWithValue("@unity_price", item.receipt_product_unity_price);
            cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            cmd.Parameters.AddWithValue("@fk_product", item.product_item.product_id);
            if (item.product_lot_item.product_lot_id == 0) {
                item.product_lot_item.product_lot_id = _m_DEFAULT_FK_PRODUCT_LOT;
            }
            cmd.Parameters.AddWithValue("@fk_product_lot", item.product_lot_item.product_lot_id);

            int item_id = Convert.ToInt32(cmd.ExecuteScalar());

            if (transaction_needed) {
                commitTransaction();
            }
            return item_id;
        }
        catch (MySqlException ex) {
            if (transaction_needed) {
                rollbackTransaction();
            }
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
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

        ObservableCollection<ReceiptProductItem> items = new();

        if (!SDataValidation.isIdValid(fk_receipt)) {
            return items;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_PRODUCT}, {_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT_LOT} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_RECEIPT} = @fk_receipt;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@fk_receipt", fk_receipt);
            {
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    items.Add(readDataAndBuildItem(reader));
                }
            }

            // Complete the items with the product and product lot data
            foreach (var item in items) {
                completeData(item);
            }

            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
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
    /// Retreve data from the MySQL reader and convert it to a <see cref="ReceiptProductItem"/>.
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


    private ReceiptProductItem completeData(ReceiptProductItem item) {
        ProductModel product_model = new();
        ProductLotModel product_lot_model = new();

        item.product_item = product_model.getItemByID(item.product_item.product_id.ToString()) ?? new ProductItem();
        item.product_lot_item = product_lot_model.getItemByID(item.product_lot_item.product_lot_id.ToString()) ?? new ProductLotItem();

        return item;
    }
}
