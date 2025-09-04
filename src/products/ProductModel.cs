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

/// <summary>
/// Represents a model for managing product-related data and operations.
/// </summary>
public class ProductModel : ABaseModel,
    ICrudOperation<ProductItem> {

    private const string _m_TBL_NAME = "products";
    private const string _m_COL_ID = "product_id";
    private const string _m_COL_NAME = "product_name";
    private const string _m_COL_WEIGHT = "product_weight";
    private const string _m_COL_FK_PRODUCT_TYPE = "fk_product_type_id";
    private const string _m_COL_FK_PRODUCT_CATEGORY = "fk_product_category_id";
    private const string _m_COL_FK_PRODUCT_SHAPE = "fk_product_shape_id";
    private const string _m_COL_ARCHIVE = "product_archive";

    private const int       _m_DEFAUL_FK_PRODUCT_SHAPE = 1;
    private const string    _m_DEFAULT_ARCHIVE = "1901-01-01";

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
    /// Retrieves a <see cref="ProductItem"/> by its unique identifier.
    /// </summary>
    /// <remarks>This method queries the database to retrieve a product's details based on its unique
    /// identifier.  If the product is found, its associated type, category, and shape names are also resolved. If an
    /// error occurs during the database operation, the method returns <see langword="null"/> and displays an error
    /// message.</remarks>
    /// <param name="id">The unique identifier of the product to retrieve. This value cannot be null or empty.</param>
    /// <returns>A <see cref="ProductItem"/> object containing the product details if found; otherwise, <see langword="null"/>.</returns>
    public ProductItem? getItemByID(string id) {
        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        return executeWithConnection<ProductItem?>(connection => {
            try {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_WEIGHT}, " +
                    $"{_m_COL_FK_PRODUCT_TYPE}, {_m_COL_FK_PRODUCT_CATEGORY}, " +
                    $"{_m_COL_FK_PRODUCT_SHAPE}, " +
                    $"{_m_COL_ARCHIVE} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = @id;",
                    connection
                );

                cmd.Parameters.AddWithValue("@id", id);

                ProductTypeModel product_type_model = new();
                ProductCategoryModel product_category_model = new();
                ProductShapeModel product_shape_model = new();
                ProductItem product = new();

                int product_type_id = 0;
                int product_category_id = 0;
                int product_shape_id = 0;

                // Using a block to ensure the reader is disposed of properly
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {
                    product_type_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT_TYPE, 0);
                    product_category_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT_CATEGORY, 0);
                    product_shape_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT_SHAPE, 0);

                    product.product_id = reader.getSafeValue<int>(_m_COL_ID);
                    product.product_name = reader.getSafeValue(_m_COL_NAME, string.Empty);
                    product.product_weight = reader.getSafeValue(_m_COL_WEIGHT, 0);
                    product.fk_product_type_id = product_type_id;
                    product.fk_product_category_id = product_category_id;
                    product.fk_product_shape_id = product_shape_id;
                    product.product_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString();
                }
                else {
                    return null;
                }

                // Get associated names from other models
                product.product_type_name = product_type_model.getItemByID(product_type_id.ToString())?.product_type_name ?? string.Empty;
                product.product_category_name = product_category_model.getItemByID(product_category_id.ToString())?.product_category_name ?? string.Empty;
                product.product_shape_name = product_shape_model.getItemByID(product_shape_id.ToString())?.product_shape_name ?? string.Empty;
                    
                return product;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return null;
            }
        });
    }

    /// <summary>
    /// Retrieves a collection of product items from the database, with additional details populated for related
    /// entities.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="ProductItem"/> objects, where each item includes
    /// additional details for related entities such as product type, category, and shape.</returns>
    public ObservableCollection<ProductItem> getTable() {
        ObservableCollection<ProductItem> table = SDatabaseModel.getAllRowsInTable<ProductItem>(_m_TBL_NAME, _m_COL_NAME);

        ProductTypeModel product_type_model = new();
        ProductCategoryModel product_category_model = new();
        ProductShapeModel product_shape_model = new();

        foreach (ProductItem item in table) {
            item.product_type_name = item.fk_product_type_id > 0 
                ? product_type_model.getItemByID(item.fk_product_type_id.ToString())?.product_type_name ?? string.Empty 
                : string.Empty;
            item.product_category_name = item.fk_product_category_id > 0 
                ? product_category_model.getItemByID(item.fk_product_category_id.ToString())?.product_category_name ?? string.Empty 
                : string.Empty;
            item.product_shape_name = item.fk_product_shape_id > 0 
                ? product_shape_model.getItemByID(item.fk_product_shape_id.ToString())?.product_shape_name ?? string.Empty 
                : string.Empty;
        }
        return table;
    }

    /// <summary>
    /// Saves the specified <see cref="ProductItem"/> to the database.
    /// </summary>
    /// <param name="item">The <see cref="ProductItem"/> to save. Must not be <c>null</c>.</param>
    /// <returns>The <c>product_id</c> of the saved item. Returns <c>0</c> if the operation fails or the item is invalid.</returns>
    public int saveItem(ProductItem item) {
        if (item == null) {
            return 0;
        }

        int item_id = item.product_id;
        string item_product_name = item.product_name.Trim();
        string query;
        
        // Determine if we're inserting or updating
        bool isInsert = (item_id == 0);
        
        if (isInsert) {
            // Check for duplicate name before inserting
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item_product_name)) {
                return 0;
            }
            
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_WEIGHT}, " +
                $"{_m_COL_FK_PRODUCT_TYPE}, {_m_COL_FK_PRODUCT_CATEGORY}, " +
                $"{_m_COL_FK_PRODUCT_SHAPE}) " +
                $"VALUES (@name, @weight, @fk_product_type, @fk_product_category, @fk_product_shape); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_WEIGHT} = @weight, " +
                $"{_m_COL_FK_PRODUCT_TYPE} = @fk_product_type, " +
                $"{_m_COL_FK_PRODUCT_CATEGORY} = @fk_product_category, " +
                $"{_m_COL_FK_PRODUCT_SHAPE} = @fk_product_shape " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        // Start transaction if needed
        bool need_transaction = !isTransactionActive();
        if (need_transaction) {
            startTransaction();
        }

        try {
            // Ensure shape ID is valid
            if (item.fk_product_shape_id == 0) {
                item.fk_product_shape_id = _m_DEFAUL_FK_PRODUCT_SHAPE;
            }

            if (isInsert) {
                // For INSERT operations, we need to return the new ID
                item_id = executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@name", item_product_name);
                    cmd.Parameters.AddWithValue("@weight", item.product_weight);
                    cmd.Parameters.AddWithValue("@fk_product_type", item.fk_product_type_id);
                    cmd.Parameters.AddWithValue("@fk_product_category", item.fk_product_category_id);
                    cmd.Parameters.AddWithValue("@fk_product_shape", item.fk_product_shape_id);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                });
            }
            else {
                // For UPDATE operations, we just execute the command
                executeWithConnection(connection => {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@id", item_id);
                    cmd.Parameters.AddWithValue("@name", item_product_name);
                    cmd.Parameters.AddWithValue("@weight", item.product_weight);
                    cmd.Parameters.AddWithValue("@fk_product_type", item.fk_product_type_id);
                    cmd.Parameters.AddWithValue("@fk_product_category", item.fk_product_category_id);
                    cmd.Parameters.AddWithValue("@fk_product_shape", item.fk_product_shape_id);
                    cmd.ExecuteNonQuery();
                });
            }

            if (need_transaction) {
                commitTransaction();
            }
            
            return item_id;
        }
        catch (MySqlException ex) {
            if (need_transaction) {
                rollbackTransaction();
            }
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// Retrieves a collection of products filtered by the specified product type IDs.
    /// </summary>
    /// <param name="product_type_id">A list of product type IDs to filter the products. Each ID represents a specific product type.</param>
    /// <returns>An observable collection of <see cref="ProductItem"/> objects that match the specified product type IDs. If no
    /// products match, the collection will be empty.</returns>
    public ObservableCollection<ProductItem> getProductWithProductTypeId(List<int> product_type_id) {
        return getProductWith(_m_COL_FK_PRODUCT_TYPE, product_type_id);
    }

    /// <summary>
    /// Retrieves a collection of products that belong to the specified product category IDs.
    /// </summary>
    /// <param name="product_category_id">A list of product category IDs used to filter the products. Each ID must correspond to a valid product category.</param>
    /// <returns>An observable collection of <see cref="ProductItem"/> objects that match the specified product category IDs. If
    /// no products are found, the collection will be empty.</returns>
    public ObservableCollection<ProductItem> getProductWithProductCategoryId(List<int> product_category_id) {
        return getProductWith(_m_COL_FK_PRODUCT_CATEGORY, product_category_id);
    }

    /// <summary>
    /// Retrieves a collection of products that match the specified product shape IDs.
    /// </summary>
    /// <param name="product_shape_id">A list of product shape IDs to filter the products. Each ID in the list must correspond to a valid product
    /// shape.</param>
    /// <returns>An <see cref="ObservableCollection{ProductItem}"/> containing the products that match the specified product
    /// shape IDs. If no products match, the collection will be empty.</returns>
    public ObservableCollection<ProductItem> getProductWithProductShapeId(List<int> product_shape_id) {
        return getProductWith(_m_COL_FK_PRODUCT_SHAPE, product_shape_id);
    }

    /// <summary>
    /// Retrieves a collection of product items from the database based on the specified column name and a list of IDs.
    /// </summary>
    /// <param name="column_name">The name of the column to filter the query by. This must correspond to a valid column in the database.</param>
    /// <param name="ids">A list of integer IDs used to filter the query results. Each ID is matched against the specified column.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="ProductItem"/> objects representing the products that
    /// match the specified criteria. If no matching products are found, the collection will be empty.</returns>
    private ObservableCollection<ProductItem> getProductWith(string column_name, List<int> ids) {
        ObservableCollection<ProductItem> product_items = new();

        if (string.IsNullOrEmpty(column_name) || ids.Count == 0) {
            return product_items;
        }
        
        // Check ID validity
        foreach (int id in ids) {
            if (!SDataValidation.isIdValid(id)) {
                return product_items;
            }
        }
        
        return executeWithConnection(connection => {
            try {
                string query = $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_WEIGHT}, " +
                    $"{_m_COL_FK_PRODUCT_TYPE}, {_m_COL_FK_PRODUCT_CATEGORY}, " +
                    $"{_m_COL_FK_PRODUCT_SHAPE} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {column_name} IN ({string.Join(",", ids.Select((id, index) => $"@id{index}"))});";
                
                using MySqlCommand cmd = new(query, connection);

                // Add parameters for each ID
                for (int i = 0; i < ids.Count; i++) {
                    cmd.Parameters.AddWithValue($"@id{i}", ids[i]);
                }

                using MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    product_items.Add(new ProductItem {
                        product_id = reader.getSafeValue<int>(_m_COL_ID),
                        product_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                        product_weight = reader.getSafeValue(_m_COL_WEIGHT, 0),
                        fk_product_type_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT_TYPE, 0),
                        fk_product_category_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT_CATEGORY, 0),
                        fk_product_shape_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT_SHAPE, 0),
                    });
                }
                
                return product_items;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return product_items;
            }
        });
    }
}

