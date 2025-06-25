using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

public class ReceiptSupplierModel : ABaseModel,
    ICrudOperation<ReceiptSupplierItem> {

    private const string _m_TBL_NAME = "receipts_suppliers";
    private const string _m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string _m_COL_FK_SUPPLIER = "fk_supplier_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_FK_RECEIPT, string.Empty, _m_TBL_NAME, delete_type);
    }


    // Parameter `id` is expected to be a receipt ID
    public ReceiptSupplierItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_RECEIPT} = @id;",
                m_conn
            );
            cmd.Parameters.AddWithValue("@id", id);

            using MySqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read()) {
                return new ReceiptSupplierItem {
                    fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.getSafeValue<int>(_m_COL_FK_SUPPLIER)
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

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER} " +
                $"FROM {_m_TBL_NAME};",
                m_conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptSupplierItem {
                    fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.getSafeValue<int>(_m_COL_FK_SUPPLIER)
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

        string query = string.Empty;
        
        //startTransaction();
        try {

            query = $"INSERT INTO {_m_TBL_NAME} " +
                $"({_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER}) " +
                $"VALUES (@fk_receipt, @fk_supplier); SELECT LAST_INSERT_ID();";

            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            cmd.Parameters.AddWithValue("@fk_supplier", item.fk_supplier_id);

            int item_id = Convert.ToInt32(cmd.ExecuteScalar());
            //commitTransaction(); // TODO: Move
            return item_id;
        }
        catch (MySqlException ex) {
            //rollbackTransaction();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }


    public ObservableCollection<ReceiptSupplierItem> getListItemWithSupplierID(string supplier_id) {

        ObservableCollection<ReceiptSupplierItem> items = new();

        if (string.IsNullOrEmpty(supplier_id)) {
            return items;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_FK_RECEIPT}, {_m_COL_FK_SUPPLIER} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_SUPPLIER} = @supplier_id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@supplier_id", supplier_id);

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptSupplierItem {
                    fk_receipt_id = reader.getSafeValue<int>(_m_COL_FK_RECEIPT),
                    fk_supplier_id = reader.getSafeValue<int>(_m_COL_FK_SUPPLIER)
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
