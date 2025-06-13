using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

public class ReceiptSupplierModel : ABaseModel,
    ICrudOperation<ReceiptSupplierItem> {

    public const string m_TBL_NAME = "receipts_suppliers";
    public const string m_COL_FK_RECEIPT = "fk_receipt_id";
    public const string m_COL_FK_SUPPLIER = "fk_supplier_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_FK_RECEIPT, "", m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    // Parameter `id` is expected to be a receipt ID
    public ReceiptSupplierItem? getItemByID(string id) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_SUPPLIER} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @id;",
                conn
            );
            cmd.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {
                return new ReceiptSupplierItem {
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.GetSafeValue<int>(m_COL_FK_SUPPLIER)
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ReceiptSupplierItem> getTable() {

        ObservableCollection<ReceiptSupplierItem> items = new();

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_SUPPLIER} " +
                $"FROM {m_TBL_NAME};",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptSupplierItem {
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.GetSafeValue<int>(m_COL_FK_SUPPLIER)
                });
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }


    public bool saveItem(ReceiptSupplierItem item) {

        if (item == null || item.fk_receipt_id == 0 || item.fk_supplier_id == 0) {
            return false;
        }

        using MySqlConnection? conn = _m_conn.openConnection();
        
        try {
            // Check if the item already exists in the database
            using MySqlCommand existCmd = new(
                $"SELECT COUNT(*) FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;",
                conn
            );
            existCmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            
            int exists = Convert.ToInt32(existCmd.ExecuteScalar());

            string query;
            if (exists == 0) {
                // Insert new record
                query = $"INSERT INTO {m_TBL_NAME} " +
                    $"({m_COL_FK_RECEIPT}, {m_COL_FK_SUPPLIER}) " +
                    $"VALUES (@fk_receipt, @fk_supplier);";
            }
            else {
                // Update existing record
                query = $"UPDATE {m_TBL_NAME} " +
                    $"SET {m_COL_FK_SUPPLIER} = @fk_supplier " +
                    $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;";
            }

            using MySqlCommand cmd = new(query, conn);
            cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            cmd.Parameters.AddWithValue("@fk_supplier", item.fk_supplier_id);
            return cmd.ExecuteNonQuery() > 0;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }


    public ObservableCollection<ReceiptSupplierItem> getListItemWithSuppliertID(string supplier_id) {

        ObservableCollection<ReceiptSupplierItem> items = new();

        if (string.IsNullOrEmpty(supplier_id)) {
            return items;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_SUPPLIER} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_SUPPLIER} = @supplier_id;",
                conn
            );

            cmd.Parameters.AddWithValue("@supplier_id", supplier_id);

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptSupplierItem {
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.GetSafeValue<int>(m_COL_FK_SUPPLIER)
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
