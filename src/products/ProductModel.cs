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

    private const string _m_TBL_NAME = "products";
    private const string _m_COL_ID = "product_id";
    private const string _m_COL_NAME = "product_name";
    private const string _m_COL_WEIGHT = "product_weight";
    private const string _m_COL_FK_PRODUCT_TYPE = "fk_product_type_id";
    private const string _m_COL_FK_PRODUCT_CATEGORY = "fk_product_category_id";
    private const string _m_COL_FK_PRODUCT_SHAPE = "fk_product_shape_id";
    private const string _m_COL_ARCHIVE = "product_archive";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
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

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_WEIGHT}, " +
                $"{_m_COL_FK_PRODUCT_TYPE}, {_m_COL_FK_PRODUCT_CATEGORY}, " +
                $"{_m_COL_FK_PRODUCT_SHAPE}, " +
                $"{_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);

            ProductTypeModel product_type_model = new();
            ProductCategoryModel product_category_model = new();
            ProductShapeModel product_shape_model = new();
            ProductItem product = new();

            int product_type_id = 0;
            int product_category_id = 0;
            int product_shape_id = 0;

            { // Using a block to ensure the reader is disposed of properly
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {

                    product_type_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_TYPE, 0);
                    product_category_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_CATEGORY, 0);
                    product_shape_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_SHAPE, 0);

                    product.product_id = reader.GetSafeValue<int>(_m_COL_ID);
                    product.product_name = reader.GetSafeValue(_m_COL_NAME, string.Empty);
                    product.product_weight = reader.GetSafeValue(_m_COL_WEIGHT, 0);
                    product.fk_product_type_id = product_type_id;
                    product.fk_product_category_id = product_category_id;
                    product.fk_product_shape_id = product_shape_id;
                    product.product_archive = reader.GetSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString();
                }
            }

            product.product_type_name = product_type_model.getItemByID(product_type_id.ToString())?.product_type_name ?? string.Empty;
            product.product_category_name = product_category_model.getItemByID(product_category_id.ToString())?.product_category_name ?? string.Empty;
            product.product_shape_name = product_shape_model.getItemByID(product_shape_id.ToString())?.product_shape_name ?? string.Empty;
                
            return product;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<ProductItem> getTable() {

        ObservableCollection<ProductItem> table = SDatabaseModel.getAllRowsInTable<ProductItem>(_m_TBL_NAME);

        ProductTypeModel product_type_model = new();
        ProductCategoryModel product_category_model = new();
        ProductShapeModel product_shape_model = new();
        ProductLotModel product_lot_model = new();

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


    public int saveItem(ProductItem item) {

        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.product_id;

        if (item_id == 0) {

            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.product_name)) {
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

        startTransaction();

        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@name", item.product_name);
            cmd.Parameters.AddWithValue("@weight", item.product_weight);
            cmd.Parameters.AddWithValue("@fk_product_type", item.fk_product_type_id);
            cmd.Parameters.AddWithValue("@fk_product_category", item.fk_product_category_id);
            cmd.Parameters.AddWithValue("@fk_product_shape", item.fk_product_shape_id);

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


    public ObservableCollection<ProductItem> getProductWithProductTypeId(List<int> product_type_id) {
        return getProductWith(_m_COL_FK_PRODUCT_TYPE, product_type_id);
    }


    public ObservableCollection<ProductItem> getProductWithProductCategoryId(List<int> product_category_id) {
        return getProductWith(_m_COL_FK_PRODUCT_CATEGORY, product_category_id);
    }


    public ObservableCollection<ProductItem> getProductWithProductShapeId(List<int> product_shape_id) {
        return getProductWith(_m_COL_FK_PRODUCT_SHAPE, product_shape_id);
    }


    private ObservableCollection<ProductItem> getProductWith(string col_name, List<int> ids) {

        ObservableCollection<ProductItem> product_items = new();
        

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_WEIGHT}, " +
                $"{_m_COL_FK_PRODUCT_TYPE}, {_m_COL_FK_PRODUCT_CATEGORY}, " +
                $"{_m_COL_FK_PRODUCT_SHAPE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {col_name} IN ({string.Join(",", ids.Select((id, index) => $"@id{index}"))});",
                m_conn
            );

            // Add parameters for each ID
            for (int i = 0; i < ids.Count; i++) {
                cmd.Parameters.AddWithValue($"@id{i}", ids[i]);
            }

            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                product_items.Add(new ProductItem {
                    product_id = reader.GetSafeValue<int>(_m_COL_ID),
                    product_name = reader.GetSafeValue(_m_COL_NAME, string.Empty),
                    product_weight = reader.GetSafeValue(_m_COL_WEIGHT, 0),
                    fk_product_type_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_TYPE, 0),
                    fk_product_category_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_CATEGORY, 0),
                    fk_product_shape_id = reader.GetSafeValue<int>(_m_COL_FK_PRODUCT_SHAPE, 0),
                });
            }
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
        }

        return product_items;
    }
}

