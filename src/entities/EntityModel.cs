using Mams.src.clients;
using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.suppliers;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Mams.src.entities;

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

    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public EntityItem? getItemByID(string id) {
        
        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}, {_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id ",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new EntityItem {
                    entity_id = reader.GetSafeValue<int>(_m_COL_ID),
                    entity_name = reader.GetSafeValue(_m_COL_NAME, string.Empty),
                    entity_phone = reader.GetSafeValue(_m_COL_PHONE, string.Empty),
                    entity_email = reader.GetSafeValue(_m_COL_EMAIL, string.Empty),
                    entity_city = reader.GetSafeValue(_m_COL_CITY, string.Empty),
                    entity_address = reader.GetSafeValue(_m_COL_ADDRESS, string.Empty),
                    entity_archive = reader.GetSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
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
        return SDatabaseModel.getAllRowsInTable<EntityItem>(_m_TBL_NAME);
    }


    public int saveItem(EntityItem item) {
        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.entity_id;

        if (item_id == 0) {

            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.entity_name)) {
                return 0;
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}) " +
                $"VALUES (@name, @phone, @email, @city, @address); " +
                $"SELECT LAST_INSERT_ID(); ";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_PHONE} = @phone, {_m_COL_EMAIL} = @email, {_m_COL_CITY} = @city, {_m_COL_ADDRESS} = @address " +
                $"WHERE {_m_COL_ID} = @id";
        }

        startTransaction();

        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@name", item.entity_name);
            cmd.Parameters.AddWithValue("@phone", item.entity_phone);
            cmd.Parameters.AddWithValue("@email", item.entity_email);
            cmd.Parameters.AddWithValue("@city", item.entity_city);
            cmd.Parameters.AddWithValue("@address", item.entity_address);

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


    //public override List<string> searchItems(string search) {

    //    using MySqlConnection? conn = _m_conn.openConnection();

    //    var results = new List<string>();

    //    try {
    //        using MySqlCommand cmd = new(
    //            $"SELECT DISTINCT {__m_COL_NAME} " +
    //            $"FROM {__m_TBL_NAME} " +
    //            //$"WHERE {__m_COL_ARCHIVE} = '' " + // Only non-archived items
    //            $"ORDER BY {__m_COL_NAME};",
    //            conn
    //        );

    //        using MySqlDataReader reader = cmd.ExecuteReader();
    //        while (reader.Read()) {
    //            results.Add(reader.GetString(__m_COL_NAME));
    //        }
    //        return results;
    //    }
    //    catch (MySqlException ex) {
    //        MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
    //        return results;
    //    }
    //}
}
