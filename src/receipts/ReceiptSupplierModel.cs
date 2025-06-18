using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

public class ReceiptSupplierModel : ABaseModel,
    ICrudOperation<ReceiptSupplierItem> {

    private const string m_TBL_NAME = "receipts_suppliers";
    private const string m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string m_COL_FK_SUPPLIER = "fk_supplier_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_FK_RECEIPT, string.Empty, m_TBL_NAME, delete_type);
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


    public int saveItem(ReceiptSupplierItem item) {
        if (item == null || item.fk_receipt_id == 0 || item.fk_supplier_id == 0) {
            return 0;
        }
        using MySqlConnection? conn = _m_conn.openConnection();
        if (conn == null) {
            return 0;
        }

        using var transaction = conn.BeginTransaction();

        try {
            string query = string.Empty;
            //int item_id = item.fk_receipt_id;

            //if (isItemPresentInDatabase(m_TBL_NAME, m_COL_FK_RECEIPT, item.fk_receipt_id.ToString())) {
                // Insert new record
                query = $"INSERT INTO {m_TBL_NAME} " +
                    $"({m_COL_FK_RECEIPT}, {m_COL_FK_SUPPLIER}) " +
                    $"VALUES (@fk_receipt, @fk_supplier); SELECT LAST_INSERT_ID();";
            //}
            //else {
            //    // Update existing record
            //    query = $"UPDATE {m_TBL_NAME} " +
            //        $"SET {m_COL_FK_SUPPLIER} = @fk_supplier " +
            //        $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;";
            //}

            using MySqlCommand cmd = new(query, conn, transaction);

            cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            cmd.Parameters.AddWithValue("@fk_supplier", item.fk_supplier_id);

            int item_id = Convert.ToInt32(cmd.ExecuteScalar());
            transaction.Commit();
            return item_id;
        }
        catch (MySqlException ex) {
            transaction.Rollback();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }


    public ObservableCollection<ReceiptSupplierItem> getListItemWithSupplierID(string supplier_id) {

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
