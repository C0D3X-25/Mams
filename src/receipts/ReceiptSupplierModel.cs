using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

/// <summary>
/// Represents a model for managing the relationship between receipts and suppliers in the database.
/// </summary>
/// <remarks>This class provides CRUD operations for the <c>receipts_suppliers</c> table, allowing interaction
/// with receipt-supplier associations. It includes methods to retrieve, save, delete, and query receipt-supplier
/// relationships. The model assumes valid database connections and handles common database operations.</remarks>
public class ReceiptSupplierModel : ABaseModel,
    ICrudOperation<ReceiptSupplierItem> {

    private const string _m_TBL_NAME = "receipts_suppliers";
    private const string _m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string _m_COL_FK_SUPPLIER = "fk_supplier_id";


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
    /// Retrieves a <see cref="ReceiptSupplierItem"/> object based on the specified receipt ID.
    /// </summary>
    /// <param name="id">The receipt ID to search for. Must be a valid ID as determined by the application's validation logic.</param>
    /// <returns>A <see cref="ReceiptSupplierItem"/> object containing the receipt and supplier information if the ID exists;
    /// otherwise, <see langword="null"/>.</returns>
    public ReceiptSupplierItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_RECEIPT} = @id;",
                m_conn
            );
            cmd.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {
                return new ReceiptSupplierItem {
                    fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.getSafeValue<int>(_m_COL_FK_SUPPLIER)
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Retrieves a collection of receipt-supplier items from the database.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing <see cref="ReceiptSupplierItem"/> objects representing the
    /// receipt-supplier relationships. The collection will be empty if no data is found or if an error occurs during
    /// the database operation.</returns>
    public ObservableCollection<ReceiptSupplierItem> getTable() {

        ObservableCollection<ReceiptSupplierItem> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER} " +
                $"FROM {_m_TBL_NAME};",
                m_conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptSupplierItem {
                    fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.getSafeValue<int>(_m_COL_FK_SUPPLIER)
                });
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }

    /// <summary>
    /// Saves a supplier receipt item to the database and returns the generated item ID.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptSupplierItem"/> object containing the supplier receipt data to be saved. The <paramref
    /// name="item"/> must not be <see langword="null"/> and its <c>fk_receipt_id</c> and <c>fk_supplier_id</c>
    /// properties must be non-zero.</param>
    /// <returns>The ID of the newly saved item if the operation succeeds; otherwise, <c>0</c>.</returns>
    public int saveItem(ReceiptSupplierItem item) {
        if (item == null 
            || item.fk_receipt_id == 0 
            || item.fk_supplier_id == 0) 
            {
            return 0;
        }

        string query = string.Empty;

        bool transaction_needed = false;
        if (!isTransactionActive()) {
            startTransaction();
            transaction_needed = true;
        }

        try {
            query = $"INSERT INTO {_m_TBL_NAME} " +
                $"({_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER}) " +
                $"VALUES (@fk_receipt, @fk_supplier); " +
                $"SELECT LAST_INSERT_ID();";

            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            cmd.Parameters.AddWithValue("@fk_supplier", item.fk_supplier_id);

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


    public ObservableCollection<ReceiptSupplierItem> getListItemWithSupplierID(string supplier_id) {

        ObservableCollection<ReceiptSupplierItem> items = new();

        if (!SDataValidation.isIdValid(supplier_id)) {
            return items;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_SUPPLIER} = @supplier_id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@supplier_id", supplier_id);

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptSupplierItem {
                    fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.getSafeValue<int>(_m_COL_FK_SUPPLIER)
                });
            }

            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }
}
