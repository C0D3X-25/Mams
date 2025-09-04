using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsTypes;

/// <summary>
/// Represents a model for managing product types in the database.
/// </summary>
public class ProductTypeModel :
    ABaseModel,
    ICrudOperation<ProductTypeItem> {

    private const string _m_TBL_NAME = "products_types";
    private const string _m_COL_ID = "product_type_id";
    private const string _m_COL_NAME = "product_type_name";
    private const string _m_COL_ARCHIVE = "product_type_archive";


    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns><see langword="true"/> if the item was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ProductTypeItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product type item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ProductTypeItem"/> object representing the product type item with the specified identifier,  or
    /// <see langword="null"/> if no matching item is found or if the identifier is invalid.</returns>
    public ProductTypeItem? getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        return executeWithConnection<ProductTypeItem?>(connection => {
            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_ARCHIVE} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = @id;",
                    connection
                );

                cmd.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) {
                    return new ProductTypeItem {
                        product_type_id = reader.getSafeValue<int>(_m_COL_ID),
                        product_type_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                        product_type_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                    };
                }
                return null;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return null;
            }
        });
    }

    /// <summary>
    /// Retrieves all rows from the specified table and returns them as an observable collection.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing all rows of type <see cref="ProductTypeItem"/> from the
    /// table. The collection will be empty if the table contains no rows.</returns>
    public ObservableCollection<ProductTypeItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<ProductTypeItem>(_m_TBL_NAME, _m_COL_NAME);
    }

    /// <summary>
    /// Saves the specified product type item to the database.
    /// </summary>
    /// <param name="item">The <see cref="ProductTypeItem"/> to save. Cannot be <see langword="null"/>.</param>
    /// <returns>The ID of the saved product type item. Returns <c>0</c> if the operation fails or if the item is <see
    /// langword="null"/>.</returns>
    public int saveItem(ProductTypeItem item) {
        if (item == null) {
            return 0;
        }

        int item_id = item.product_type_id;
        string item_name = item.product_type_name.Trim();
        string query;
        
        // Determine if we're inserting or updating
        bool isInsert = (item_id == 0);
        
        if (isInsert) {
            // Check for duplicate name before inserting
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item_name)) {
                return 0;
            }
            
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}) " +
                $"VALUES (@name); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        // Start transaction if needed
        bool need_transaction = !isTransactionActive();
        if (need_transaction) {
            startTransaction();
        }

        try {
            if (isInsert) {
                // For INSERT operations, we need to return the new ID
                item_id = executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@name", item_name);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                });
            }
            else {
                // For UPDATE operations, we just execute the command
                executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@id", item_id);
                    cmd.Parameters.AddWithValue("@name", item_name);
                    cmd.ExecuteNonQuery();
                });
            }
            
            if (need_transaction) {
                commitTransaction();
            }
            
            return item_id;
        }
        catch (MySqlException ex) {
            if (need_transaction) {
                rollbackTransaction();
            }
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }
}
