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

    private const string _m_TBL_NAME = "products_lots";
    private const string _m_COL_ID = "product_lot_id";
    private const string _m_COL_NAME = "product_lot_name";
    private const string _m_COL_YEAR = "product_lot_year";
    private const string _m_COL_FK_BEEHIVE = "fk_beehive_id";
    private const string _m_COL_ARCHIVE = "product_lot_archive";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ProductLotItem? getItem(string search) {
        throw new NotImplementedException();
    }


    public ProductLotItem? getItemByID(string id) {

        if (string.IsNullOrWhiteSpace(id)) {
            return null;
        }

        using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_YEAR}, {_m_COL_FK_BEEHIVE}, {_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
        );

        ProductLotItem product_lot = new();
        BeehiveModel beehive_model = new();
        int beehive_id = 0;

        cmd.Parameters.AddWithValue("@id", id);

        try {
            {
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {

                    beehive_id = reader.GetSafeValue<int>(_m_COL_FK_BEEHIVE, 0);

                    product_lot.product_lot_id = reader.GetSafeValue<int>(_m_COL_ID);
                    product_lot.product_lot_name = reader.GetSafeValue(_m_COL_NAME, string.Empty);
                    product_lot.product_lot_year = reader.GetSafeValue<int>(_m_COL_YEAR);
                    product_lot.fk_beehive_id = beehive_id;
                    product_lot.product_lot_archive = reader.GetSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString();
                }
            }

            product_lot.beehive_name = beehive_model.getItemByID(beehive_id.ToString())?.beehive_name ?? string.Empty;

            return product_lot;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ProductLotItem> getTable() {
        ObservableCollection<ProductLotItem> table = SDatabaseModel.getAllRowsInTable<ProductLotItem>(_m_TBL_NAME);

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

        string query = string.Empty;
        int item_id = item.product_lot_id;

        if (item_id == 0) {
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.product_lot_name)) {
                return 0;
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_YEAR}, {_m_COL_FK_BEEHIVE}) " +
                $"VALUES (@name, @year, @fk_beehive); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_YEAR} = @year, {_m_COL_FK_BEEHIVE} = @fk_beehive " +
                $"WHERE {_m_COL_ID} = @id";
        }

        startTransaction();

        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);

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

            commitTransaction();
            return item_id;
        }
        catch (MySqlException ex) {
            rollbackTransaction();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }


    public ObservableCollection<ProductLotItem> getProductLotsWithBeehiveId(List<int> beehive_ids) {

        ObservableCollection<ProductLotItem> table = new();

        if (beehive_ids.Count == 0) {
            return table;
        }

        string query = $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_YEAR}, {_m_COL_FK_BEEHIVE} " +
            $"FROM {_m_TBL_NAME} " +
            $"WHERE {_m_COL_FK_BEEHIVE} IN ({string.Join(",", beehive_ids)})";

        try {
            using MySqlCommand cmd = new(query, m_conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                table.Add(new ProductLotItem {
                    product_lot_id = reader.GetSafeValue<int>(_m_COL_ID),
                    product_lot_name = reader.GetSafeValue(_m_COL_NAME, string.Empty),
                    product_lot_year = reader.GetSafeValue<int>(_m_COL_YEAR),
                    fk_beehive_id = reader.GetSafeValue<int>(_m_COL_FK_BEEHIVE)
                });
            }
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
        }
        return table;
    }
}
