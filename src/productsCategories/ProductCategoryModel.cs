using Mams.src.databaseOperations;
using Mams.src.errors;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsCategories;

/// <summary>
/// Represents a model for managing product categories in the database.
/// </summary>
public class ProductCategoryModel :
    ABaseModel,
    ICrudOperation<ProductCategoryItem> {

    private const string _m_TBL_NAME = "products_categories";
    private const string _m_COL_ID = "product_category_id";
    private const string _m_COL_NAME = "product_category_name";
    private const string _m_COL_ARCHIVE = "product_category_archive";

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ProductCategoryItem"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product category item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ResponseGetItem{ProductCategoryItem}"/> containing the product category and any error message.</returns>
    public ResponseGetItem<ProductCategoryItem> getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return ResponseGetItem<ProductCategoryItem>.Failure(EErrors.INVALID_INPUT);
        }

        return executeWithConnection(connection => {
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
                    return ResponseGetItem<ProductCategoryItem>.Success(new ProductCategoryItem {
                        product_category_id = reader.getSafeValue<int>(_m_COL_ID),
                        product_category_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                        product_category_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                    });
                }
                return ResponseGetItem<ProductCategoryItem>.NotFound();
            }
            catch (MySqlException ex) {
                return ResponseGetItem<ProductCategoryItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all product category items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{ProductCategoryItem}"/> containing all product category items and any error message.</returns>
    public ResponseGetAllItems<ProductCategoryItem> getAllItems() {
        var items = SDatabaseModel.getAllRowsInTable<ProductCategoryItem>(_m_TBL_NAME, _m_COL_NAME);
        return ResponseGetAllItems<ProductCategoryItem>.Success(items);
    }

    /// <summary>
    /// Saves a product category item to the database. If the item does not exist, it inserts a new record and returns
    /// the generated ID. If the item already exists, it updates the existing record.
    /// </summary>
    /// <param name="item">The <see cref="ProductCategoryItem"/> to be saved.  The <see cref="ProductCategoryItem.product_category_id"/>
    /// property determines whether the item is inserted (if 0) or updated (if non-zero).</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved product category item and any error message.
    /// Returns a response with ID 0 if the operation fails or if the item is null.</returns>
    public ResponseSaveItem saveItem(ProductCategoryItem item) {
        if (item == null) {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE);
        }

        int item_id = item.product_category_id;
        string item_name = item.product_category_name.Trim();
        string query;
        
        // Determine if we're inserting or updating
        bool isInsert = (item_id == 0);
        
        if (isInsert) {
            // Check for duplicate name before inserting
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.product_category_name.Trim())) {
                return ResponseSaveItem.Failure(EErrors.ALREADY_EXISTS);
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
            
            return ResponseSaveItem.Success(item_id);
        }
        catch (MySqlException ex) {
            if (need_transaction) {
                rollbackTransaction();
            }
            return ResponseSaveItem.MySqlFailure(ex.ErrorCode, ex.Message);
        }
    }
}
