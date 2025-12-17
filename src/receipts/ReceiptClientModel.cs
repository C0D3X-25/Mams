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
/// Represents a model for managing receipt-client relationships in the database.
/// </summary>
/// <remarks>This class provides methods to perform CRUD operations on receipt-client relationships,  including
/// retrieving, saving, and deleting records. It interacts with the database using  predefined queries and supports
/// operations such as hard and soft deletion.</remarks>
public class ReceiptClientModel : ABaseModel, ICrudOperation<ReceiptClientItem> {

    private const string m_TBL_NAME = "receipts_clients";
    private const string _m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string _m_COL_FK_CLIENT = "fk_client_id";

    private const string SELECT_COLUMNS = $"{_m_COL_FK_RECEIPT}, {_m_COL_FK_CLIENT}";
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
    /// Retrieves a <see cref="ReceiptClientItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ResponseGetItem{ReceiptClientItem}"/> containing the receipt client item and any error message.</returns>
    public ResponseGetItem<ReceiptClientItem> getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return ResponseGetItem<ReceiptClientItem>.Failure(EErrors.INVALID_INPUT,
                $"ReceiptClientModel.getItemByID: Invalid ID provided '{id}'");
        }

        return executeWithConnection(connection => {
            try {
                using var cmd = new MySqlCommand($"{BASE_SELECT_QUERY} WHERE {_m_COL_FK_RECEIPT} = @id;", connection);
                cmd.Parameters.AddWithValue("@id", id);

                using var reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    return ResponseGetItem<ReceiptClientItem>.Success(CreateItemFromReader(reader));
                }
                return ResponseGetItem<ReceiptClientItem>.NotFound();
            }
            catch (MySqlException ex) {
                return ResponseGetItem<ReceiptClientItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all receipt client items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{ReceiptClientItem}"/> containing all receipt client items and any error message.</returns>
    public ResponseGetAllItems<ReceiptClientItem> getAllItems() {
        return executeWithConnection(connection => {
            var items = new ObservableCollection<ReceiptClientItem>();

            try {
                using var cmd = new MySqlCommand(BASE_SELECT_QUERY, connection);
                using var reader = cmd.ExecuteReader();

                while (reader.Read()) {
                    items.Add(CreateItemFromReader(reader));
                }
                return ResponseGetAllItems<ReceiptClientItem>.Success(items);
            }
            catch (MySqlException ex) {
                return ResponseGetAllItems<ReceiptClientItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Saves the specified <see cref="ReceiptClientItem"/> to the database.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptClientItem"/> to save. The item must not be <see langword="null"/>, and its
    /// <c>fk_receipt_id</c> and <c>fk_client_id</c> properties must be non-zero.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved item and any error message.
    /// Returns a response with ID 0 if the input is invalid or if an error occurs during the operation.</returns>
    public ResponseSaveItem saveItem(ReceiptClientItem item) {
        if (item == null || item.fk_receipt_id == 0 || item.fk_client_id == 0) {
            return ResponseSaveItem.Failure(EErrors.INVALID_INPUT,
                $"ReceiptClientModel.saveItem: Invalid input - item is null or fk_receipt_id ({item?.fk_receipt_id}) or fk_client_id ({item?.fk_client_id}) is 0");
        }

        int item_id = item.fk_receipt_id;

        startTransaction();

        try {
            string query = isIdenticItemPresentInTable(m_TBL_NAME, _m_COL_FK_RECEIPT, item_id.ToString())
                ? $"UPDATE {m_TBL_NAME} SET {_m_COL_FK_CLIENT} = @fk_client WHERE {_m_COL_FK_RECEIPT} = @fk_receipt; SELECT @fk_receipt;"
                : $"INSERT INTO {m_TBL_NAME} ({_m_COL_FK_RECEIPT}, {_m_COL_FK_CLIENT}) VALUES (@fk_receipt, @fk_client); " +
                  $"SELECT LAST_INSERT_ID();";

            item_id = executeWithConnection(connection => {
                using var cmd = new MySqlCommand(query, connection, m_transaction);
                cmd.Parameters.AddWithValue("@fk_receipt", item_id);
                cmd.Parameters.AddWithValue("@fk_client", item.fk_client_id);

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
    /// Retrieves a collection of <see cref="ReceiptClientItem"/> objects associated with the specified receipt ID.
    /// </summary>
    /// <param name="fk_receipt">The foreign key receipt ID used to filter the items. Must not be null or empty.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing the <see cref="ReceiptClientItem"/> objects associated with
    /// the specified receipt ID. If <paramref name="fk_receipt"/> is null or empty, an empty collection is returned.</returns>
    public ObservableCollection<ReceiptClientItem> getListItemWithReceiptID(string fk_receipt) {
        if (string.IsNullOrEmpty(fk_receipt)) {
            return new ObservableCollection<ReceiptClientItem>();
        }

        return executeWithConnection(connection => {
            var items = new ObservableCollection<ReceiptClientItem>();

            try {
                using var cmd = new MySqlCommand(
                    $"{BASE_SELECT_QUERY} WHERE {_m_COL_FK_RECEIPT} = @fk_receipt;",
                    connection);
                cmd.Parameters.AddWithValue("@fk_receipt", fk_receipt);

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
    /// Creates a new instance of <see cref="ReceiptClientItem"/> using data from the specified <see
    /// cref="MySqlDataReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="MySqlDataReader"/> containing the data used to populate the <see cref="ReceiptClientItem"/>
    /// instance. Must not be null.</param>
    /// <returns>A <see cref="ReceiptClientItem"/> populated with values retrieved from the <paramref name="reader"/>.</returns>
    private static ReceiptClientItem CreateItemFromReader(MySqlDataReader reader) {
        return new ReceiptClientItem {
            fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT),
            fk_client_id = reader.getSafeValue<int>(_m_COL_FK_CLIENT)
        };
    }
}
