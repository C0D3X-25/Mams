using Mams.src.databaseOperations;
using Mams.src.errors;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace Mams.src.receipts;

/// <summary>
/// Represents a model for managing the relationship between receipts and suppliers in the database.
/// </summary>
/// <remarks>This class provides CRUD operations for the <c>receipts_suppliers</c> table, allowing interaction
/// with receipt-supplier associations. It includes methods to retrieve, save, delete, and query receipt-supplier
/// relationships. The model assumes valid database connections and handles common database operations.</remarks>
public class ReceiptSupplierModel : ABaseModel, ICrudOperation<ReceiptSupplierItem> {

    private const string m_TBL_NAME = "receipts_suppliers";
    private const string _m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string _m_COL_FK_SUPPLIER = "fk_supplier_id";

    private const string SELECT_COLUMNS = $"{_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER}";
    private const string BASE_SELECT_QUERY = $"SELECT {SELECT_COLUMNS} FROM {m_TBL_NAME}";

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_FK_RECEIPT, string.Empty, m_TBL_NAME, delete_type);
    }


    /// <summary>
    /// Retrieves a <see cref="ReceiptSupplierItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ResponseGetItem{ReceiptSupplierItem}"/> containing the receipt supplier item and any error message.</returns>
    public ResponseGetItem<ReceiptSupplierItem> getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return ResponseGetItem<ReceiptSupplierItem>.Failure(EErrors.INVALID_INPUT);
        }

        return executeWithConnection(connection => {
            try {
                using var cmd = new MySqlCommand($"{BASE_SELECT_QUERY} WHERE {_m_COL_FK_RECEIPT} = @id;", connection);
                cmd.Parameters.AddWithValue("@id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    return ResponseGetItem<ReceiptSupplierItem>.Success(CreateItemFromReader(reader));
                }
                return ResponseGetItem<ReceiptSupplierItem>.NotFound();
            }
            catch (MySqlException ex) {
                return ResponseGetItem<ReceiptSupplierItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all receipt supplier items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{ReceiptSupplierItem}"/> containing all receipt supplier items and any error message.</returns>
    public ResponseGetAllItems<ReceiptSupplierItem> getAllItems() {
        return executeWithConnection(connection => {
            var items = new ObservableCollection<ReceiptSupplierItem>();

            try {
                using var cmd = new MySqlCommand(BASE_SELECT_QUERY, connection);
                using var reader = cmd.ExecuteReader();

                while (reader.Read()) {
                    items.Add(CreateItemFromReader(reader));
                }
                return ResponseGetAllItems<ReceiptSupplierItem>.Success(items);
            }
            catch (MySqlException ex) {
                return ResponseGetAllItems<ReceiptSupplierItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Saves the specified <see cref="ReceiptSupplierItem"/> to the database.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptSupplierItem"/> to save. The item must not be <see langword="null"/>, and its
    /// <c>fk_receipt_id</c> and <c>fk_supplier_id</c> properties must be non-zero.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved item and any error message.
    /// Returns a response with ID 0 if the input is invalid or if an error occurs during the operation.</returns>
    public ResponseSaveItem saveItem(ReceiptSupplierItem item) {
        if (item == null || item.fk_receipt_id == 0 || item.fk_supplier_id == 0) {
            return ResponseSaveItem.Failure(EErrors.INVALID_INPUT);
        }

        int item_id = item.fk_receipt_id;

        startTransaction();

        try {
            string query = isIdenticItemPresentInTable(m_TBL_NAME, _m_COL_FK_RECEIPT, item_id.ToString())
                ? $"UPDATE {m_TBL_NAME} SET {_m_COL_FK_SUPPLIER} = @fk_supplier WHERE {_m_COL_FK_RECEIPT} = @fk_receipt; SELECT @fk_receipt;"
                : $"INSERT INTO {m_TBL_NAME} ({_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER}) VALUES (@fk_receipt, @fk_supplier); " +
                  $"SELECT LAST_INSERT_ID();";

            item_id = executeWithConnection(connection => {
                using var cmd = new MySqlCommand(query, connection, m_transaction);
                cmd.Parameters.AddWithValue("@fk_receipt", item_id);
                cmd.Parameters.AddWithValue("@fk_supplier", item.fk_supplier_id);

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
    /// Retrieves a collection of <see cref="ReceiptSupplierItem"/> objects associated with the specified supplier ID.
    /// </summary>
    /// <param name="supplier_id">The foreign key supplier ID used to filter the items. Must not be null or empty.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing the <see cref="ReceiptSupplierItem"/> objects associated with
    /// the specified supplier ID. If <paramref name="supplier_id"/> is null or empty, an empty collection is returned.</returns>
    public ObservableCollection<ReceiptSupplierItem> getListItemWithSupplierID(string supplier_id) {
        if (!SDataValidation.isIdValid(supplier_id)) {
            return new ObservableCollection<ReceiptSupplierItem>();
        }

        return executeWithConnection(connection => {
            var items = new ObservableCollection<ReceiptSupplierItem>();

            try {
                using var cmd = new MySqlCommand(
                    $"{BASE_SELECT_QUERY} WHERE {_m_COL_FK_SUPPLIER} = @supplier_id;",
                    connection);
                cmd.Parameters.AddWithValue("@supplier_id", supplier_id);

                using var reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    items.Add(CreateItemFromReader(reader));
                }
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            }
            
            return items;
        });
    }

    /// <summary>
    /// Creates a new instance of <see cref="ReceiptSupplierItem"/> using data from the specified <see
    /// cref="MySqlDataReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="MySqlDataReader"/> containing the data used to populate the <see cref="ReceiptSupplierItem"/>
    /// instance. Must not be null.</param>
    /// <returns>A <see cref="ReceiptSupplierItem"/> populated with values retrieved from the <paramref name="reader"/>.</returns>
    private static ReceiptSupplierItem CreateItemFromReader(MySqlDataReader reader) {
        return new ReceiptSupplierItem {
            fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT),
            fk_supplier_id = reader.getSafeValue<int>(_m_COL_FK_SUPPLIER)
        };
    }
}
