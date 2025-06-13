using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts; 

public class ReceiptModel : ABaseModel,
    ICrudOperation<ReceiptItem> {

    private const string m_TBL_NAME = "receipts";
    private const string m_COL_ID = "receipt_id";
    private const string m_COL_RECEIPT_TOTAL_PRICE = "receipt_total_price";
    private const string m_COL_RECEIPT_DATE_CREATED = "receipt_date_created";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_ID, string.Empty, m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ReceiptItem? getItemByID(string id) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID}, " +
                $"{m_COL_RECEIPT_TOTAL_PRICE}, " +
                $"{m_COL_RECEIPT_DATE_CREATED} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ReceiptItem {
                    receipt_id = reader.GetSafeValue<int>(m_COL_ID),
                    receipt_total_price = reader.GetSafeValue<decimal>(m_COL_RECEIPT_TOTAL_PRICE),
                    receipt_date_created = reader.GetSafeValue(m_COL_RECEIPT_DATE_CREATED, DateOnly.MinValue).ToString("dd.MM.yyyy")
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ReceiptItem> getTable() {
        return SDatabaseModel.getAllData<ReceiptItem>(this, m_TBL_NAME);
    }


    public bool saveItem(ReceiptItem item) {

        if (item == null) {
            return false;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;
        
        DateTime parsed_date = DateTime.ParseExact(item.receipt_date_created, "dd.MM.yyyy", null);
        string mysql_formatted_date = parsed_date.ToString("yyyy-MM-dd");

        if (item.receipt_id == 0) {
            query = $"INSERT INTO {m_TBL_NAME} (" +
                $"{m_COL_RECEIPT_TOTAL_PRICE}, " +
                $"{m_COL_RECEIPT_DATE_CREATED}) " +
                $"VALUES (@total_price, @date_created); " +
                $"SELECT LAST_INSERT_ID();"; // Get ID immediately, need the same connection open
        }
        else {
            query = $"UPDATE {m_TBL_NAME} " +
                $"SET {m_COL_RECEIPT_TOTAL_PRICE} = @total_price, {m_COL_RECEIPT_DATE_CREATED} = @date_created " +
                $"WHERE {m_COL_ID} = @id;";
        }

        try {
            using MySqlCommand cmd = new(query, conn);
            if (item.receipt_id != 0) {
                cmd.Parameters.AddWithValue("@id", item.receipt_id);
            }
            cmd.Parameters.AddWithValue("@total_price", item.receipt_total_price);
            cmd.Parameters.AddWithValue("@date_created", mysql_formatted_date);
            
            if (item.receipt_id == 0) {
                // For INSERT, get the ID directly
                var newId = Convert.ToInt32(cmd.ExecuteScalar());
                item.receipt_id = newId;
            }
            else {
                // For UPDATE
                cmd.ExecuteNonQuery();
            }

            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }


    public int saveAndGetLastID(ReceiptItem item) {

        if (item == null) {
            return 0;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;

        DateTime parsed_date = DateTime.ParseExact(item.receipt_date_created, "dd.MM.yyyy", null);
        string mysql_formatted_date = parsed_date.ToString("yyyy-MM-dd");

        if (item.receipt_id == 0) {
            query = $"INSERT INTO {m_TBL_NAME} (" +
                $"{m_COL_RECEIPT_TOTAL_PRICE}, " +
                $"{m_COL_RECEIPT_DATE_CREATED}) " +
                $"VALUES (@total_price, @date_created); " +
                $"SELECT LAST_INSERT_ID();"; // Get ID immediately, need the same connection open
        }
        else {
            query = $"UPDATE {m_TBL_NAME} " +
                $"SET {m_COL_RECEIPT_TOTAL_PRICE} = @total_price, " +
                $"{m_COL_RECEIPT_DATE_CREATED} = @date_created " +
                $"WHERE {m_COL_ID} = @id;";
        }

        try {
            using MySqlCommand cmd = new(query, conn);
            if (item.receipt_id != 0) {
                cmd.Parameters.AddWithValue("@id", item.receipt_id);
            }
            cmd.Parameters.AddWithValue("@total_price", item.receipt_total_price);
            cmd.Parameters.AddWithValue("@date_created", mysql_formatted_date);

            if (item.receipt_id == 0) {
                // For INSERT, get the ID directly
                item.receipt_id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else {
                // For UPDATE
                cmd.ExecuteNonQuery();
            }
            return item.receipt_id;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }


    public ObservableCollection<int> getRowsID() {

        using MySqlConnection? conn = _m_conn.openConnection();

        ObservableCollection<int> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID} FROM {m_TBL_NAME};",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(reader.GetSafeValue<int>(m_COL_ID));
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }


    public ObservableCollection<string> getExistingYear() {

        using MySqlConnection? conn = _m_conn.openConnection();
        ObservableCollection<string> items = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT DISTINCT YEAR({m_COL_RECEIPT_DATE_CREATED}) " +
                $"AS year " +
                $"FROM {m_TBL_NAME} " +
                $"ORDER BY year DESC;",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(reader.GetSafeValue<int>("year").ToString());
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }
}
