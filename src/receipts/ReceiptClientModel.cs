using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

public class ReceiptClientModel : ABaseModel,
    ICrudOperation<ReceiptClientItem> {

    public const string m_TBL_NAME = "receipts_clients";
    public const string m_COL_FK_RECEIPT = "fk_receipt_id";
    public const string m_COL_FK_CLIENT = "fk_client_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        throw new NotImplementedException();
    }


    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        throw new NotImplementedException();
    }


    public bool deleteItemWithReceiptFK(int fk_receipt, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {

        if (fk_receipt == 0) {
            return false;
        }
        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"DELETE FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;",
                conn
            );
            cmd.Parameters.AddWithValue("@fk_receipt", fk_receipt);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }


    public ReceiptClientItem? getItemByID(string id) {
        throw new NotImplementedException();
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
        if (item == null) {
            return false;
        }
        if (item.fk_receipt_id == 0 || item.fk_client_id == 0) {
            return false;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            // Check if the item already exists in the database
            {
                using MySqlCommand cmd = new(
                    $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_CLIENT} " +
                    $"FROM {m_TBL_NAME} " +
                    $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt " +
                    $"AND {m_COL_FK_CLIENT} = @fk_supplier;",
                    conn
                );

                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows) {
                    return false;
                }
            }

            // If not, insert the item into the database
            {
                using MySqlCommand cmd = new(
                    $"INSERT INTO {m_TBL_NAME} " +
                    $"({m_COL_FK_RECEIPT}, {m_COL_FK_CLIENT}) " +
                    $"VALUES (@fk_receipt, @fk_supplier);",
                    conn
                );
                cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
                cmd.Parameters.AddWithValue("@fk_supplier", item.fk_client_id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }


    public ObservableCollection<ReceiptClientItem> getListItemWithReceiptFK(string fk_receipt) {

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


    public bool deleteListItemWithFkEntity(string fk_receipt) {

        ObservableCollection<ReceiptClientItem> items = getListItemWithReceiptFK(fk_receipt);

        if (items.Count == 0) {
            return false;
        }
        foreach (ReceiptClientItem item in items) {
            deleteItemWithReceiptFK(item.fk_receipt_id);
        }
        return true;
    }
}
