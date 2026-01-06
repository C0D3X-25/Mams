using Mams_App.src.databaseOperations;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams_App.src.receipts;

/// <summary>
/// Represents a model for managing receipt data in the database.
/// </summary>
public class ReceiptModel : ABaseModel, ICrudOperation<ReceiptItem> {

    private const string _m_TBL_NAME = "receipts";
    private const string _m_COL_ID = "receipt_id";
    private const string _m_COL_RECEIPT_NUMBER = "receipt_number";
    private const string _m_COL_RECEIPT_TOTAL_PRICE = "receipt_total_price";
    private const string _m_COL_RECEIPT_DATE_CREATED = "receipt_date_created";


    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, string.Empty, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ReceiptItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ResponseGetItem{ReceiptItem}"/> containing the receipt item and any error message.</returns>
    public ResponseGetItem<ReceiptItem> getItemByID(string id) {
        if (!SDataValidation.isIdValidForRetrieval(id)) {
            return ResponseGetItem<ReceiptItem>.Failure(EErrors.INVALID_INPUT,
                $"ReceiptModel.getItemByID: Invalid ID provided '{id}'");
        }

        return executeWithConnection(connection => {
            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, " +
                    $"{_m_COL_RECEIPT_NUMBER}, " +
                    $"{_m_COL_RECEIPT_TOTAL_PRICE}, " +
                    $"{_m_COL_RECEIPT_DATE_CREATED} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = @id;",
                    connection
                );

                cmd.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) {
                    return ResponseGetItem<ReceiptItem>.Success(new ReceiptItem {
                        receipt_id = reader.getSafeValue<int>(_m_COL_ID),
                        receipt_number = reader.getSafeValue(_m_COL_RECEIPT_NUMBER, string.Empty),
                        receipt_total_price = reader.getSafeValue<decimal>(_m_COL_RECEIPT_TOTAL_PRICE),
                        receipt_date_created = reader.getSafeValue(_m_COL_RECEIPT_DATE_CREATED, DateOnly.MinValue).ToString(globals.SGlobals.g_EU_DATE_FORMAT)
                    });
                }
                return ResponseGetItem<ReceiptItem>.NotFound();
            }
            catch (MySqlException ex) {
                return ResponseGetItem<ReceiptItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all receipt items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{ReceiptItem}"/> containing all receipt items and any error message.</returns>
    public ResponseGetAllItems<ReceiptItem> getAllItems() {
        var items = SDatabaseModel.getAllRowsInTable<ReceiptItem>(_m_TBL_NAME, _m_COL_RECEIPT_DATE_CREATED);
        return ResponseGetAllItems<ReceiptItem>.Success(items);
    }

    /// <summary>
    /// Saves the specified receipt item to the database.
    /// </summary>
    /// <param name="item">The receipt item to save. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved receipt item and any error message.
    /// Returns a response with ID 0 if the <paramref name="item"/> is <see langword="null"/> or if a
    /// database error occurs.</returns>
    public ResponseSaveItem saveItem(ReceiptItem item)
    {
        if (item == null)
        {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE,
                "ReceiptModel.saveItem: Item cannot be null");
        }

        int item_id = item.receipt_id;
        string receipt_nbr = item.receipt_number;

        // Check if receipt number already exists
        if (isReceiptNumberExisting(receipt_nbr))
        {
            // A new receipt cannot be created with an existing receipt number
            if (item_id == 0)
            {
                return ResponseSaveItem.Failure(EErrors.ALREADY_EXISTS,
                    $"ReceiptModel.saveItem: Receipt number '{receipt_nbr}' already exists");
            }
            else
            {
                int id_to_save = getIdWithReceiptNumber(receipt_nbr);
                // The receipt to modify uses a receipt number that already exists and is not the one already assigned
                if (id_to_save != item_id)
                {
                    return ResponseSaveItem.Failure(EErrors.ALREADY_EXISTS,
                        $"ReceiptModel.saveItem: Receipt number '{receipt_nbr}' is already assigned to another receipt (ID: {id_to_save})");
                }
            }
        }

        // Prepare query based on whether we're inserting or updating
        string query = item_id == 0
            ? $"INSERT INTO {_m_TBL_NAME} ({_m_COL_RECEIPT_NUMBER}, {_m_COL_RECEIPT_TOTAL_PRICE}, {_m_COL_RECEIPT_DATE_CREATED}) " +
              $"VALUES (@receipt_number, @total_price, @date_created); " +
              $"SELECT LAST_INSERT_ID();"
            : $"UPDATE {_m_TBL_NAME} " +
              $"SET {_m_COL_RECEIPT_NUMBER} = @receipt_number, " +
              $"{_m_COL_RECEIPT_TOTAL_PRICE} = @total_price, " +
              $"{_m_COL_RECEIPT_DATE_CREATED} = @date_created " +
              $"WHERE {_m_COL_ID} = @id;";

        startTransaction();

        try {
            if (item_id == 0) {
                // For INSERT operations
                item_id = executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@receipt_number", receipt_nbr);
                    cmd.Parameters.AddWithValue("@total_price", item.receipt_total_price);
                    cmd.Parameters.AddWithValue("@date_created", SFormatData.formatEUDateToMySQLDate(item.receipt_date_created));
                    return Convert.ToInt32(cmd.ExecuteScalar());
                });
            }
            else {
                // For UPDATE operations
                executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@id", item_id);
                    cmd.Parameters.AddWithValue("@receipt_number", receipt_nbr);
                    cmd.Parameters.AddWithValue("@total_price", item.receipt_total_price);
                    cmd.Parameters.AddWithValue("@date_created", SFormatData.formatEUDateToMySQLDate(item.receipt_date_created));
                    cmd.ExecuteNonQuery();
                });
            }

            commitTransaction();
            return ResponseSaveItem.Success(item_id);
        }
        catch (MySqlException ex) {
            rollbackTransaction();
            return ResponseSaveItem.MySqlFailure(ex.ErrorCode, ex.Message);
        }
    }

    /// <summary>
    /// Retrieves a collection of row IDs from the database table.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing the IDs of the rows in the specified database table. If an
    /// error occurs during the query execution, the collection may be empty or partially populated.</returns>
    public ObservableCollection<int> getRowsID() {
        return executeWithConnection(connection => {
            ObservableCollection<int> items = new();

            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID} FROM {_m_TBL_NAME} ORDER BY {_m_COL_RECEIPT_DATE_CREATED} DESC;",
                    connection,
                    m_transaction
                );
                
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    items.Add(reader.getSafeValue<int>(_m_COL_ID));
                }
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            }
            
            return items;
        });
    }

    /// <summary>
    /// Retrieves a collection of distinct years from the database based on receipt creation dates.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of strings containing the distinct years in descending order. The
    /// collection will be empty if no data is found or if an error occurs.</returns>
    public ObservableCollection<string> getExistingYear() {
        return executeWithConnection(connection => {
            ObservableCollection<string> items = new();

            try {
                using MySqlCommand cmd = new(
                    $"SELECT DISTINCT YEAR({_m_COL_RECEIPT_DATE_CREATED}) " +
                    $"AS year " +
                    $"FROM {_m_TBL_NAME} " +
                    $"ORDER BY year DESC;",
                    connection,
                    m_transaction
                );
                
                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    items.Add(reader.getSafeValue<int>("year").ToString());
                }
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            }
            
            return items;
        });
    }

    /// <summary>
    /// Determines whether a receipt number exists in the database.
    /// </summary>
    /// <param name="receipt_number">The receipt number to check for existence. Cannot be null, empty, or consist only of white-space characters.</param>
    /// <returns><see langword="true"/> if the specified receipt number exists in the database; otherwise, 
    /// <see langword="false"/>.</returns>
    public bool isReceiptNumberExisting(string receipt_number)
    {
        if (string.IsNullOrWhiteSpace(receipt_number))
        {
            return false;
        }

        return executeWithConnection(connection =>
        {
            try
            {
                using MySqlCommand cmd = new(
                    $"SELECT COUNT(*) FROM {_m_TBL_NAME} WHERE {_m_COL_RECEIPT_NUMBER} = @receipt_number;",
                    connection,
                    m_transaction
                );
                cmd.Parameters.AddWithValue("@receipt_number", receipt_number);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return false;
            }
        });
    }

    /// <summary>
    /// Find the ID of a receipt based on its receipt number.
    /// </summary>
    /// <param name="receipt_number">The string attribued to the receipt</param>
    /// <returns>The ID of the corresponding receipt number. Else 0 if not found</returns>
    public int getIdWithReceiptNumber(string receipt_number) {
        if (string.IsNullOrWhiteSpace(receipt_number)) {
            return 0;
        }

        return executeWithConnection(connection => {
            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID} FROM {_m_TBL_NAME} WHERE {_m_COL_RECEIPT_NUMBER} = @receipt_number;",
                    connection,
                    m_transaction
                );
                cmd.Parameters.AddWithValue("@receipt_number", receipt_number);
                object? result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return 0;
            }
        });
    }
}
