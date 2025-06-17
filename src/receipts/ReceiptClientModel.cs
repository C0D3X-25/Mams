using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

public class ReceiptClientModel : ABaseModel,
    ICrudOperation<ReceiptClientItem> {

    private const string m_TBL_NAME = "receipts_clients";
    private const string m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string m_COL_FK_CLIENT = "fk_client_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_FK_RECEIPT, string.Empty, m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ReceiptClientItem? getItemByID(string id) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_CLIENT} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @id;",
                conn
            );
            cmd.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {
                return new ReceiptClientItem {
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    fk_client_id = reader.GetSafeValue<int>(m_COL_FK_CLIENT)
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ReceiptClientItem> getTable() {

        ObservableCollection<ReceiptClientItem> items = new();

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_CLIENT} " +
                $"FROM {m_TBL_NAME};",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptClientItem {
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    fk_client_id = reader.GetSafeValue<int>(m_COL_FK_CLIENT)
                });
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }


    public bool saveItem(ReceiptClientItem item) {

        if (item == null 
            || item.fk_receipt_id == 0 
            || item.fk_client_id == 0)
        {
            return false;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            // Check if the item already exists in the database
            using MySqlCommand exist_cmd = new(
                $"SELECT COUNT(*) FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;",
                conn
            );
            exist_cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);

            int exists = Convert.ToInt32(exist_cmd.ExecuteScalar());

            string query;
            if (exists == 0) {
                // Insert new record
                query = $"INSERT INTO {m_TBL_NAME} " +
                    $"({m_COL_FK_RECEIPT}, {m_COL_FK_CLIENT}) " +
                    $"VALUES (@fk_receipt, @fk_client);";
            }
            else {
                // Update existing record
                query = $"UPDATE {m_TBL_NAME} " +
                    $"SET {m_COL_FK_CLIENT} = @fk_client " +
                    $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;";
            }

            using MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            cmd.Parameters.AddWithValue("@fk_client", item.fk_client_id);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }


    public ObservableCollection<ReceiptClientItem> getListItemWithReceiptID(string fk_receipt) {

        ObservableCollection<ReceiptClientItem> items = new();

        if (string.IsNullOrEmpty(fk_receipt)) {
            return items;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_CLIENT} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;",
                conn
            );

            cmd.Parameters.AddWithValue("@fk_receipt", fk_receipt);

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptClientItem {
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    fk_client_id = reader.GetSafeValue<int>(m_COL_FK_CLIENT)
                });
            }

            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }
}
