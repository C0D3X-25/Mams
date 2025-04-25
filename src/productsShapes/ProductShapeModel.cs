using Mams.src.crudOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.productsCategories;
using Mams.src.productsTypes;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsShapes;

public class ProductShapeModel :
    ABaseModel,
    ICrudOperation<ProductShapeItem> {

    private const string _m_TBL_NAME = "products_shapes";
    private const string _m_COL_ID = "product_shape_id";
    private const string _m_COL_NAME = "product_shape_name";
    private const string _m_COL_ARCHIVE = "product_shape_archive";


    public bool deleteItem(string id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return DeleteItemModel.deleteItem(this, id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }


    public bool deleteItem(int id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ProductShapeItem? getItem(string search) {
        throw new NotImplementedException();
    }


    public ProductShapeItem? getItemByID(string id) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ProductShapeItem {
                    product_shape_id = reader.GetSafeValue<int>(_m_COL_ID),
                    product_shape_name = reader.GetSafeValue(_m_COL_NAME, string.Empty),
                    product_shape_archive = reader.GetSafeValue(_m_COL_ARCHIVE, DateTime.MinValue).ToString()
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ProductShapeItem> getTable() {
        return GetTableModel.getTableData<ProductShapeItem>(this, _m_TBL_NAME);

    }


    public bool saveItem(ProductShapeItem item) {

        using MySqlConnection? conn = _m_conn.openConnection();

        string query = string.Empty;

        if (item.product_shape_id == 0) {

            if (checkIfItemExist(_m_TBL_NAME, _m_COL_NAME, item.product_shape_name)) {
                return false;
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}) " +
                $"VALUES (@name);";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name" +
                $"WHERE {_m_COL_ID} = @id;";
        }

        try {
            using MySqlCommand cmd = new(query, conn);
            if (item.product_shape_id != 0) {
                cmd.Parameters.AddWithValue("@id", item.product_shape_id);
            }
            cmd.Parameters.AddWithValue("@name", item.product_shape_name);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}

