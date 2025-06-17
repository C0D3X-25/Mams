using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

/// <summary>
/// Manages receipt-related database operations including CRUD functionality.
/// Implements the ICrudOperation interface for ReceiptItem objects.
/// </summary>
public class ReceiptModel : ABaseModel, ICrudOperation<ReceiptItem> {

    private const string m_TBL_NAME = "receipts";
    private const string m_COL_ID = "receipt_id";
    private const string m_COL_RECEIPT_TOTAL_PRICE = "receipt_total_price";
    private const string m_COL_RECEIPT_DATE_CREATED = "receipt_date_created";

    /// <summary>
    /// Deletes a receipt item from the database based on the provided ID.
    /// </summary>
    /// <param name="id">The ID of the receipt to delete</param>
    /// <param name="delete_type">The type of deletion to perform (default: HARD_DELETE)</param>
    /// <returns>True if deletion was successful, false otherwise</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_ID, string.Empty, m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    /// <summary>
    /// Retrieves a specific receipt item from the database by its ID.
    /// </summary>
    /// <param name="id">The ID of the receipt to retrieve</param>
    /// <returns>A ReceiptItem object if found; null otherwise</returns>
    public ReceiptItem? getItemByID(string id) {
        using MySqlConnection? conn = _m_conn.openConnection();
        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID}, " +
                $"{m_COL_RECEIPT_TOTAL_PRICE}, " +
                $"{m_COL_RECEIPT_DATE_CREATED} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ReceiptItem {
                    receipt_id = reader.GetSafeValue<int>(m_COL_ID),
                    receipt_total_price = reader.GetSafeValue<decimal>(m_COL_RECEIPT_TOTAL_PRICE),
                    receipt_date_created = reader.GetSafeValue(m_COL_RECEIPT_DATE_CREATED, DateOnly.MinValue).ToString(globals.SGlobals.g_DATE_FORMAT)
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
    /// Retrieves all receipt items from the database.
    /// </summary>
    /// <returns>An ObservableCollection of ReceiptItem objects</returns>
    public ObservableCollection<ReceiptItem> getTable() {
        return SDatabaseModel.getAllData<ReceiptItem>(this, m_TBL_NAME);
    }

    /// <summary>
    /// Saves a receipt item to the database. If the item's ID is 0, creates a new record;
    /// otherwise updates the existing record.
    /// </summary>
    /// <param name="item">The ReceiptItem to save</param>
    /// <returns>The ID of the saved receipt; 0 if the operation failed</returns>
    public int saveItem(ReceiptItem item) {
        if (item == null) {
            return 0;
        }

        using MySqlConnection? conn = _m_conn.openConnection();
        if (conn == null)
            return 0;

        int item_id = item.receipt_id;

        using var transaction = conn.BeginTransaction();

        DateTime parsed_date = DateTime.ParseExact(item.receipt_date_created, globals.SGlobals.g_DATE_FORMAT, null);
        string mysql_formatted_date = parsed_date.ToString("yyyy-MM-dd");

        string query = item_id == 0
            ? $"INSERT INTO {m_TBL_NAME} ({m_COL_RECEIPT_TOTAL_PRICE}, {m_COL_RECEIPT_DATE_CREATED}) VALUES (@total_price, @date_created); SELECT LAST_INSERT_ID();"
            : $"UPDATE {m_TBL_NAME} SET {m_COL_RECEIPT_TOTAL_PRICE} = @total_price, {m_COL_RECEIPT_DATE_CREATED} = @date_created WHERE {m_COL_ID} = @id;";

        try {
            using MySqlCommand cmd = new(query, conn, transaction);
            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@total_price", item.receipt_total_price);
            cmd.Parameters.AddWithValue("@date_created", mysql_formatted_date);

            if (item_id == 0) {
                item_id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else {
                cmd.ExecuteNonQuery();
            }
            
            transaction.Commit();
            return item_id;
        }
        catch (MySqlException ex) {
            transaction.Rollback();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Retrieves all receipt IDs from the database.
    /// </summary>
    /// <returns>An ObservableCollection of receipt IDs</returns>
    public ObservableCollection<int> getRowsID() {
        using MySqlConnection? conn = _m_conn.openConnection();
        ObservableCollection<int> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID} FROM {m_TBL_NAME};",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(reader.GetSafeValue<int>(m_COL_ID));
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }

    /// <summary>
    /// Retrieves all distinct years from receipt dates in the database.
    /// </summary>
    /// <returns>An ObservableCollection of years as strings, sorted in descending order</returns>
    public ObservableCollection<string> getExistingYear() {
        using MySqlConnection? conn = _m_conn.openConnection();
        ObservableCollection<string> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT DISTINCT YEAR({m_COL_RECEIPT_DATE_CREATED}) " +
                $"AS year " +
                $"FROM {m_TBL_NAME} " +
                $"ORDER BY year DESC;",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(reader.GetSafeValue<int>("year").ToString());
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }
}
