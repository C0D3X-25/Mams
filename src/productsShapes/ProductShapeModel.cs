using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsShapes;

/// <summary>
/// Represents a model for managing product shape data, including CRUD operations.
/// </summary>
public class ProductShapeModel :
    ABaseModel,
    ICrudOperation<ProductShapeItem> {

    private const string _m_TBL_NAME = "products_shapes";
    private const string _m_COL_ID = "product_shape_id";
    private const string _m_COL_NAME = "product_shape_name";
    private const string _m_COL_ARCHIVE = "product_shape_archive";

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns><see langword="true"/> if the item was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="ProductShapeItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product shape item to retrieve. Cannot be null or empty.</param>
    /// <returns>A <see cref="ProductShapeItem"/> object if an item with the specified identifier exists;  otherwise, <see
    /// langword="null"/>.</returns>
    public ProductShapeItem? getItemByID(string id) {

        if (string.IsNullOrEmpty(id)) {
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
                return new ProductShapeItem {
                    product_shape_id = reader.getSafeValue<int>(_m_COL_ID),
                    product_shape_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                    product_shape_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Retrieves all rows from the table as an observable collection of <see cref="ProductShapeItem"/>.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing all rows in the table as <see cref="ProductShapeItem"/>
    /// objects. The collection will be empty if no rows are found.</returns>
    public ObservableCollection<ProductShapeItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<ProductShapeItem>(_m_TBL_NAME, _m_COL_ARCHIVE);
    }

    /// <summary>
    /// Saves the specified <see cref="ProductShapeItem"/> to the database.
    /// </summary>
    /// <param name="item">The <see cref="ProductShapeItem"/> to save. Must not be <c>null</c>.</param>
    /// <returns>The <c>product_shape_id</c> of the saved item. Returns <c>0</c> if the operation fails or if the item is
    /// <c>null</c>.</returns>
    public int saveItem(ProductShapeItem item) {

        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.product_shape_id;
        string item_name = item.product_shape_name.Trim();

        if (item_id == 0) {
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item_name)) {
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
            cmd.Parameters.AddWithValue("@name", item_name);

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

