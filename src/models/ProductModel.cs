using Mams.src.enums;
using Mams.src.interfaces;
using Mams.src.items;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mams.src.models;
internal class ProductModel :
    ABaseSearchModel,
    ISearchItemsByName,
    ICRUDItem<ProductItem> {

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
        throw new NotImplementedException();
    }

    public ObservableCollection<ProductItem> getTable() {
        return GetTableModel.getTableData<ProductItem>(this, _m_TBL_NAME);
    }

    public bool saveItem(ProductItem item) {
        using MySqlConnection? conn = _m_conn.openConnection();

        string query = String.Empty;

        if (item.product_id == 0) {

            if (checkIfItemExist(item.product_name)) {
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


    public List<string> searchItemsByName(string search) {
        throw new NotImplementedException();
    }


    private bool checkIfItemExist(string name) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT COUNT(*) FROM {_m_TBL_NAME} WHERE {_m_COL_NAME} = @name",
                conn
            );
            cmd.Parameters.AddWithValue("@name", name);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}

