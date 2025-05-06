using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts; 

public class ReceiptModel : ABaseModel,
    ICrudOperation<ReceiptItem> {

    public const string m_TBL_NAME = "receipts";
    public const string m_COL_ID = "receipt_id";
    public const string m_COL_RECEIPT_TOTAL_PRICE = "receipt_total_price";
    public const string m_COL_RECEIPT_DATE_CREATED = "receipt_date_created";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {

        ReceiptProductModel receipt_product_model = new();
        ReceiptClientModel receipt_client_model = new();
        ReceiptSupplierModel receipt_supplier_model = new();

        receipt_product_model.deleteItemWithReceiptFK(int.Parse(id));
        receipt_client_model.deleteItemWithReceiptFK(int.Parse(id));
        receipt_supplier_model.deleteItemWithReceiptFK(int.Parse(id));

        return SDatabaseModel.deleteItem(this, id, m_COL_ID, "", m_TBL_NAME, delete_type);
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
                    receipt_total_price = reader.GetSafeValue<double>(m_COL_RECEIPT_TOTAL_PRICE),
                    receipt_date_created = reader.GetSafeValue(m_COL_RECEIPT_DATE_CREATED, DateOnly.MinValue).ToString()
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

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;

        if (item.receipt_id == 0) {

            //if (checkIfItemExist(m_TBL_NAME, m_COL_NAME, item.product_name)) {
            //    // TODO
            //    return false;
            //}

            query = $"INSERT INTO {m_TBL_NAME} (" +
                $"{m_COL_RECEIPT_TOTAL_PRICE}, " +
                $"{m_COL_RECEIPT_DATE_CREATED}) " +
                $"VALUES (@total_price, @date_created);";
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
            cmd.Parameters.AddWithValue("@date_created", item.receipt_date_created);
            cmd.ExecuteNonQuery();

            ReceiptProductModel receipt_product_model = new();
            ReceiptClientModel receipt_client_model = new();
            ReceiptSupplierModel receipt_supplier_model = new();

            ObservableCollection<ReceiptProductItem> receipt_product_items = new();

            ReceiptClientItem receipt_client_item = new() {
                fk_receipt_id = item.receipt_id,
                fk_client_id = item.fk_client_id
            };
            ReceiptSupplierItem receipt_supplier_item = new() {
                fk_receipt_id = item.receipt_id,
                fk_supplier_id = item.fk_supplier_id
            };

            if (receipt_client_item.fk_client_id != 0) {
                receipt_client_model.saveItem(receipt_client_item);
            }
            else {
                if (receipt_supplier_item.fk_supplier_id != 0) {
                    receipt_supplier_model.saveItem(receipt_supplier_item);
                }
            }

            // TODO: need to get all the products then add them a receipt id, maybe a other save method where all save are done in same time ?
            foreach (ReceiptProductItem receipt_product_item in item.receipt_product_items) {
                receipt_product_item.fk_receipt_id = item.receipt_id;
                receipt_product_model.saveItem(receipt_product_item);
            }

            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}
