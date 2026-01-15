using Mams_App.src.databaseOperations;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams_App.src.productsTypes;

/// <summary>
/// Represents a model for managing product types in the database.
/// </summary>
public class ProductTypeModel :
    ABaseModel,
    ICrudOperation<ProductTypeItem>
{

    private const string _m_TBL_NAME = "products_types";
    private const string _m_COL_ID = "product_type_id";
    private const string _m_COL_NAME = "product_type_name";
    private const string _m_COL_ARCHIVE = "product_type_archive";

    private const string _m_DEFAULT_ARCHIVE = "1901-01-01";


    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE)
    {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ProductTypeItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product type item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ResponseGetItem{ProductTypeItem}"/> containing the product type item and any error message.</returns>
    public ResponseGetItem<ProductTypeItem> getItemByID(string id)
    {
        if (!SDataValidation.isIdValidForRetrieval(id))
        {
            return ResponseGetItem<ProductTypeItem>.Failure(EErrors.INVALID_INPUT,
                $"ProductTypeModel.getItemByID: Invalid ID provided '{id}'");
        }

        return executeWithConnection(connection =>
        {
            try
            {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_ARCHIVE} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = @id;",
                    connection
                );

                cmd.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return ResponseGetItem<ProductTypeItem>.Success(new ProductTypeItem
                    {
                        product_type_id = reader.getSafeValue<int>(_m_COL_ID),
                        product_type_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                        product_type_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                    });
                }
                return ResponseGetItem<ProductTypeItem>.NotFound();
            }
            catch (MySqlException ex)
            {
                return ResponseGetItem<ProductTypeItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all product type items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{ProductTypeItem}"/> containing all product type items and any error message.</returns>
    public ResponseGetAllItems<ProductTypeItem> getAllItems()
    {
        var items = SDatabaseModel.getAllRowsInTable<ProductTypeItem>(_m_TBL_NAME, _m_COL_NAME);
        return ResponseGetAllItems<ProductTypeItem>.Success(items);
    }

    /// <summary>
    /// Retrieves all active (non-archived) product type items from the database.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{ProductTypeItem}"/> containing all non-archived product types, ordered by name.</returns>
    public ObservableCollection<ProductTypeItem> getActiveProductTypes()
    {
        return executeWithConnection(connection =>
        {
            try
            {
                string query = $@"
                    SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_ARCHIVE}
                    FROM {_m_TBL_NAME}
                    WHERE {_m_COL_ARCHIVE} = '{_m_DEFAULT_ARCHIVE}' OR {_m_COL_ARCHIVE} IS NULL
                    ORDER BY {_m_COL_NAME} ASC;";

                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                ObservableCollection<ProductTypeItem> items = [];

                while (reader.Read())
                {
                    items.Add(new ProductTypeItem
                    {
                        product_type_id = reader.getSafeValue<int>(_m_COL_ID),
                        product_type_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                        product_type_archive = string.Empty
                    });
                }

                return items;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return [];
            }
        });
    }

    /// <summary>
    /// Determines whether a product type can be permanently deleted or will be archived due to foreign key references.
    /// </summary>
    /// <param name="id">The unique identifier of the product type to check.</param>
    /// <returns><see langword="true"/> if the product type can be permanently deleted; 
    /// <see langword="false"/> if it will be archived due to references.</returns>
    public bool canBeHardDeleted(string id)
    {
        return !isReferencedByOtherTables(_m_TBL_NAME, _m_COL_ID, id);
    }

    /// <summary>
    /// Saves the specified product type item to the database.
    /// </summary>
    /// <param name="item">The <see cref="ProductTypeItem"/> to save. Cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved product type item and any error message.
    /// Returns a response with ID 0 if the operation fails or if the item is <see langword="null"/>.</returns>
    public ResponseSaveItem saveItem(ProductTypeItem item)
    {
        if (item == null)
        {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE,
                "ProductTypeModel.saveItem: Item cannot be null");
        }

        int item_id = item.product_type_id;
        string item_name = item.product_type_name.Trim();
        string query;

        // Determine if we're inserting or updating
        bool isInsert = (item_id == 0);

        if (isInsert)
        {
            // Check for duplicate name before inserting
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.product_type_name.Trim()))
            {
                return ResponseSaveItem.Failure(EErrors.ALREADY_EXISTS,
                    $"ProductTypeModel.saveItem: Product type with name '{item.product_type_name}' already exists");
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}) " +
                $"VALUES (@name); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else
        {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        // Start transaction if needed
        bool need_transaction = !isTransactionActive();
        if (need_transaction)
        {
            startTransaction();
        }

        try
        {
            if (isInsert)
            {
                // For INSERT operations, we need to return the new ID
                item_id = executeWithConnection(connection =>
                {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@name", item_name);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                });
            }
            else
            {
                // For UPDATE operations, we just execute the command
                executeWithConnection(connection =>
                {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@id", item_id);
                    cmd.Parameters.AddWithValue("@name", item_name);
                    cmd.ExecuteNonQuery();
                });
            }

            if (need_transaction)
            {
                commitTransaction();
            }

            return ResponseSaveItem.Success(item_id);
        }
        catch (MySqlException ex)
        {
            if (need_transaction)
            {
                rollbackTransaction();
            }
            return ResponseSaveItem.MySqlFailure(ex.ErrorCode, ex.Message);
        }
    }
}
