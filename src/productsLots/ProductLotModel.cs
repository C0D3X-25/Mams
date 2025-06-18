using Mams.src.beehives;
using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.products;
using Mams.src.productsShapes;
using Mams.src.productsTypes;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsLots;

public class ProductLotModel :
    ABaseModel,
    ICrudOperation<ProductLotItem> {

    private const string m_TBL_NAME = "products_lots";
    private const string m_COL_ID = "product_lot_id";
    private const string m_COL_NAME = "product_lot_name";
    private const string m_COL_YEAR = "product_lot_year";
    private const string m_COL_FK_BEEHIVE = "fk_beehive_id";
    private const string m_COL_ARCHIVE = "product_lot_archive";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_ID, m_COL_ARCHIVE, m_TBL_NAME, delete_type);
    }


    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ProductLotItem? getItem(string search) {
        throw new NotImplementedException();
    }


    public ProductLotItem? getItemByID(string id) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID}, {m_COL_NAME}, {m_COL_YEAR}, {m_COL_FK_BEEHIVE}, {m_COL_ARCHIVE} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {

                int beehive_id = reader.GetSafeValue<int>(m_COL_FK_BEEHIVE, 0);

                BeehiveModel beehive_model = new();

                return new ProductLotItem {
                    product_lot_id = reader.GetSafeValue<int>(m_COL_ID),
                    product_lot_name = reader.GetSafeValue(m_COL_NAME, string.Empty),
                    product_lot_year = reader.GetSafeValue<int>(m_COL_YEAR),
                    fk_beehive_id = beehive_id,
                    beehive_name = beehive_model.getItemByID(beehive_id.ToString())?.beehive_name ?? string.Empty,
                    product_lot_archive = reader.GetSafeValue(m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ProductLotItem> getTable() {
        ObservableCollection<ProductLotItem> table = SDatabaseModel.getAllData<ProductLotItem>(this, m_TBL_NAME);

        BeehiveModel beehive_model = new();

        foreach (ProductLotItem item in table) {
            item.beehive_name = item.fk_beehive_id > 0 ? beehive_model.getItemByID(item.fk_beehive_id.ToString())?.beehive_name ?? string.Empty : string.Empty;
        }

        return table;
    }


    public int saveItem(ProductLotItem item) {
        if (item == null) {
            return 0;
        }
        using MySqlConnection? conn = _m_conn.openConnection();
        if (conn == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.product_lot_id;

        if (item_id == 0) {

            if (isItemPresentInDatabase(m_TBL_NAME, m_COL_NAME, item.product_lot_name)) {
                return 0;
            }

            query = $"INSERT INTO {m_TBL_NAME} ({m_COL_NAME}, {m_COL_YEAR}, {m_COL_FK_BEEHIVE}) " +
                $"VALUES (@name, @year, @fk_beehive); SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {m_TBL_NAME} " +
                $"SET {m_COL_NAME} = @name, {m_COL_YEAR} = @year, {m_COL_FK_BEEHIVE} = @fk_beehive " +
                $"WHERE {m_COL_ID} = @id";
        }

        using var transaction = conn.BeginTransaction();

        try {
            using MySqlCommand cmd = new(query, conn, transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@name", item.product_lot_name);
            cmd.Parameters.AddWithValue("@year", item.product_lot_year);
            cmd.Parameters.AddWithValue("@fk_beehive", item.fk_beehive_id);

            if (item_id == 0) {
                item_id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else {
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
            return item_id;
        }
        catch (MySqlException ex) {
            transaction.Rollback();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }


    public ObservableCollection<ProductLotItem> getProductLotWithBeehiveId(List<int> beehive_ids) {

        ObservableCollection<ProductLotItem> table = new();

        if (beehive_ids.Count == 0) {
            return table;
        }

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = $"SELECT {m_COL_ID}, {m_COL_NAME}, {m_COL_YEAR}, {m_COL_FK_BEEHIVE} " +
            $"FROM {m_TBL_NAME} " +
            $"WHERE {m_COL_FK_BEEHIVE} IN ({string.Join(",", beehive_ids)})";

        try {
            using MySqlCommand cmd = new(query, conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                table.Add(new ProductLotItem {
                    product_lot_id = reader.GetSafeValue<int>(m_COL_ID),
                    product_lot_name = reader.GetSafeValue(m_COL_NAME, string.Empty),
                    product_lot_year = reader.GetSafeValue<int>(m_COL_YEAR),
                    fk_beehive_id = reader.GetSafeValue<int>(m_COL_FK_BEEHIVE)
                });
            }
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
        }
        return table;
    }
}
