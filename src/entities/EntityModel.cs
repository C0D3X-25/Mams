using Mams.src.databaseOperations;
using Mams.src.errors;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.entities;

/// <summary>
/// Represents a model for managing entities in a database./>
/// </summary>
public class EntityModel : ABaseModel,
    ICrudOperation<EntityItem> {

    private const string _m_TBL_NAME  = "entities";
    private const string _m_COL_ID = "entity_id";
    private const string _m_COL_NAME = "entity_name";
    private const string _m_COL_PHONE = "entity_phone";
    private const string _m_COL_EMAIL = "entity_email";
    private const string _m_COL_CITY = "entity_city";
    private const string _m_COL_ADDRESS = "entity_address";
    private const string _m_COL_ARCHIVE = "entity_archive";

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
    /// Retrieves an entity item from the database by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity item to retrieve. This value must not be null or empty.</param>
    /// <returns>A <see cref="ResponseGetItem{EntityItem}"/> containing the entity item and any error message.</returns>
    public ResponseGetItem<EntityItem> getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return ResponseGetItem<EntityItem>.Failure(EErrors.INVALID_INPUT);
        }

        return executeWithConnection(connection => {
            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}, {_m_COL_ARCHIVE} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = @id ",
                    connection
                );

                cmd.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) {
                    return ResponseGetItem<EntityItem>.Success(new EntityItem {
                        entity_id = reader.getSafeValue<int>(_m_COL_ID),
                        entity_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                        entity_phone = reader.getSafeValue(_m_COL_PHONE, string.Empty),
                        entity_email = reader.getSafeValue(_m_COL_EMAIL, string.Empty),
                        entity_city = reader.getSafeValue(_m_COL_CITY, string.Empty),
                        entity_address = reader.getSafeValue(_m_COL_ADDRESS, string.Empty),
                        entity_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                    });
                }
                return ResponseGetItem<EntityItem>.NotFound();
            }
            catch (MySqlException ex) {
                return ResponseGetItem<EntityItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all entity items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{EntityItem}"/> containing all entity items and any error message.</returns>
    public ResponseGetAllItems<EntityItem> getAllItems() {
        var items = SDatabaseModel.getAllRowsInTable<EntityItem>(_m_TBL_NAME, _m_COL_NAME);
        return ResponseGetAllItems<EntityItem>.Success(items);
    }

    /// <summary>
    /// Retrieves a collection of entities that are not archived.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="EntityItem"/> objects  that are not archived. The
    /// collection will be empty if no such entities exist.</returns>
    public ObservableCollection<EntityItem> getNonArchivedEntities() {
        return SDatabaseModel.getAllRowsInTable<EntityItem>(_m_TBL_NAME, _m_COL_NAME, _m_COL_ARCHIVE);
    }

    /// <summary>
    /// Saves the specified <see cref="EntityItem"/> to the database.
    /// </summary>
    /// <param name="item">The <see cref="EntityItem"/> to save. Cannot be <c>null</c>.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved item and any error message.
    /// Returns a response with ID 0 if the operation fails, the item is <c>null</c>, or a duplicate item
    /// is detected.</returns>
    public ResponseSaveItem saveItem(EntityItem item) {
        if (item == null) {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE);
        }

        int item_id = item.entity_id;
        string query;
        
        // Determine if we're inserting or updating
        bool isInsert = (item_id == 0);
        
        if (isInsert) {
            // Check for duplicate name before inserting
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.entity_name)) {
                return ResponseSaveItem.Failure(EErrors.ALREADY_EXISTS);
            }
            
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}) " +
                $"VALUES (@name, @phone, @email, @city, @address); " +
                $"SELECT LAST_INSERT_ID(); ";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_PHONE} = @phone, {_m_COL_EMAIL} = @email, " +
                $"{_m_COL_CITY} = @city, {_m_COL_ADDRESS} = @address " +
                $"WHERE {_m_COL_ID} = @id";
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
                    cmd.Parameters.AddWithValue("@name", item.entity_name.Trim());
                    cmd.Parameters.AddWithValue("@phone", item.entity_phone.Trim());
                    cmd.Parameters.AddWithValue("@email", item.entity_email.Trim());
                    cmd.Parameters.AddWithValue("@city", item.entity_city.Trim());
                    cmd.Parameters.AddWithValue("@address", item.entity_address.Trim());
                    return Convert.ToInt32(cmd.ExecuteScalar());
                });
            }
            else {
                // For UPDATE operations, we just execute the command
                executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@id", item_id);
                    cmd.Parameters.AddWithValue("@name", item.entity_name.Trim());
                    cmd.Parameters.AddWithValue("@phone", item.entity_phone.Trim());
                    cmd.Parameters.AddWithValue("@email", item.entity_email.Trim());
                    cmd.Parameters.AddWithValue("@city", item.entity_city.Trim());
                    cmd.Parameters.AddWithValue("@address", item.entity_address.Trim());
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
