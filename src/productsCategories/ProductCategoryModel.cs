using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsCategories;

/// <summary>
/// Represents a model for managing product categories in the database.
/// </summary>
public class ProductCategoryModel :
    ABaseModel,
    ICrudOperation<ProductCategoryItem> {

    private const string _m_TBL_NAME = "products_categories";
    private const string _m_COL_ID = "product_category_id";
    private const string _m_COL_NAME = "product_category_name";
    private const string _m_COL_ARCHIVE = "product_category_archive";

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
    /// Retrieves a <see cref="ProductCategoryItem"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product category item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ProductCategoryItem"/> representing the product category with the specified identifier,  or <see
    /// langword="null"/> if the identifier is invalid or no matching item is found.</returns>
    public ProductCategoryItem? getItemByID(string id) {

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
                return new ProductCategoryItem {
                    product_category_id = reader.getSafeValue<int>(_m_COL_ID),
                    product_category_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                    product_category_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
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
    /// Retrieves all rows from the product category table as an observable collection.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing all rows in the product category table. If the table is
    /// empty, the collection will be empty.</returns>
    public ObservableCollection<ProductCategoryItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<ProductCategoryItem>(_m_TBL_NAME);
    }

    /// <summary>
    /// Saves a product category item to the database. If the item does not exist, it inserts a new record and returns
    /// the generated ID. If the item already exists, it updates the existing record.
    /// </summary>
    /// <param name="item">The <see cref="ProductCategoryItem"/> to be saved.  The <see cref="ProductCategoryItem.product_category_id"/>
    /// property determines whether the item is inserted (if 0) or updated (if non-zero).</param>
    /// <returns>The ID of the saved product category item.  Returns 0 if the operation fails or if the item is null.</returns>
    public int saveItem(ProductCategoryItem item) {

        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.product_category_id;
        string item_name = item.product_category_name.Trim();

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
