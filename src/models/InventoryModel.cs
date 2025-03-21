using MySqlConnector;
using Mams.src.helpers;
using Mams.src.interfaces;
using Mams.src.items;
using System.Data;
using System.Windows;

namespace Mams.src.models;

public class InventoryModel : 
    ABaseSearchModel, 
    ISearchItemsByName, 
    ISearchItemsByID,
    ISaveItem<InventoryItem>,
    IGetItem<InventoryItem>,
    IGetTable,
    IDeleteItem 
    {

    private const string _m_TBL_NAME      = "inventory";
    private const string _m_COL_ID        = "id";
    private const string _m_COL_NAME      = "name";
    private const string _m_COL_QUANTITY  = "quantity";


    public DataTable? getTable() {
        return GetTableModel.getTable(this, _m_TBL_NAME);
    }


    public override List<string> searchItems(string search) {
        return CombineLists.getCombinedList(new() {
            searchItemsByID(search),
            searchItemsByName(search)
            
        });
    }


    public List<string> searchItemsByID(string search) {
        return _m_search_model.getItemList(search, _m_COL_ID, _m_TBL_NAME);
    }
    public List<string> searchItemsByID(int search) {
        return searchItemsByID(search.ToString());
    }


    public List<string> searchItemsByName(string search) {
        return _m_search_model.getItemList(search, _m_COL_NAME, _m_TBL_NAME);
    }


    public bool saveItem(InventoryItem item) {

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = "";

        if (item.m_id == 0) {
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_QUANTITY}) " +
                $"VALUES (@name, @quantity)";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_QUANTITY} = @quantity " +
                $"WHERE {_m_COL_ID} = @id";
        }

        try {
            using MySqlCommand cmd = new(query, conn);
            if (item.m_id != 0) {
                cmd.Parameters.AddWithValue("@id", item.m_id);
            }
            cmd.Parameters.AddWithValue("@name", item.m_name);
            cmd.Parameters.AddWithValue("@quantity", item.m_quantity);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }


    public bool deleteItem(string id) {
        return DeleteItemModel.deleteItem(this, id, _m_TBL_NAME);
    }
    public bool deleteItem(int id) {
        return deleteItem(id.ToString());
    }


    public InventoryItem? getItem(string search) {
        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            // First try exact name match
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_QUANTITY} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_NAME} = @search " +
                $"UNION " +
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_QUANTITY} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @search AND NOT EXISTS (" +
                    $"SELECT 1 FROM {_m_TBL_NAME} WHERE {_m_COL_NAME} = @search" +
                $") LIMIT 1;",
                conn
            );

            cmd.Parameters.AddWithValue("@search", search);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new InventoryItem(
                    reader.GetInt32(_m_COL_ID),
                    reader.GetString(_m_COL_NAME),
                    reader.GetInt32(_m_COL_QUANTITY)
                );
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }
}

