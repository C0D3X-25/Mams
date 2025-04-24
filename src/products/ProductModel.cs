using Mams.src.crudOperations;
using Mams.src.helpers;
using Mams.src.productsCategories;
using Mams.src.productsLots;
using Mams.src.productsShapes;
using Mams.src.productsTypes;
using Mams.src.searchBars;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.products;
internal class ProductModel :
    ABaseSearchModel,
    ICrudOperation<ProductItem> {

    private const string _m_TBL_NAME = "products";
    private const string _m_COL_ID = "product_id";
    private const string _m_COL_NAME = "product_name";
    private const string _m_COL_WEIGHT = "product_weight";
    private const string _m_COL_FK_PRODUCT_TYPE_ID = "fk_product_type_id";
    private const string _m_COL_FK_PRODUCT_TYPE_NAME = "product_type_name";
    private const string _m_COL_FK_PRODUCT_CATEGORY_ID = "fk_product_category_id";
    private const string _m_COL_FK_PRODUCT_CATEGORY_NAME = "product_category_name";
    private const string _m_COL_FK_PRODUCT_SHAPE_ID = "fk_product_shape_id";
    private const string _m_COL_FK_PRODUCT_SHAPE_NAME = "product_shape_name";
    private const string _m_COL_FK_PRODUCT_LOT_ID = "fk_product_lot_id";
    private const string _m_COL_FK_PRODUCT_LOT_NAME = "product_lot_nbr";
    private const string _m_COL_ARCHIVE = "product_archive";


    public bool deleteItem(string id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return DeleteItemModel.deleteItem(this, id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    public bool deleteItem(int id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    public ProductItem? getItem(string search) {
        throw new NotImplementedException();
    }

    public ProductItem? getItemByID(string id) {
        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_WEIGHT}, " +
                $"{_m_COL_FK_PRODUCT_TYPE_ID}, {_m_COL_FK_PRODUCT_TYPE_NAME}, " +
                $"{_m_COL_FK_PRODUCT_CATEGORY_ID}, {_m_COL_FK_PRODUCT_CATEGORY_NAME}, " +
                $"{_m_COL_FK_PRODUCT_SHAPE_ID}, {_m_COL_FK_PRODUCT_SHAPE_NAME}, " +
                $"{_m_COL_FK_PRODUCT_LOT_ID}, {_m_COL_FK_PRODUCT_LOT_NAME}, " +
                $"{_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id ",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {

                int product_type_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_TYPE_ID);
                int product_category_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_CATEGORY_ID);
                int product_shape_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_SHAPE_ID);
                int product_lot_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_LOT_ID);

                ProductCategoryModel product_category_model = new();
                ProductTypeModel product_type_model = new();
                ProductShapeModel product_shape_model = new();
                ProductLotModel product_lot_model = new();

                return new ProductItem {
                    product_id = reader.GetSafeValue<int>(_m_COL_ID),
                    product_name = reader.GetSafeValue(_m_COL_NAME, string.Empty),
                    product_weight = reader.GetSafeValue(_m_COL_WEIGHT, 0),
                    fk_product_type_id = product_type_id,
                    product_type_name = product_type_model.getItemByID(product_type_id.ToString())?.product_type_name ?? string.Empty,
                    fk_product_category_id = product_category_id,
                    product_category_name = product_category_model.getItemByID(product_category_id.ToString())?.product_category_name ?? string.Empty,
                    fk_product_shape_id = product_shape_id,
                    product_shape_name = product_shape_model.getItemByID(product_shape_id.ToString())?.product_shape_name ?? string.Empty,
                    fk_product_lot_id = product_lot_id,
                    product_lot_name = product_lot_model.getItemByID(product_lot_id.ToString())?.product_lot_name ?? string.Empty,
                    product_archive = reader.GetSafeValue(_m_COL_ARCHIVE, DateTime.MinValue).ToString()
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
        return GetTableModel.getTableData<ProductItem>(this, _m_TBL_NAME);
    }

    public bool saveItem(ProductItem item) {
        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;

        if (item.product_id == 0) {

            if (checkIfItemExist(_m_TBL_NAME, _m_COL_NAME, item.product_name)) {
                return false;
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_WEIGHT}, {_m_COL_FK_PRODUCT_TYPE_ID}, {_m_COL_FK_PRODUCT_CATEGORY_ID}, {_m_COL_FK_PRODUCT_SHAPE_ID}, {_m_COL_FK_PRODUCT_LOT_ID}) " +
                $"VALUES (@name, @weight, @fk_product_type, @fk_product_category, @fk_product_shape, @fk_product_lot)";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_WEIGHT} = @weight, {_m_COL_FK_PRODUCT_TYPE_ID} = @fk_product_type, {_m_COL_FK_PRODUCT_CATEGORY_ID} = @fk_product_category, {_m_COL_FK_PRODUCT_SHAPE_ID} = @fk_product_shape, {_m_COL_FK_PRODUCT_LOT_ID} = @fk_product_lot " +
                $"WHERE {_m_COL_ID} = @id";
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


    public override List<string> searchItems(string search) {
        throw new NotImplementedException();
    }
}

