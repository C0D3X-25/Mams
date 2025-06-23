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

    private const string _m_TBL_NAME = "receipts";
    private const string _m_COL_ID = "receipt_id";
    private const string _m_COL_RECEIPT_TOTAL_PRICE = "receipt_total_price";
    private const string _m_COL_RECEIPT_DATE_CREATED = "receipt_date_created";

    /// <summary>
    /// Deletes a receipt item from the database based on the provided ID.
    /// </summary>
    /// <param name="id">The ID of the receipt to delete</param>
    /// <param name="delete_type">The type of deletion to perform (default: HARD_DELETE)</param>
    /// <returns>True if deletion was successful, false otherwise</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, string.Empty, _m_TBL_NAME, delete_type);
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
        
        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, " +
                $"{_m_COL_RECEIPT_TOTAL_PRICE}, " +
                $"{_m_COL_RECEIPT_DATE_CREATED} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ReceiptItem {
                    receipt_id = reader.GetSafeValue<int>(_m_COL_ID),
                    receipt_total_price = reader.GetSafeValue<decimal>(_m_COL_RECEIPT_TOTAL_PRICE),
                    receipt_date_created = reader.GetSafeValue(_m_COL_RECEIPT_DATE_CREATED, DateOnly.MinValue).ToString(globals.SGlobals.g_DATE_FORMAT)
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
        return SDatabaseModel.getAllRowsInTable<ReceiptItem>(_m_TBL_NAME);
    }

    /// <summary>
    /// Saves a receipt item to the database.If the item's ID is 0, creates a new record;
    /// otherwise updates the existing record.
    /// </summary>
    /// <param name = "item" > The ReceiptItem to save</param>
    /// <returns>The ID of the saved receipt; 0 if the operation failed</returns>
    public int saveItem(ReceiptItem item) {
        if (item == null) {
            return 0;
        }

        int item_id = item.receipt_id;

        DateTime parsed_date = DateTime.ParseExact(item.receipt_date_created, globals.SGlobals.g_DATE_FORMAT, null);
        string mysql_formatted_date = parsed_date.ToString("yyyy-MM-dd");

        string query = item_id == 0
            ? $"INSERT INTO {_m_TBL_NAME} ({_m_COL_RECEIPT_TOTAL_PRICE}, {_m_COL_RECEIPT_DATE_CREATED}) VALUES (@total_price, @date_created); SELECT LAST_INSERT_ID();"
            : $"UPDATE {_m_TBL_NAME} SET {_m_COL_RECEIPT_TOTAL_PRICE} = @total_price, {_m_COL_RECEIPT_DATE_CREATED} = @date_created WHERE {_m_COL_ID} = @id;";

        //if (transaction == null) {
        //    transaction = conn?.BeginTransaction();
        //}

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

            //transaction?.Commit();
            return item_id;
        }
        catch (MySqlException ex) {
            //transaction?.Rollback();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }



    /// <summary>
    /// Retrieves all receipt IDs from the database.
    /// </summary>
    /// <returns>An ObservableCollection of receipt IDs</returns>
    public ObservableCollection<int> getRowsID() {
        
        ObservableCollection<int> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID} FROM {_m_TBL_NAME};",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(reader.GetSafeValue<int>(_m_COL_ID));
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
        
        ObservableCollection<string> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT DISTINCT YEAR({_m_COL_RECEIPT_DATE_CREATED}) " +
                $"AS year " +
                $"FROM {_m_TBL_NAME} " +
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
