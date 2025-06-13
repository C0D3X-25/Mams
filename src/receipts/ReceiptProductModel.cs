using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.productsLots;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.receipts;

/// <summary>
/// ReceiptProduct is for 1 product in a receipt.
public class ReceiptProductModel : ABaseModel,
    ICrudOperation<ReceiptProductItem> {

    private const string m_TBL_NAME = "receipts_products";
    private const string m_COL_ID = "receipt_product_id";
    private const string m_COL_QUANTITY = "receipt_product_quantity";
    private const string m_COL_UNITY_PRICE = "receipt_product_unity_price";
    private const string m_COL_FK_PRODUCT = "fk_product_id";
    private const string m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string m_COL_FK_PRODUCT_LOT = "fk_product_lot_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_FK_RECEIPT, string.Empty, m_TBL_NAME, delete_type);
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
                $"{m_COL_UNITY_PRICE}, " +
                $"{m_COL_FK_PRODUCT}, " +
                $"{m_COL_FK_RECEIPT}, " +
                $"{m_COL_FK_PRODUCT_LOT} " +
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
                    receipt_product_unity_price = reader.GetSafeValue<decimal>(m_COL_UNITY_PRICE),
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    product_item = new ProductItem {
                        product_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT)
                    },
                    product_lot_item = new ProductLotItem {
                        product_lot_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT_LOT)
                    },
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
                $"SELECT {m_COL_ID}, " +
                $"{m_COL_QUANTITY}, " +
                $"{m_COL_UNITY_PRICE}, " +
                $"{m_COL_FK_PRODUCT}, " +
                $"{m_COL_FK_RECEIPT}, " +
                $"{m_COL_FK_PRODUCT_LOT} " +
                $"FROM {m_TBL_NAME};",
                conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptProductItem {
                    receipt_product_id = reader.GetSafeValue<int>(m_COL_ID),
                    receipt_product_quantity = reader.GetSafeValue<int>(m_COL_QUANTITY),
                    receipt_product_unity_price = reader.GetSafeValue<decimal>(m_COL_UNITY_PRICE),
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    product_item = new ProductItem {
                        product_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT)
                    },
                    product_lot_item = new ProductLotItem {
                        product_lot_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT_LOT)
                    },
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

        if (!ValidateReceiptProduct(item)) {
            return false;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            // Check if the item already exists in the database, if not insert the item into the database
            using MySqlCommand cmd = new(
                $"INSERT INTO {m_TBL_NAME} ({m_COL_QUANTITY}, {m_COL_UNITY_PRICE}, " +
                $"{m_COL_FK_RECEIPT}, {m_COL_FK_PRODUCT}, {m_COL_FK_PRODUCT_LOT}) " +
                $"SELECT @quantity, @unity_price, @fk_receipt, @fk_product, @fk_product_lot " +
                $"WHERE NOT EXISTS (SELECT 1 FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt " +
                $"AND {m_COL_FK_PRODUCT} = @fk_product " +
                $"AND {m_COL_QUANTITY} = @quantity " +
                $"AND {m_COL_UNITY_PRICE} = @unity_price)",
                conn
            );
            cmd.Parameters.AddWithValue("@quantity", item.receipt_product_quantity);
            cmd.Parameters.AddWithValue("@unity_price", item.receipt_product_unity_price);
            cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            cmd.Parameters.AddWithValue("@fk_product", item.product_item.product_id);
            cmd.Parameters.AddWithValue("@fk_product_lot", item.product_lot_item.product_lot_id);
            return cmd.ExecuteNonQuery() > 0;
            
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
                $"SELECT {m_COL_ID}, " +
                $"{m_COL_QUANTITY}, " +
                $"{m_COL_UNITY_PRICE}, " +
                $"{m_COL_FK_PRODUCT}, " +
                $"{m_COL_FK_RECEIPT}, " +
                $"{m_COL_FK_PRODUCT_LOT} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_FK_RECEIPT} = @fk_receipt;",
                conn
            );

            cmd.Parameters.AddWithValue("@fk_receipt", fk_receipt);

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptProductItem {
                    receipt_product_id = reader.GetSafeValue<int>(m_COL_ID),
                    receipt_product_quantity = reader.GetSafeValue<int>(m_COL_QUANTITY),
                    receipt_product_unity_price = reader.GetSafeValue<decimal>(m_COL_UNITY_PRICE),
                    fk_receipt_id = reader.GetSafeValue<int>(m_COL_FK_RECEIPT),
                    product_item = new ProductItem {
                        product_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT)
                    },
                    product_lot_item = new ProductLotItem {
                        product_lot_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT_LOT)
                    },
                });
            }
            return items;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return items;
        }
    }


    private bool ValidateReceiptProduct(ReceiptProductItem item) {
        return item != null
            && item.receipt_product_quantity > 0
            && item.receipt_product_unity_price >= 0
            && item.fk_receipt_id > 0
            && item.product_item.product_id > 0;
    }
}
