using Mams.src.helpers;
using Mams.src.interfaces;
using Mams.src.items;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.models;

public class EntityModel : 
    ABaseSearchModel,
    ISearchItemsByName,
    ICRUDItem<EntityItem> {

    private const string _m_TBL_NAME = "entities";
    private const string _m_COL_ID = "entity_id";
    private const string _m_COL_NAME = "entity_name";
    private const string _m_COL_PHONE = "entity_phone";
    private const string _m_COL_EMAIL = "entity_email";
    private const string _m_COL_CITY = "entity_city";
    private const string _m_COL_ADDRESS = "entity_address";
    private const string _m_COL_ARCHIVE = "entity_archive";


    public bool deleteItem(string id) {
        return DeleteItemModel.deleteItem(this, id, _m_COL_ID, _m_TBL_NAME);
    }
    public bool deleteItem(int id) {
        return deleteItem(id.ToString());
    }


    public EntityItem? getItem(string search) {
        throw new NotImplementedException();

        //using MySqlConnection? conn = _m_conn.openConnection();

        //try {
        //    // First try exact name match
        //    using MySqlCommand cmd = new(
        //        $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_QUANTITY} " +
        //        $"FROM {_m_TBL_NAME} " +
        //        $"WHERE {_m_COL_NAME} = @search " +
        //        $"UNION " +
        //        $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_QUANTITY} " +
        //        $"FROM {_m_TBL_NAME} " +
        //        $"WHERE {_m_COL_ID} = @search AND NOT EXISTS (" +
        //            $"SELECT 1 FROM {_m_TBL_NAME} WHERE {_m_COL_NAME} = @search" +
        //        $") LIMIT 1;",
        //        conn
        //    );

        //    cmd.Parameters.AddWithValue("@search", search);
        //    using MySqlDataReader reader = cmd.ExecuteReader();

        //    if (reader.Read()) {
        //        return new InventoryItem(
        //            reader.GetInt32(_m_COL_ID),
        //            reader.GetString(_m_COL_NAME),
        //            reader.GetInt32(_m_COL_QUANTITY)
        //        );
        //    }
        //    return null;
        //}
        //catch (MySqlException ex) {
        //    MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
        //    return null;
        //}
    }

    public EntityItem? getItemByID(string id) {
        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}, {_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id ",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new EntityItem {
                    entity_id = reader.GetSafeValue<int>(_m_COL_ID),
                    entity_name = reader.GetSafeValue<string>(_m_COL_NAME, string.Empty),
                    entity_phone = reader.GetSafeValue<string>(_m_COL_PHONE, string.Empty),
                    entity_email = reader.GetSafeValue<string>(_m_COL_EMAIL, string.Empty),
                    entity_city = reader.GetSafeValue<string>(_m_COL_CITY, string.Empty),
                    entity_address = reader.GetSafeValue<string>(_m_COL_ADDRESS, string.Empty),
                    entity_archive = reader.GetSafeValue<DateTime>(_m_COL_ARCHIVE, DateTime.MinValue).ToString()
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
        return GetTableModel.getTableData<EntityItem>(this, _m_TBL_NAME);
    }

    public bool saveItem(EntityItem item) {
        using MySqlConnection? conn = _m_conn.openConnection();

        string query = String.Empty;

        if (item.entity_id == 0) {
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}) " +
                $"VALUES (@name, @phone, @email, @city, @address)";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_PHONE} = @phone, {_m_COL_EMAIL} = @email, {_m_COL_CITY} = @city, {_m_COL_ADDRESS} = @address " +
                $"WHERE {_m_COL_ID} = @id";
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
            return false;
        }
    }

    public override List<string> searchItems(string search) {
        throw new NotImplementedException();
    }

    public List<string> searchItemsByName(string search) {
        throw new NotImplementedException();
    }
}
