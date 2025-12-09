using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.suppliers;

/// <summary>
/// Represents a model for managing supplier data, including CRUD operations and database interactions.
/// </summary>
public class SupplierModel : ABaseModel,
    ICrudOperation<SupplierItem> {

    private const string _m_TBL_NAME = "suppliers";
    private const string _m_COL_ID = "supplier_id";
    private const string _m_COL_FK_ENTITY = "fk_entity_id";

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, string.Empty, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="SupplierItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the supplier item. Cannot be null or empty.</param>
    /// <returns>A <see cref="ResponseGetItem{SupplierItem}"/> containing the supplier item and any error message.</returns>
    public ResponseGetItem<SupplierItem> getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return ResponseGetItem<SupplierItem>.Failure("Invalid ID provided.");
        }

        return executeWithConnection(connection => {
            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = @id;",
                    connection,
                    m_transaction
                );

                cmd.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) {
                    return ResponseGetItem<SupplierItem>.Success(new SupplierItem {
                        supplier_id = reader.getSafeValue<int>(_m_COL_ID),
                        fk_entity_id = reader.getSafeValue<int>(_m_COL_FK_ENTITY, 0)
                    });
                }
                return ResponseGetItem<SupplierItem>.NotFound();
            }
            catch (MySqlException ex) {
                return ResponseGetItem<SupplierItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all supplier items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{SupplierItem}"/> containing all supplier items and any error message.</returns>
    public ResponseGetAllItems<SupplierItem> getAllItems() {
        var items = SDatabaseModel.getAllRowsInTable<SupplierItem>(_m_TBL_NAME);
        return ResponseGetAllItems<SupplierItem>.Success(items);
    }

    /// <summary>
    /// Saves the specified supplier item to the database.
    /// </summary>
    /// <param name="item">The <see cref="SupplierItem"/> object to save. Must not be null.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved supplier item and any error message.
    /// Returns a response with ID 0 if the operation fails, the item is null, or an identical item is
    /// already present in the database.</returns>
    public ResponseSaveItem saveItem(SupplierItem item) {
        if (item == null) {
            return ResponseSaveItem.Failure("Item cannot be null.");
        }

        int item_id = item.supplier_id;
        string query;
        
        // Determine if we're inserting or updating
        bool isInsert = (item_id == 0);
        
        if (isInsert) {
            // Check for duplicate before inserting
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_FK_ENTITY, item.fk_entity_id.ToString())) {
                return ResponseSaveItem.Failure("A supplier with the same entity already exists.");
            }
            
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_FK_ENTITY}) " +
                $"VALUES (@fk_entity); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_FK_ENTITY} = @fk_entity " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        startTransaction();

        try {
            if (isInsert) {
                // For INSERT operations, we need to return the new ID
                item_id = executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@fk_entity", item.fk_entity_id);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                });
            }
            else {
                // For UPDATE operations, we just execute the command
                executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@id", item_id);
                    cmd.Parameters.AddWithValue("@fk_entity", item.fk_entity_id);
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
    /// Retrieves a <see cref="SupplierItem"/> object based on the specified foreign key value.
    /// </summary>
    /// <param name="fk_entity">The foreign key value used to query the supplier. This parameter cannot be null or empty.</param>
    /// <returns>A <see cref="SupplierItem"/> object containing supplier details if a matching record is found;  otherwise, <see
    /// langword="null"/>.</returns>
    public SupplierItem? getSupplierWithEntityFK(string fk_entity) {
        if (!SDataValidation.isIdValid(fk_entity)) {
            return null;
        }

        return executeWithConnection<SupplierItem?>(connection => {
            try {
                SupplierItem item = new();
                
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_FK_ENTITY} = @fk_entity;",
                    connection,
                    m_transaction
                );

                cmd.Parameters.AddWithValue("@fk_entity", fk_entity);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) {
                    item.supplier_id = reader.getSafeValue<int>(_m_COL_ID);
                    item.fk_entity_id = reader.getSafeValue<int>(_m_COL_FK_ENTITY);
                    return item;
                }
                
                // If we get here, no matching record was found
                return item.supplier_id > 0 ? item : null;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return null;
            }
        });
    }

    /// <summary>
    /// Deletes a supplier associated with the specified foreign key.
    /// </summary>
    /// <param name="fk_entity">The foreign key of the entity associated with the supplier to be deleted. Must not be null or empty.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteSupplierWithEntityFK(string fk_entity) {
        if (string.IsNullOrEmpty(fk_entity)) {
            return ResponseDeleteItem.Failure("Foreign key cannot be null or empty.");
        }

        SupplierItem? item = getSupplierWithEntityFK(fk_entity);

        if (item == null) {
            return ResponseDeleteItem.Failure("Supplier not found.");
        }

        return deleteItem(item.supplier_id.ToString());
    }
}
