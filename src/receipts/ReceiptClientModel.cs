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
    private const string _m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string _m_COL_FK_CLIENT = "fk_client_id";

    private const string SELECT_COLUMNS = $"{_m_COL_FK_RECEIPT}, {_m_COL_FK_CLIENT}";
    private const string BASE_SELECT_QUERY = $"SELECT {SELECT_COLUMNS} FROM {m_TBL_NAME}";

    /// <summary>
    /// Deletes a receipt-client relationship from the database.
    /// </summary>
    /// <param name="id">The receipt ID to delete.</param>
    /// <param name="delete_type">The type of deletion operation to perform.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_FK_RECEIPT, string.Empty, m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    /// <summary>
    /// Retrieves a specific receipt-client relationship by receipt ID.
    /// </summary>
    /// <param name="id">The receipt ID to search for.</param>
    /// <returns>The matching ReceiptClientItem if found, null otherwise.</returns>
    public ReceiptClientItem? getItemByID(string id) {
        
        if (m_conn == null) return null;
        
        try {
            using var cmd = new MySqlCommand($"{BASE_SELECT_QUERY} WHERE {_m_COL_FK_RECEIPT} = @id;", m_conn);
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

        if (m_conn == null) 
            return items;

        try {
            using var cmd = new MySqlCommand(BASE_SELECT_QUERY, m_conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(CreateItemFromReader(reader));
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }

    /// <summary>
    /// Saves a receipt item to the database. If the item's ID is 0, creates a new record;
    /// otherwise updates the existing record.
    /// </summary>
    /// <param name="item">The ReceiptClientItem to save</param>
    /// <returns>The ID of the saved receipt; 0 if the operation failed</returns>
    public int saveItem(ReceiptClientItem item) {
        if (item == null || item.fk_receipt_id == 0 || item.fk_client_id == 0) 
            return 0;

        int item_id = item.fk_receipt_id;    

        //startTransaction();

        try {
            //using var exist_cmd = new MySqlCommand(
            //    $"SELECT COUNT(*) FROM {m_TBL_NAME} WHERE {_m_COL_FK_RECEIPT} = @fk_receipt;",
            //    conn, transaction);
            //exist_cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            //bool exists = Convert.ToInt32(exist_cmd.ExecuteScalar()) > 0;
                
            string query = isIdenticItemPresentInTable(m_TBL_NAME, _m_COL_FK_RECEIPT, item_id.ToString())
                ? $"UPDATE {m_TBL_NAME} SET {_m_COL_FK_CLIENT} = @fk_client WHERE {_m_COL_FK_RECEIPT} = @fk_receipt; SELECT @fk_receipt;"
                : $"INSERT INTO {m_TBL_NAME} ({_m_COL_FK_RECEIPT}, {_m_COL_FK_CLIENT}) VALUES (@fk_receipt, @fk_client); SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(query, m_conn, m_transaction);
            cmd.Parameters.AddWithValue("@fk_receipt", item_id);
            cmd.Parameters.AddWithValue("@fk_client", item.fk_client_id);

            item_id = Convert.ToInt32(cmd.ExecuteScalar());

            //commitTransaction();
            return item_id;
        }
        catch (MySqlException ex) {
            //rollbackTransaction();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
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

        
        if (m_conn == null) return items;

        try {
            using var cmd = new MySqlCommand(
                $"{BASE_SELECT_QUERY} WHERE {_m_COL_FK_RECEIPT} = @fk_receipt;",
                m_conn);
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
            fk_receipt_id = reader.GetSafeValue<int>(_m_COL_FK_RECEIPT),
            fk_client_id = reader.GetSafeValue<int>(_m_COL_FK_CLIENT)
        };
    }
}
