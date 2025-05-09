using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

/// <summary>
/// ReceiptProduct is for 1 product in a receipt.
public class ReceiptProductModel : ABaseModel,
    ICrudOperation<ReceiptProductItem> {

    public const string m_TBL_NAME = "receipts_products";
    public const string m_COL_ID = "receipt_product_id";
    public const string m_COL_QUANTITY = "receipt_product_quantity";
    public const string m_COL_UNITY_PRICE = "receipt_product_unity_price";
    public const string m_COL_FK_PRODUCT = "fk_product_id";
    public const string m_COL_FK_RECEIPT = "fk_receipt_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_FK_RECEIPT, "", m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ReceiptProductItem? getItemByID(string id) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID}, " +
                $"{m_COL_QUANTITY}, " +
                $"{m_COL_UNITY_PRICE} " +
                $"{m_COL_FK_PRODUCT} " +
                $"{m_COL_FK_RECEIPT} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ReceiptProductItem {
                    receipt_product_id = reader.GetSafeValue<int>(m_COL_ID),
                    receipt_product_quantity = reader.GetSafeValue<int>(m_COL_QUANTITY),
                    receipt_product_unity_price = reader.GetSafeValue<double>(m_COL_UNITY_PRICE),
                    fk_product_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT),
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT)
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ReceiptProductItem> getTable() {

        ObservableCollection<ReceiptProductItem> items = new();

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_PRODUCT} " +
                $"FROM {m_TBL_NAME};",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptProductItem {
                    receipt_product_id = reader.GetSafeValue<int>(m_COL_ID),
                    receipt_product_quantity = reader.GetSafeValue<int>(m_COL_QUANTITY),
                    receipt_product_unity_price = reader.GetSafeValue<double>(m_COL_UNITY_PRICE),
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    fk_product_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT)
                });
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }


    public bool saveItem(ReceiptProductItem item) {
        if (item == null) {
            return false;
        }
        if (item.fk_receipt_id == 0 || item.fk_product_id == 0) {
            return false;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            // Check if the item already exists in the database
            {
                using MySqlCommand cmd = new(
                    $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_PRODUCT} " +
                    $"FROM {m_TBL_NAME} " +
                    $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt " +
                    $"AND {m_COL_FK_PRODUCT} = @fk_product " +
                    $"AND {m_COL_QUANTITY} = @quantity " +
                    $"AND {m_COL_UNITY_PRICE} = @unity_price;",
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
                    $"({m_COL_QUANTITY}, {m_COL_UNITY_PRICE} " +
                    $"{m_COL_FK_RECEIPT}, {m_COL_FK_PRODUCT}) " +
                    $"VALUES (@quantity, @unity_price, @fk_receipt, @fk_product);",
                    conn
                );
                cmd.Parameters.AddWithValue("@quantity", item.receipt_product_quantity);
                cmd.Parameters.AddWithValue("@unity_price", item.receipt_product_unity_price);
                cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
                cmd.Parameters.AddWithValue("@fk_product", item.fk_product_id);
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }


    public ObservableCollection<ReceiptProductItem> getListItemWithReceiptID(string fk_receipt) {

        ObservableCollection<ReceiptProductItem> items = new();

        if (string.IsNullOrEmpty(fk_receipt)) {
            return items;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_FK_RECEIPT}, {m_COL_FK_PRODUCT} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;",
                conn
            );

            cmd.Parameters.AddWithValue("@fk_receipt", fk_receipt);

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptProductItem {
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    fk_product_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT)
                });
            }

            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }


    //public bool deleteListItemWithReceiptID(string fk_receipt) {

    //    ObservableCollection<ReceiptProductItem> items = getListItemWithReceiptID(fk_receipt);

    //    if (items.Count == 0) {
    //        return false;
    //    }
    //    foreach (ReceiptProductItem item in items) {
    //        deleteItemWithReceiptFK(item.fk_receipt_id);
    //    }
    //    return true;
    //}
}
