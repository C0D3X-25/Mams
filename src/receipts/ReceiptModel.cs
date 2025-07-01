using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

/// <summary>
/// Represents a model for managing receipt data in the database.
/// </summary>
public class ReceiptModel : ABaseModel, ICrudOperation<ReceiptItem> {

    private const string _m_TBL_NAME = "receipts";
    private const string _m_COL_ID = "receipt_id";
    private const string _m_COL_RECEIPT_TOTAL_PRICE = "receipt_total_price";
    private const string _m_COL_RECEIPT_DATE_CREATED = "receipt_date_created";

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns><see langword="true"/> if the item was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, string.Empty, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ReceiptItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ReceiptItem"/> object representing the receipt item with the specified identifier,  or <see
    /// langword="null"/> if no matching item is found or if the identifier is invalid.</returns>
    public ReceiptItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, " +
                $"{_m_COL_RECEIPT_TOTAL_PRICE}, " +
                $"{_m_COL_RECEIPT_DATE_CREATED} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ReceiptItem {
                    receipt_id = reader.getSafeValue<int>(_m_COL_ID),
                    receipt_total_price = reader.getSafeValue<decimal>(_m_COL_RECEIPT_TOTAL_PRICE),
                    receipt_date_created = reader.getSafeValue(_m_COL_RECEIPT_DATE_CREATED, DateOnly.MinValue).ToString(globals.SGlobals.g_EU_DATE_FORMAT)
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
    /// Retrieves all rows from the table associated with <see cref="ReceiptItem"/>.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing all rows in the table. If the table is empty, the collection
    /// will be empty.</returns>
    public ObservableCollection<ReceiptItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<ReceiptItem>(_m_TBL_NAME);
    }

    /// <summary>
    /// Saves the specified receipt item to the database.
    /// </summary>
    /// <param name="item">The receipt item to save. Cannot be <see langword="null"/>.</param>
    /// <returns>The ID of the saved receipt item. Returns 0 if the <paramref name="item"/> is <see langword="null"/> or if a
    /// database error occurs.</returns>
    public int saveItem(ReceiptItem item) {

        if (item == null) {
            return 0;
        }

        int item_id = item.receipt_id;

        string query = item_id == 0
            ? $"INSERT INTO {_m_TBL_NAME} ({_m_COL_RECEIPT_TOTAL_PRICE}, {_m_COL_RECEIPT_DATE_CREATED}) VALUES (@total_price, @date_created); " +
            $"SELECT LAST_INSERT_ID();"
            : $"UPDATE {_m_TBL_NAME} SET {_m_COL_RECEIPT_TOTAL_PRICE} = @total_price, {_m_COL_RECEIPT_DATE_CREATED} = @date_created WHERE {_m_COL_ID} = @id;";

        bool transaction_needed = false;
        if (!isTransactionActive()) {
            startTransaction();
            transaction_needed = true;
        }

        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);
            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@total_price", item.receipt_total_price);
            cmd.Parameters.AddWithValue("@date_created", SFormatData.formatEUDateToMySQLDate(item.receipt_date_created));

            if (item_id == 0) {
                item_id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else {
                cmd.ExecuteNonQuery();
            }

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
    /// Retrieves a collection of row IDs from the database table.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing the IDs of the rows in the specified database table. If an
    /// error occurs during the query execution, the collection may be empty or partially populated.</returns>
    public ObservableCollection<int> getRowsID() {
        
        ObservableCollection<int> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID} FROM {_m_TBL_NAME};",
                m_conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(reader.getSafeValue<int>(_m_COL_ID));
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }

    /// <summary>
    /// Retrieves a collection of distinct years from the database based on receipt creation dates.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of strings containing the distinct years in descending order. The
    /// collection will be empty if no data is found or if an error occurs.</returns>
    public ObservableCollection<string> getExistingYear() {
        
        ObservableCollection<string> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT DISTINCT YEAR({_m_COL_RECEIPT_DATE_CREATED}) " +
                $"AS year " +
                $"FROM {_m_TBL_NAME} " +
                $"ORDER BY year DESC;",
                m_conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(reader.getSafeValue<int>("year").ToString());
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }
}
