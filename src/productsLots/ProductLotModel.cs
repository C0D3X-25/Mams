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

    public const string m_TBL_NAME = "products_lots";
    public const string m_COL_ID = "product_lot_id";
    public const string m_COL_NAME = "product_lot_name";
    public const string m_COL_YEAR = "product_lot_year";
    public const string m_COL_FK_BEEHIVE = "fk_beehive_id";
    public const string m_COL_ARCHIVE = "product_lot_archive";


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
            item.beehive_name = item.fk_beehive_id > 0 ? beehive_model.getItemByID(item.fk_beehive_id.ToString())?.beehive_name ?? string.Empty : "";
        }

        return table;
    }


    public bool saveItem(ProductLotItem item) {

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;

        if (item.product_lot_id == 0) {

            if (checkIfItemExist(m_TBL_NAME, m_COL_NAME, item.product_lot_name)) {
                return false;
            }

            query = $"INSERT INTO {m_TBL_NAME} ({m_COL_NAME}, {m_COL_YEAR}, {m_COL_FK_BEEHIVE}) " +
                $"VALUES (@name, @year, @fk_beehive)";
        }
        else {
            query = $"UPDATE {m_TBL_NAME} " +
                $"SET {m_COL_NAME} = @name, {m_COL_YEAR} = @year, {m_COL_FK_BEEHIVE} = @fk_beehive " +
                $"WHERE {m_COL_ID} = @id";
        }

        try {
            using MySqlCommand cmd = new(query, conn);
            if (item.product_lot_id != 0) {
                cmd.Parameters.AddWithValue("@id", item.product_lot_id);
            }
            cmd.Parameters.AddWithValue("@name", item.product_lot_name);
            cmd.Parameters.AddWithValue("@year", item.product_lot_year);
            cmd.Parameters.AddWithValue("@fk_beehive", item.fk_beehive_id);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}
