using Mams.src.beehives;
using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.productsLots;

/// <summary>
/// Represents a model for managing product lots in the database.
/// </summary>
public class ProductLotModel : ABaseModel,
    ICrudOperation<ProductLotItem> {

    private const string _m_TBL_NAME = "products_lots";
    private const string _m_COL_ID = "product_lot_id";
    private const string _m_COL_NAME = "product_lot_name";
    private const string _m_COL_YEAR = "product_lot_year";
    private const string _m_COL_FK_BEEHIVE = "fk_beehive_id";
    private const string _m_COL_ARCHIVE = "product_lot_archive";

    // This default data are directly inserted in the database when she is created.
    // They are used because a FK can't be null, so we need to have a default value.
    private const int       _m_DEFAULT_LOT_FK_BEEHIVE = 1;
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
    /// Retrieves a <see cref="ProductLotItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product lot item to retrieve. Must be a valid identifier.</param>
    /// <returns>A <see cref="ProductLotItem"/> object containing the details of the product lot item if found; otherwise, <see
    /// langword="null"/>.</returns>
    public ProductLotItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_YEAR}, {_m_COL_FK_BEEHIVE}, {_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
        );

        ProductLotItem product_lot = new();
        BeehiveModel beehive_model = new();
        int beehive_id = 0;

        cmd.Parameters.AddWithValue("@id", id);

        try {
            {
                using MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read()) {

                    beehive_id = reader.getSafeValue<int>(_m_COL_FK_BEEHIVE, 0);

                    product_lot.product_lot_id = reader.getSafeValue<int>(_m_COL_ID);
                    product_lot.product_lot_name = reader.getSafeValue(_m_COL_NAME, string.Empty);
                    product_lot.product_lot_year = reader.getSafeValue<int>(_m_COL_YEAR);
                    product_lot.fk_beehive_id = beehive_id;
                    product_lot.product_lot_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString();
                }
            }

            product_lot.beehive_name = beehive_model.getItemByID(beehive_id.ToString())?.beehive_name ?? string.Empty;

            return product_lot;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Retrieves a collection of <see cref="ProductLotItem"/> objects from the database table,  with additional beehive
    /// name information populated for each item.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing all <see cref="ProductLotItem"/> objects  from the database
    /// table, with beehive name information populated.</returns>
    public ObservableCollection<ProductLotItem> getTable() {
        ObservableCollection<ProductLotItem> table = SDatabaseModel.getAllRowsInTable<ProductLotItem>(_m_TBL_NAME, _m_COL_ARCHIVE);

        BeehiveModel beehive_model = new();

        foreach (ProductLotItem item in table) {
            item.beehive_name = item.fk_beehive_id > 0 
                ? beehive_model.getItemByID(item.fk_beehive_id.ToString())?.beehive_name ?? string.Empty 
                : string.Empty;
        }
        return table;
    }

    /// <summary>
    /// Saves the specified <see cref="ProductLotItem"/> to the database.
    /// </summary>
    /// <param name="item">The <see cref="ProductLotItem"/> to save. Must not be null.</param>
    /// <returns>The ID of the saved <see cref="ProductLotItem"/>. Returns 0 if the operation fails or if the item is null.</returns>
    public int saveItem(ProductLotItem item) {

        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.product_lot_id;
        string item_name = item.product_lot_name.Trim();

        if (item_id == 0) {
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item_name)) {
                return 0;
            }
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_YEAR}, {_m_COL_FK_BEEHIVE}) " +
                $"VALUES (@name, @year, @fk_beehive); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_YEAR} = @year, {_m_COL_FK_BEEHIVE} = @fk_beehive " +
                $"WHERE {_m_COL_ID} = @id";
        }

        startTransaction();
        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }

            cmd.Parameters.AddWithValue("@name", item_name);
            cmd.Parameters.AddWithValue("@year", item.product_lot_year);
            if (item.fk_beehive_id == 0) {
                item.fk_beehive_id = _m_DEFAULT_LOT_FK_BEEHIVE;
            }
            cmd.Parameters.AddWithValue("@fk_beehive", item.fk_beehive_id);

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

    /// <summary>
    /// Retrieves a collection of product lot items associated with the specified beehive IDs.
    /// </summary>
    /// <param name="beehive_ids">A list of beehive IDs for which to retrieve product lot items. The list must not be empty.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing <see cref="ProductLotItem"/> objects that correspond to the
    /// specified beehive IDs. Returns an empty collection if the input list is empty or if no matching product lot
    /// items are found.</returns>
    public ObservableCollection<ProductLotItem> getProductLotsWithBeehiveId(List<int> beehive_ids) {

        ObservableCollection<ProductLotItem> items = new();

        if (beehive_ids.Count == 0) {
            return items;
        }

        string query = $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_YEAR}, {_m_COL_FK_BEEHIVE} " +
            $"FROM {_m_TBL_NAME} " +
            $"WHERE {_m_COL_FK_BEEHIVE} IN ({string.Join(",", beehive_ids)})";

        try {
            using MySqlCommand cmd = new(query, m_conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read()) {
                items.Add(new ProductLotItem {
                    product_lot_id = reader.getSafeValue<int>(_m_COL_ID),
                    product_lot_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                    product_lot_year = reader.getSafeValue<int>(_m_COL_YEAR),
                    fk_beehive_id = reader.getSafeValue<int>(_m_COL_FK_BEEHIVE)
                });
            }
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
        }
        return items;
    }
}
