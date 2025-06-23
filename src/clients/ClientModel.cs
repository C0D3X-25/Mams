using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Transactions;
using System.Windows;

namespace Mams.src.clients;

public class ClientModel : ABaseModel,
    ICrudOperation<ClientItem> {

    private const string _m_TBL_NAME = "clients";
    private const string _m_COL_ID = "client_id";
    private const string _m_COL_FK_ENTITY = "fk_entity_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, string.Empty, _m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ClientItem? getItemByID(string id) {

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ClientItem {
                    client_id = reader.GetSafeValue<int>(_m_COL_ID),
                    fk_entity_id = reader.GetSafeValue<int>(_m_COL_FK_ENTITY, 0)
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

        transaction = conn?.BeginTransaction();

        try {
            using MySqlCommand cmd = new(query, conn, transaction);

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

            transaction?.Commit();
            return item_id;
        }
        catch (MySqlException ex) {
            transaction?.Rollback();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }


    public ClientItem? getClientWithEntityFK(string fk_entity) {

        if (string.IsNullOrEmpty(fk_entity)) {
            return null;
        }

        ClientItem item = new();

        
        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_ENTITY} = @fk_entity;",
                conn
            );

            cmd.Parameters.AddWithValue("@fk_entity", fk_entity);

            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                item.client_id = reader.GetSafeValue<int>(_m_COL_ID);
                item.fk_entity_id = reader.GetSafeValue<int>(_m_COL_FK_ENTITY);
            }

            return item;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return item;
        }
    }


    public bool deleteClientWithEntityFK(string fk_entity) {

        if (string.IsNullOrEmpty(fk_entity)) {
            return false;
        }

        ClientItem? item = getClientWithEntityFK(fk_entity);

        if (item == null) {
            return false;
        }

        return deleteItem(item.client_id);
    }
}
