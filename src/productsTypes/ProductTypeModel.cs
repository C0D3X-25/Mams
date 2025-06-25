using Mams.src.beehives;
using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using Mams.src.productsShapes;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsTypes;

/// <summary>
/// Type is about the product utility, where it will be used like "Exploitation", "Production"


public class ProductTypeModel :
    ABaseModel,
    ICrudOperation<ProductTypeItem> {

    private const string _m_TBL_NAME = "products_types";
    private const string _m_COL_ID = "product_type_id";
    private const string _m_COL_NAME = "product_type_name";
    private const string _m_COL_ARCHIVE = "product_type_archive";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }


    public ProductTypeItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new ProductTypeItem {
                    product_type_id = reader.getSafeValue<int>(_m_COL_ID),
                    product_type_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                    product_type_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }

    public ObservableCollection<ProductTypeItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<ProductTypeItem>(_m_TBL_NAME);
    }

    public int saveItem(ProductTypeItem item) {
        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.product_type_id;

        if (item_id == 0) {
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.product_type_name)) {
                return 0;
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}) " +
                $"VALUES (@name); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        startTransaction();
        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@name", item.product_type_name);

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
}
