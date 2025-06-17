using Mams.src.clients;
using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.suppliers;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.entities;

public class EntityModel : ABaseModel,
    ICrudOperation<EntityItem> {

    private const string m_TBL_NAME  = "entities";
    private const string m_COL_ID = "entity_id";
    private const string m_COL_NAME = "entity_name";
    private const string m_COL_PHONE = "entity_phone";
    private const string m_COL_EMAIL = "entity_email";
    private const string m_COL_CITY = "entity_city";
    private const string m_COL_ADDRESS = "entity_address";
    private const string m_COL_ARCHIVE = "entity_archive";

    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {

        // Cascad delete the supplier and client items
        if (delete_type == EDeleteItemOperation.HARD_DELETE) {

            ClientModel client_model = new();
            SupplierModel supplier_model = new();

            client_model.deleteClientWithEntityFK(id);
            supplier_model.deleteSupplierWithEntityFK(id);
        }

        return SDatabaseModel.deleteItem(this, id, m_COL_ID, m_COL_ARCHIVE, m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public EntityItem? getItemByID(string id) {
        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID}, {m_COL_NAME}, {m_COL_PHONE}, {m_COL_EMAIL}, {m_COL_CITY}, {m_COL_ADDRESS}, {m_COL_ARCHIVE} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_ID} = @id ",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new EntityItem {
                    entity_id = reader.GetSafeValue<int>(m_COL_ID),
                    entity_name = reader.GetSafeValue(m_COL_NAME, string.Empty),
                    entity_phone = reader.GetSafeValue(m_COL_PHONE, string.Empty),
                    entity_email = reader.GetSafeValue(m_COL_EMAIL, string.Empty),
                    entity_city = reader.GetSafeValue(m_COL_CITY, string.Empty),
                    entity_address = reader.GetSafeValue(m_COL_ADDRESS, string.Empty),
                    entity_archive = reader.GetSafeValue(m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<EntityItem> getTable() {
        return SDatabaseModel.getAllData<EntityItem>(this, m_TBL_NAME);
    }


    public int saveItem(EntityItem item) {
        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;

        if (item.entity_id == 0) {

            if (isItemPresentInDatabase(m_TBL_NAME, m_COL_NAME, item.entity_name)) {
                return 0;
            }

            query = $"INSERT INTO {m_TBL_NAME} ({m_COL_NAME}, {m_COL_PHONE}, {m_COL_EMAIL}, {m_COL_CITY}, {m_COL_ADDRESS}) " +
                $"VALUES (@name, @phone, @email, @city, @address)";
        }
        else {
            query = $"UPDATE {m_TBL_NAME} " +
                $"SET {m_COL_NAME} = @name, {m_COL_PHONE} = @phone, {m_COL_EMAIL} = @email, {m_COL_CITY} = @city, {m_COL_ADDRESS} = @address " +
                $"WHERE {m_COL_ID} = @id";
        }

        try {
            using MySqlCommand cmd = new(query, conn);
            if (item.entity_id != 0) {
                cmd.Parameters.AddWithValue("@id", item.entity_id);
            }
            cmd.Parameters.AddWithValue("@name", item.entity_name);
            cmd.Parameters.AddWithValue("@phone", item.entity_phone);
            cmd.Parameters.AddWithValue("@email", item.entity_email);
            cmd.Parameters.AddWithValue("@city", item.entity_city);
            cmd.Parameters.AddWithValue("@address", item.entity_address);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }


    //public override List<string> searchItems(string search) {

    //    using MySqlConnection? conn = _m_conn.openConnection();

    //    var results = new List<string>();

    //    try {
    //        using MySqlCommand cmd = new(
    //            $"SELECT DISTINCT {_m_COL_NAME} " +
    //            $"FROM {_m_TBL_NAME} " +
    //            //$"WHERE {_m_COL_ARCHIVE} = '' " + // Only non-archived items
    //            $"ORDER BY {_m_COL_NAME};",
    //            conn
    //        );

    //        using MySqlDataReader reader = cmd.ExecuteReader();
    //        while (reader.Read()) {
    //            results.Add(reader.GetString(_m_COL_NAME));
    //        }
    //        return results;
    //    }
    //    catch (MySqlException ex) {
    //        MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
    //        return results;
    //    }
    //}
}
