using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.clients;

public class ClientModel : ABaseModel,
    ICrudOperation<ClientItem> {

    public const string m_TBL_NAME = "clients";
    public const string m_COL_ID = "client_id";
    public const string m_COL_FK_ENTITY = "fk_entity_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_ID, "", m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ClientItem? getItemByID(string id) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID}, {m_COL_FK_ENTITY} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ClientItem {
                    client_id = reader.GetSafeValue<int>(m_COL_ID),
                    fk_entity_id = reader.GetSafeValue<int>(m_COL_FK_ENTITY, 0)
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ClientItem> getListClientWithEntityFK(string fk_entity) {

        ObservableCollection<ClientItem> items = new();

        if (string.IsNullOrEmpty(fk_entity)) {
            return items;
        }

        using MySqlConnection? conn = _m_conn.openConnection();
        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID}, {m_COL_FK_ENTITY} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_ENTITY} = @fk_entity;",
                conn
            );

            cmd.Parameters.AddWithValue("@fk_entity", fk_entity);

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ClientItem {
                    client_id = reader.GetSafeValue<int>(m_COL_ID),
                    fk_entity_id = reader.GetSafeValue<int>(m_COL_FK_ENTITY)
                });
            }

            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }


    public bool deleteListClientWithEntityFK(string fk_entity) {

        ObservableCollection<ClientItem> items = getListClientWithEntityFK(fk_entity);

        if (items.Count == 0) {
            return false;
        }
        foreach (ClientItem item in items) {
            deleteItem(item.client_id);
        }
        return true;
    }


    public ObservableCollection<ClientItem> getTable() {
        return SDatabaseModel.getAllData<ClientItem>(this, m_TBL_NAME);
    }


    public bool saveItem(ClientItem item) {

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;

        if (item.client_id == 0) {

            if (checkIfItemExist(m_TBL_NAME, m_COL_FK_ENTITY, item.fk_entity_id.ToString())) {
                return false;
            }

            query = $"INSERT INTO {m_TBL_NAME} ({m_COL_FK_ENTITY}) " +
                $"VALUES (@fk_entity);";
        }
        else {
            query = $"UPDATE {m_TBL_NAME} " +
                $"SET {m_COL_FK_ENTITY} = @fk_entity " +
                $"WHERE {m_COL_ID} = @id;";
        }

        try {
            using MySqlCommand cmd = new(query, conn);
            if (item.client_id != 0) {
                cmd.Parameters.AddWithValue("@id", item.client_id);
            }
            cmd.Parameters.AddWithValue("@fk_entity", item.fk_entity_id);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}
