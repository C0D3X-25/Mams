using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

/// <summary>
/// Manages database operations for receipt-client relationships.
/// Implements CRUD operations for ReceiptClientItem entities.
/// </summary>
public class ReceiptClientModel : ABaseModel, ICrudOperation<ReceiptClientItem> {

    private const string m_TBL_NAME = "receipts_clients";
    private const string m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string m_COL_FK_CLIENT = "fk_client_id";

    private const string SELECT_COLUMNS = $"{m_COL_FK_RECEIPT}, {m_COL_FK_CLIENT}";
    private const string BASE_SELECT_QUERY = $"SELECT {SELECT_COLUMNS} FROM {m_TBL_NAME}";

    /// <summary>
    /// Deletes a receipt-client relationship from the database.
    /// </summary>
    /// <param name="id">The receipt ID to delete.</param>
    /// <param name="delete_type">The type of deletion operation to perform.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_FK_RECEIPT, string.Empty, m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Deletes a receipt-client relationship from the database using an integer ID.
    /// </summary>
    /// <param name="id">The receipt ID to delete as an integer.</param>
    /// <param name="delete_type">The type of deletion operation to perform.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    /// <summary>
    /// Retrieves a specific receipt-client relationship by receipt ID.
    /// </summary>
    /// <param name="id">The receipt ID to search for.</param>
    /// <returns>The matching ReceiptClientItem if found, null otherwise.</returns>
    public ReceiptClientItem? getItemByID(string id) {
        using MySqlConnection? conn = _m_conn.openConnection();
        if (conn == null) return null;
        
        try {
            using var cmd = new MySqlCommand($"{BASE_SELECT_QUERY} WHERE {m_COL_FK_RECEIPT} = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? CreateItemFromReader(reader) : null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Retrieves all receipt-client relationships from the database.
    /// </summary>
    /// <returns>An ObservableCollection of all ReceiptClientItems.</returns>
    public ObservableCollection<ReceiptClientItem> getTable() {
        var items = new ObservableCollection<ReceiptClientItem>();
        using MySqlConnection? conn = _m_conn.openConnection();
        if (conn == null) return items;

        try {
            using var cmd = new MySqlCommand(BASE_SELECT_QUERY, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(CreateItemFromReader(reader));
            }
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
        }
        return items;
    }

    /// <summary>
    /// Saves a receipt-client relationship to the database. Creates new record if it doesn't exist,
    /// updates existing record if it does.
    /// </summary>
    /// <param name="item">The ReceiptClientItem to save.</param>
    /// <returns>True if the save operation was successful, false otherwise.</returns>
    public bool saveItem(ReceiptClientItem item) {
        if (item?.fk_receipt_id == 0 || item?.fk_client_id == 0) return false;

        using MySqlConnection? conn = _m_conn.openConnection();
        if (conn == null) return false;

        try {
            using var transaction = conn.BeginTransaction();
            try {
                using var exist_cmd = new MySqlCommand(
                    $"SELECT COUNT(*) FROM {m_TBL_NAME} WHERE {m_COL_FK_RECEIPT} = @fk_receipt;",
                    conn, transaction);
                exist_cmd.Parameters.AddWithValue("@fk_receipt", item?.fk_receipt_id);

                bool exists = Convert.ToInt32(exist_cmd.ExecuteScalar()) > 0;
                string query = exists
                    ? $"UPDATE {m_TBL_NAME} SET {m_COL_FK_CLIENT} = @fk_client WHERE {m_COL_FK_RECEIPT} = @fk_receipt;"
                    : $"INSERT INTO {m_TBL_NAME} ({m_COL_FK_RECEIPT}, {m_COL_FK_CLIENT}) VALUES (@fk_receipt, @fk_client);";

                using var cmd = new MySqlCommand(query, conn, transaction);
                cmd.Parameters.AddWithValue("@fk_receipt", item?.fk_receipt_id);
                cmd.Parameters.AddWithValue("@fk_client", item?.fk_client_id);

                bool success = cmd.ExecuteNonQuery() > 0;
                transaction.Commit();
                return success;
            }
            catch {
                transaction.Rollback();
                throw;
            }
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Retrieves all receipt-client relationships for a specific receipt ID.
    /// </summary>
    /// <param name="fk_receipt">The receipt ID to search for.</param>
    /// <returns>An ObservableCollection of ReceiptClientItems associated with the given receipt ID.</returns>
    public ObservableCollection<ReceiptClientItem> getListItemWithReceiptID(string fk_receipt) {
        var items = new ObservableCollection<ReceiptClientItem>();
        if (string.IsNullOrEmpty(fk_receipt)) return items;

        using MySqlConnection? conn = _m_conn.openConnection();
        if (conn == null) return items;

        try {
            using var cmd = new MySqlCommand(
                $"{BASE_SELECT_QUERY} WHERE {m_COL_FK_RECEIPT} = @fk_receipt;",
                conn);
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
    }

    /// <summary>
    /// Creates a ReceiptClientItem from a MySqlDataReader.
    /// </summary>
    /// <param name="reader">The MySqlDataReader containing the data.</param>
    /// <returns>A ReceiptClientItem populated with data from the reader.</returns>
    private static ReceiptClientItem CreateItemFromReader(MySqlDataReader reader) {
        return new ReceiptClientItem {
            fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
            fk_client_id = reader.GetSafeValue<int>(m_COL_FK_CLIENT)
        };
    }
}
