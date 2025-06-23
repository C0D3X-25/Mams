using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.productsLots;
using Mams.src.productsShapes;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsCategories;

/// <summary>
/// Product Category is about what is is, like "Honey", "Wax", "Soap"

public class ProductCategoryModel :
    ABaseModel,
    ICrudOperation<ProductCategoryItem> {

    private const string _m_TBL_NAME = "products_categories";
    private const string _m_COL_ID = "product_category_id";
    private const string _m_COL_NAME = "product_category_name";
    private const string _m_COL_ARCHIVE = "product_category_archive";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }


    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ProductCategoryItem? getItem(string search) {
        throw new NotImplementedException();
    }


    public ProductCategoryItem? getItemByID(string id) {

        

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
                return new ProductCategoryItem {
                    product_category_id = reader.GetSafeValue<int>(_m_COL_ID),
                    product_category_name = reader.GetSafeValue(_m_COL_NAME, string.Empty),
                    product_category_archive = reader.GetSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ProductCategoryItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<ProductCategoryItem>(_m_TBL_NAME);
    }


    public int saveItem(ProductCategoryItem item) {
        if (item == null) {
            return 0;
        }
        
        if (conn == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.product_category_id;

        if (item_id == 0) {

            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.product_category_name)) {
                return 0;
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}) " +
                $"VALUES (@name); SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        transaction = conn?.BeginTransaction();

        try {
            using MySqlCommand cmd = new(query, conn, transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@name", item.product_category_name);

            if (item_id == 0) {
                item_id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else {
                cmd.ExecuteNonQuery();
            }

            transaction?.Commit();
            return item_id;
        }
        catch (MySqlException ex) {
            transaction?.Rollback();
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }
}
