using Mams.src.databaseOperations;
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
    /// <returns><see langword="true"/> if the item was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, string.Empty, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ClientItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item to retrieve. Cannot be null or empty.</param>
    /// <returns>A <see cref="ClientItem"/> object representing the item with the specified identifier,  or <see
    /// langword="null"/> if no matching item is found.</returns>
    public ClientItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ClientItem {
                    client_id = reader.getSafeValue<int>(_m_COL_ID),
                    fk_entity_id = reader.getSafeValue<int>(_m_COL_FK_ENTITY, 0)
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
    public ObservableCollection<ClientItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<ClientItem>(_m_TBL_NAME);
    }

    /// <summary>
    /// Saves a receipt item to the database. If the item's ID is 0, creates a new record;
    /// otherwise updates the existing record.
    /// </summary>
    /// <param name="item">The ClientItem to save</param>
    /// <returns>The ID of the saved receipt; 0 if the operation failed</returns>
    public int saveItem(ClientItem item) {

        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.client_id;

        if (item_id == 0) {

            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_FK_ENTITY, item.fk_entity_id.ToString())) {
                return 0;
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
            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@fk_entity", item.fk_entity_id);

            if (item_id == 0) {
                item_id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else {
                cmd.ExecuteNonQuery();
            }

            commitTransaction();
            return item_id;
        }
        catch (MySqlException ex) {
            rollbackTransaction();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
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

        ClientItem item = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_ENTITY} = @fk_entity;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@fk_entity", fk_entity);

            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                item.client_id = reader.getSafeValue<int>(_m_COL_ID);
                item.fk_entity_id = reader.getSafeValue<int>(_m_COL_FK_ENTITY);
            }

            return item;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return item;
        }
    }

    /// <summary>
    /// Deletes a client associated with the specified foreign key.
    /// </summary>
    /// <param name="fk_entity">The foreign key of the entity associated with the client to be deleted. Must not be null or empty.</param>
    /// <returns><see langword="true"/> if the client was successfully deleted;  otherwise, <see langword="false"/> if the
    /// foreign key is invalid,  no client is found, or the deletion fails.</returns>
    public bool deleteClientWithEntityFK(string fk_entity) {

        if (!SDataValidation.isIdValid(fk_entity)) {
            return false;
        }

        ClientItem? item = getClientWithEntityFK(fk_entity);

        if (item == null) {
            return false;
        }

        return deleteItem(item.client_id.ToString());
    }
}
