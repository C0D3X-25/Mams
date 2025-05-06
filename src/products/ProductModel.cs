using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.productsCategories;
using Mams.src.productsLots;
using Mams.src.productsShapes;
using Mams.src.productsTypes;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.products;
internal class ProductModel : ABaseModel,
    ICrudOperation<ProductItem> {

    public const string m_TBL_NAME = "products";
    public const string m_COL_ID = "product_id";
    public const string m_COL_NAME = "product_name";
    public const string m_COL_WEIGHT = "product_weight";
    public const string m_COL_FK_PRODUCT_TYPE = "fk_product_type_id";
    public const string m_COL_FK_PRODUCT_CATEGORY = "fk_product_category_id";
    public const string m_COL_FK_PRODUCT_SHAPE = "fk_product_shape_id";
    public const string m_COL_FK_PRODUCT_LOT = "fk_product_lot_id";
    public const string m_COL_ARCHIVE = "product_archive";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return SDatabaseModel.deleteItem(this, id, m_COL_ID, m_COL_ARCHIVE, m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ProductItem? getItemByID(string id) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {m_COL_ID}, {m_COL_NAME}, {m_COL_WEIGHT}, " +
                $"{m_COL_FK_PRODUCT_TYPE}, {m_COL_FK_PRODUCT_CATEGORY}, " +
                $"{m_COL_FK_PRODUCT_SHAPE}, {m_COL_FK_PRODUCT_LOT}, " +
                $"{m_COL_ARCHIVE} " +
                $"FROM {m_TBL_NAME} " +
                $"WHERE {m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {

                int product_type_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT_TYPE, 0);
                int product_category_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT_CATEGORY, 0);
                int product_shape_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT_SHAPE, 0);
                int product_lot_id = reader.GetSafeValue<int>(m_COL_FK_PRODUCT_LOT, 0);

                ProductTypeModel product_type_model = new();
                ProductCategoryModel product_category_model = new();
                ProductShapeModel product_shape_model = new();
                ProductLotModel product_lot_model = new();

                return new ProductItem {
                    product_id = reader.GetSafeValue<int>(m_COL_ID),
                    product_name = reader.GetSafeValue(m_COL_NAME, string.Empty),
                    product_weight = reader.GetSafeValue(m_COL_WEIGHT, 0),
                    fk_product_type_id = product_type_id,
                    product_type_name = product_type_model.getItemByID(product_type_id.ToString())?.product_type_name ?? string.Empty,
                    fk_product_category_id = product_category_id,
                    product_category_name = product_category_model.getItemByID(product_category_id.ToString())?.product_category_name ?? string.Empty,
                    fk_product_shape_id = product_shape_id,
                    product_shape_name = product_shape_model.getItemByID(product_shape_id.ToString())?.product_shape_name ?? string.Empty,
                    fk_product_lot_id = product_lot_id,
                    product_lot_name = product_lot_model.getItemByID(product_lot_id.ToString())?.product_lot_name ?? string.Empty,
                    product_archive = reader.GetSafeValue(m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ProductItem> getTable() {

        ObservableCollection<ProductItem> table = SDatabaseModel.getAllData<ProductItem>(this, m_TBL_NAME);

        ProductTypeModel product_type_model = new();
        ProductCategoryModel product_category_model = new();
        ProductShapeModel product_shape_model = new();
        ProductLotModel product_lot_model = new();

        foreach (ProductItem item in table) {
            item.product_type_name = item.fk_product_type_id > 0 ? product_type_model.getItemByID(item.fk_product_type_id.ToString())?.product_type_name ?? string.Empty : "";
            item.product_category_name = item.fk_product_category_id > 0 ? product_category_model.getItemByID(item.fk_product_category_id.ToString())?.product_category_name ?? string.Empty : "";
            item.product_shape_name = item.fk_product_shape_id > 0 ? product_shape_model.getItemByID(item.fk_product_shape_id.ToString())?.product_shape_name ?? string.Empty : "";
            item.product_lot_name = item.fk_product_lot_id > 0 ? product_lot_model.getItemByID(item.fk_product_lot_id.ToString())?.product_lot_name ?? string.Empty : "";
        }
        return table;
    }


    public bool saveItem(ProductItem item) {

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;

        if (item.product_id == 0) {

            if (checkIfItemExist(m_TBL_NAME, m_COL_NAME, item.product_name)) {
                return false;
            }

            query = $"INSERT INTO {m_TBL_NAME} ({m_COL_NAME}, {m_COL_WEIGHT}, " +
                $"{m_COL_FK_PRODUCT_TYPE}, {m_COL_FK_PRODUCT_CATEGORY}, " +
                $"{m_COL_FK_PRODUCT_SHAPE}, {m_COL_FK_PRODUCT_LOT}) " +
                $"VALUES (@name, @weight, @fk_product_type, @fk_product_category, @fk_product_shape, @fk_product_lot);";
        }
        else {
            query = $"UPDATE {m_TBL_NAME} " +
                $"SET {m_COL_NAME} = @name, {m_COL_WEIGHT} = @weight, " +
                $"{m_COL_FK_PRODUCT_TYPE} = @fk_product_type, " +
                $"{m_COL_FK_PRODUCT_CATEGORY} = @fk_product_category, " +
                $"{m_COL_FK_PRODUCT_SHAPE} = @fk_product_shape, " +
                $"{m_COL_FK_PRODUCT_LOT} = @fk_product_lot " +
                $"WHERE {m_COL_ID} = @id;";
        }

        try {
            using MySqlCommand cmd = new(query, conn);
            if (item.product_id != 0) {
                cmd.Parameters.AddWithValue("@id", item.product_id);
            }
            cmd.Parameters.AddWithValue("@name", item.product_name);
            cmd.Parameters.AddWithValue("@weight", item.product_weight);
            cmd.Parameters.AddWithValue("@fk_product_type", item.fk_product_type_id);
            cmd.Parameters.AddWithValue("@fk_product_category", item.fk_product_category_id);
            cmd.Parameters.AddWithValue("@fk_product_shape", item.fk_product_shape_id);
            cmd.Parameters.AddWithValue("@fk_product_lot", item.fk_product_lot_id);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}

