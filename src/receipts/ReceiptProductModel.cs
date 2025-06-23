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

    private const string _m_TBL_NAME = "receipts_products";
    private const string _m_COL_ID = "receipt_product_id";
    private const string _m_COL_QUANTITY = "receipt_product_quantity";
    private const string _m_COL_UNITY_PRICE = "receipt_product_unity_price";
    private const string _m_COL_FK_PRODUCT = "fk_product_id";
    private const string _m_COL_FK_RECEIPT = "fk_receipt_id";
    private const string _m_COL_FK_PRODUCT_LOT = "fk_product_lot_id";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_FK_RECEIPT, string.Empty, _m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ReceiptProductItem? getItemByID(string id) {

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, " +
                $"{_m_COL_QUANTITY}, " +
                $"{_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_PRODUCT}, " +
                $"{_m_COL_FK_RECEIPT}, " +
                $"{_m_COL_FK_PRODUCT_LOT} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ReceiptProductItem {
                    receipt_product_id = reader.GetSafeValue<int>(_m_COL_ID),
                    receipt_product_quantity = reader.GetSafeValue<int>(_m_COL_QUANTITY),
                    receipt_product_unity_price = reader.GetSafeValue<decimal>(_m_COL_UNITY_PRICE),
                    fk_receipt_id = reader.GetSafeValue<int>(_m_COL_FK_RECEIPT),
                    product_item = new ProductItem {
                        product_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT)
                    },
                    product_lot_item = new ProductLotItem {
                        product_lot_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_LOT)
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

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, " +
                $"{_m_COL_QUANTITY}, " +
                $"{_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_PRODUCT}, " +
                $"{_m_COL_FK_RECEIPT}, " +
                $"{_m_COL_FK_PRODUCT_LOT} " +
                $"FROM {_m_TBL_NAME};",
                m_conn
            );
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptProductItem {
                    receipt_product_id = reader.GetSafeValue<int>(_m_COL_ID),
                    receipt_product_quantity = reader.GetSafeValue<int>(_m_COL_QUANTITY),
                    receipt_product_unity_price = reader.GetSafeValue<decimal>(_m_COL_UNITY_PRICE),
                    fk_receipt_id = reader.GetSafeValue<int>(_m_COL_FK_RECEIPT),
                    product_item = new ProductItem {
                        product_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT)
                    },
                    product_lot_item = new ProductLotItem {
                        product_lot_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_LOT)
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


    public int saveItem(ReceiptProductItem item) {

        if (!ValidateReceiptProduct(item)) { 
            return 0;
        }
        
        if (m_conn == null) {
            return 0;
        }

        // TODO: Will probably need to be refactored
        // Check if the item already exists in the database, if not insert the item into the database
        string query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_QUANTITY}, {_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_RECEIPT}, {_m_COL_FK_PRODUCT}, {_m_COL_FK_PRODUCT_LOT}) " +
                $"SELECT @quantity, @unity_price, @fk_receipt, @fk_product, @fk_product_lot " +
                $"WHERE NOT EXISTS (SELECT 1 FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_RECEIPT} = @fk_receipt " +
                $"AND {_m_COL_FK_PRODUCT} = @fk_product " +
                $"AND {_m_COL_QUANTITY} = @quantity " +
                $"AND {_m_COL_UNITY_PRICE} = @unity_price); " +
                $"SELECT LAST_INSERT_ID();";

        //startTransaction();
        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);
            cmd.Parameters.AddWithValue("@quantity", item.receipt_product_quantity);
            cmd.Parameters.AddWithValue("@unity_price", item.receipt_product_unity_price);
            cmd.Parameters.AddWithValue("@fk_receipt", item.fk_receipt_id);
            cmd.Parameters.AddWithValue("@fk_product", item.product_item.product_id);
            cmd.Parameters.AddWithValue("@fk_product_lot", item.product_lot_item.product_lot_id);

            int item_id = Convert.ToInt32(cmd.ExecuteScalar());
            //commitTransaction();
            return item_id;
        }
        catch (MySqlException ex) {
            //rollbackTransaction(); // TODO: Move
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }


    public ObservableCollection<ReceiptProductItem> getListItemWithReceiptID(string fk_receipt) {

        ObservableCollection<ReceiptProductItem> items = new();

        if (string.IsNullOrEmpty(fk_receipt)) {
            return items;
        }

        

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, " +
                $"{_m_COL_QUANTITY}, " +
                $"{_m_COL_UNITY_PRICE}, " +
                $"{_m_COL_FK_PRODUCT}, " +
                $"{_m_COL_FK_RECEIPT}, " +
                $"{_m_COL_FK_PRODUCT_LOT} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_RECEIPT} = @fk_receipt;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@fk_receipt", fk_receipt);

            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                items.Add(new ReceiptProductItem {
                    receipt_product_id = reader.GetSafeValue<int>(_m_COL_ID),
                    receipt_product_quantity = reader.GetSafeValue<int>(_m_COL_QUANTITY),
                    receipt_product_unity_price = reader.GetSafeValue<decimal>(_m_COL_UNITY_PRICE),
                    fk_receipt_id = reader.GetSafeValue<int>(_m_COL_FK_RECEIPT),
                    product_item = new ProductItem {
                        product_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT)
                    },
                    product_lot_item = new ProductLotItem {
                        product_lot_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_LOT)
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
