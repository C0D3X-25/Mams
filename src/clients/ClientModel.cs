using Mams.src.databaseOperations;
using Mams.src.errors;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.clients;

/// <summary>
/// Represents a model for managing client data in the database.
/// </summary>
public class ClientModel : ABaseModel,
    ICrudOperation<ClientItem> {

    private const string _m_TBL_NAME = "clients";
    private const string _m_COL_ID = "client_id";
    private const string _m_COL_FK_ENTITY = "fk_entity_id";

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For safe delete operations, additional checks may be performed to ensure data integrity.</remarks>
    /// <param name="id">The unique identifier of the item to delete. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, string.Empty, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ClientItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item to retrieve. Cannot be null or empty.</param>
    /// <returns>A <see cref="ResponseGetItem{ClientItem}"/> containing the item with the specified identifier and any error message.</returns>
    public ResponseGetItem<ClientItem> getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return ResponseGetItem<ClientItem>.Failure(EErrors.INVALID_INPUT,
                $"ClientModel.getItemByID: Invalid ID provided '{id}'");
        }

        return executeWithConnection(connection => { 
            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = @id;",
                    connection
                );

                cmd.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read()) {
                    return ResponseGetItem<ClientItem>.Success(new ClientItem {
                        client_id = reader.getSafeValue<int>(_m_COL_ID),
                        fk_entity_id = reader.getSafeValue<int>(_m_COL_FK_ENTITY, 0)
                    });
                }
                return ResponseGetItem<ClientItem>.NotFound();
            }
            catch (MySqlException ex) {
                return ResponseGetItem<ClientItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all client items from the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{ClientItem}"/> containing all client items and any error message.</returns>
    public ResponseGetAllItems<ClientItem> getAllItems() {
        var items = SDatabaseModel.getAllRowsInTable<ClientItem>(_m_TBL_NAME);
        return ResponseGetAllItems<ClientItem>.Success(items);
    }

    /// <summary>
    /// Saves a receipt item to the database. If the item's ID is 0, creates a new record;
    /// otherwise updates the existing record.
    /// </summary>
    /// <param name="item">The ClientItem to save</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved receipt and any error message.
    /// Returns a response with ID 0 if the operation failed.</returns>
    public ResponseSaveItem saveItem(ClientItem item) {
        if (item == null) {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE,
                "ClientModel.saveItem: Item cannot be null");
        }

        int item_id = item.client_id;
        string query;
        
        // Determine if we're inserting or updating
        bool isInsert = (item_id == 0);
        
        if (isInsert) {
            // Check for duplicate before inserting
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_FK_ENTITY, item.fk_entity_id.ToString())) {
                return ResponseSaveItem.Failure(EErrors.ALREADY_EXISTS,
                    $"ClientModel.saveItem: Client with entity FK '{item.fk_entity_id}' already exists");
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
    /// Retrieves a <see cref="ClientItem"/> object based on the specified foreign key value.
    /// </summary>
    /// <param name="fk_entity">The foreign key value used to query the client. Cannot be null or empty.</param>
    /// <returns>A <see cref="ClientItem"/> object populated with client data if a matching record is found;  otherwise, <see
    /// langword="null"/> if the <paramref name="fk_entity"/> is null or empty, or if no matching record exists.</returns>
    public ClientItem? getClientWithEntityFK(string fk_entity) {
        if (!SDataValidation.isIdValid(fk_entity)) {
            return null;
        }

        return executeWithConnection<ClientItem?>(connection => {
            try {
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
                    return new ClientItem {
                        client_id = reader.getSafeValue<int>(_m_COL_ID),
                        fk_entity_id = reader.getSafeValue<int>(_m_COL_FK_ENTITY)
                    };
                }

                // Return empty item when no record is found
                return new ClientItem();
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return new ClientItem();
            }
        });
    }

    /// <summary>
    /// Deletes a client associated with the specified foreign key.
    /// </summary>
    /// <param name="fk_entity">The foreign key of the entity associated with the client to be deleted. Must not be null or empty.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.</returns>
    public ResponseDeleteItem deleteClientWithEntityFK(string fk_entity) {
        if (!SDataValidation.isIdValid(fk_entity)) {
            return ResponseDeleteItem.Failure(EErrors.INVALID_INPUT,
                $"ClientModel.deleteClientWithEntityFK: Invalid FK entity ID provided '{fk_entity}'");
        }

        ClientItem? item = getClientWithEntityFK(fk_entity);

        if (item == null) {
            return ResponseDeleteItem.Failure(EErrors.NOT_FOUND,
                $"ClientModel.deleteClientWithEntityFK: No client found with entity FK '{fk_entity}'");
        }

        return deleteItem(item.client_id.ToString());
    }
}
