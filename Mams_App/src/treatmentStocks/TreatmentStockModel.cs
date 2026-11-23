using Mams_App.src.databaseOperations;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams_App.src.treatmentStocks;

/// <summary>
/// Represents a model for managing treatment stock data in the database.
/// </summary>
public class TreatmentStockModel : ABaseModel
{
    private const string _m_TBL_NAME = "treatment_stocks";
    private const string _m_COL_ID = "treatment_stock_id";
    private const string _m_COL_PURCHASE_DATE = "treatment_stock_purchase_date";
    private const string _m_COL_INITIAL_QUANTITY = "treatment_stock_initial_quantity";
    private const string _m_COL_FIRST_USED_DATE = "treatment_stock_first_used_date";
    private const string _m_COL_LAST_USED_DATE = "treatment_stock_last_used_date";
    private const string _m_COL_FK_PRODUCT = "fk_product_id";
    private const string _m_COL_FK_DOSE_UNIT = "fk_dose_unit_id";
    private const string _m_COL_FK_SUPPLIER = "fk_supplier_id";

    private const string _m_TBL_PRODUCT = "products";
    private const string _m_COL_PRODUCT_ID = "product_id";
    private const string _m_COL_PRODUCT_NAME = "product_name";

    private const string _m_TBL_DOSE_UNIT = "dose_units";
    private const string _m_COL_DOSE_UNIT_ID = "dose_unit_id";
    private const string _m_COL_DOSE_UNIT_NAME = "dose_unit_name";

    private const string _m_TBL_SUPPLIER = "suppliers";
    private const string _m_COL_SUPPLIER_ID = "supplier_id";
    private const string _m_COL_FK_ENTITY = "fk_entity_id";

    private const string _m_TBL_ENTITY = "entities";
    private const string _m_COL_ENTITY_ID = "entity_id";
    private const string _m_COL_ENTITY_NAME = "entity_name";

    private const string _m_TBL_TREATMENT = "treatments";
    private const string _m_COL_TREATMENT_DATE = "treatment_date";
    private const string _m_COL_TREATMENT_DOSE_PER_HIVE = "treatment_dose_per_hive";
    private const string _m_COL_TREATMENT_HIVE_COUNT = "treatment_hive_count";
    private const string _m_COL_FK_TREATMENT_STOCK = "fk_treatment_stock_id";

    /// <summary>
    /// Saves the specified <see cref="TreatmentStockItem"/> to the database (INSERT or UPDATE).
    /// </summary>
    public ResponseSaveItem saveItem(TreatmentStockItem item)
    {
        if (item == null)
        {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE,
                "TreatmentStockModel.saveItem: Item cannot be null");
        }

        int item_id = item.treatment_stock_id;
        string item_date = SFormatData.formatEUDateToMySQLDate(item.treatment_stock_purchase_date);
        decimal item_initial_quantity = item.treatment_stock_initial_quantity;
        int item_fk_product = item.fk_product_id;
        int item_fk_dose_unit = item.fk_dose_unit_id;
        int item_fk_supplier = item.fk_supplier_id;
        string query;

        bool isInsert = (item_id == 0);

        if (isInsert)
        {
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_PURCHASE_DATE}, {_m_COL_INITIAL_QUANTITY}, {_m_COL_FK_PRODUCT}, {_m_COL_FK_DOSE_UNIT}, {_m_COL_FK_SUPPLIER}) " +
                $"VALUES (@date, @initial_quantity, @fk_product, @fk_dose_unit, @fk_supplier); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else
        {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_PURCHASE_DATE} = @date, {_m_COL_INITIAL_QUANTITY} = @initial_quantity, " +
                $"{_m_COL_FK_PRODUCT} = @fk_product, {_m_COL_FK_DOSE_UNIT} = @fk_dose_unit, {_m_COL_FK_SUPPLIER} = @fk_supplier " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        bool need_transaction = !isTransactionActive();
        startTransaction();

        try
        {
            if (isInsert)
            {
                item_id = executeWithConnection(connection =>
                {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@date", item_date);
                    cmd.Parameters.AddWithValue("@initial_quantity", item_initial_quantity);
                    cmd.Parameters.AddWithValue("@fk_product", item_fk_product);
                    cmd.Parameters.AddWithValue("@fk_dose_unit", item_fk_dose_unit > 0 ? item_fk_dose_unit : DBNull.Value);
                    cmd.Parameters.AddWithValue("@fk_supplier", item_fk_supplier > 0 ? item_fk_supplier : DBNull.Value);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                });
            }
            else
            {
                executeWithConnection(connection =>
                {
                    using MySqlCommand cmd = new(query, connection, m_transaction);
                    cmd.Parameters.AddWithValue("@id", item_id);
                    cmd.Parameters.AddWithValue("@date", item_date);
                    cmd.Parameters.AddWithValue("@initial_quantity", item_initial_quantity);
                    cmd.Parameters.AddWithValue("@fk_product", item_fk_product);
                    cmd.Parameters.AddWithValue("@fk_dose_unit", item_fk_dose_unit > 0 ? item_fk_dose_unit : DBNull.Value);
                    cmd.Parameters.AddWithValue("@fk_supplier", item_fk_supplier > 0 ? item_fk_supplier : DBNull.Value);
                    cmd.ExecuteNonQuery();
                });
            }

            commitTransaction();
            return ResponseSaveItem.Success(item_id);
        }
        catch (MySqlException ex)
        {
            rollbackTransaction();
            return ResponseSaveItem.MySqlFailure(ex.ErrorCode, ex.Message);
        }
    }

    /// <summary>
    /// Deletes a treatment stock from the database (hard delete).
    /// </summary>
    public ResponseDeleteItem deleteItem(string id)
    {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_TBL_NAME, EDeleteItemOperation.HARD_DELETE);
    }

    /// <summary>
    /// Retrieves a <see cref="TreatmentStockItem"/> by its unique identifier.
    /// </summary>
    public ResponseGetItem<TreatmentStockItem> getItemByID(string id)
    {
        if (!SDataValidation.isIdValidForRetrieval(id))
        {
            return ResponseGetItem<TreatmentStockItem>.Failure(EErrors.INVALID_INPUT,
                $"TreatmentStockModel.getItemByID: Invalid ID provided '{id}'");
        }

        return executeWithConnection(connection =>
        {
            try
            {
                string query = $@"
                    SELECT ts.{_m_COL_ID}, ts.{_m_COL_PURCHASE_DATE}, ts.{_m_COL_INITIAL_QUANTITY},
                           ts.{_m_COL_FIRST_USED_DATE}, ts.{_m_COL_LAST_USED_DATE},
                           ts.{_m_COL_FK_PRODUCT}, ts.{_m_COL_FK_DOSE_UNIT}, ts.{_m_COL_FK_SUPPLIER},
                           p.{_m_COL_PRODUCT_NAME},
                           COALESCE(du.{_m_COL_DOSE_UNIT_NAME}, '') AS {_m_COL_DOSE_UNIT_NAME},
                           COALESCE(e.{_m_COL_ENTITY_NAME}, '') AS {_m_COL_ENTITY_NAME},
                           COALESCE(SUM(t.{_m_COL_TREATMENT_HIVE_COUNT} * t.{_m_COL_TREATMENT_DOSE_PER_HIVE}), 0) AS used_quantity
                    FROM {_m_TBL_NAME} ts
                    JOIN {_m_TBL_PRODUCT} p ON ts.{_m_COL_FK_PRODUCT} = p.{_m_COL_PRODUCT_ID}
                    LEFT JOIN {_m_TBL_DOSE_UNIT} du ON ts.{_m_COL_FK_DOSE_UNIT} = du.{_m_COL_DOSE_UNIT_ID}
                    LEFT JOIN {_m_TBL_SUPPLIER} s ON ts.{_m_COL_FK_SUPPLIER} = s.{_m_COL_SUPPLIER_ID}
                    LEFT JOIN {_m_TBL_ENTITY} e ON s.{_m_COL_FK_ENTITY} = e.{_m_COL_ENTITY_ID}
                    LEFT JOIN {_m_TBL_TREATMENT} t ON t.{_m_COL_FK_TREATMENT_STOCK} = ts.{_m_COL_ID}
                    WHERE ts.{_m_COL_ID} = @id
                    GROUP BY ts.{_m_COL_ID};";

                using MySqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return ResponseGetItem<TreatmentStockItem>.Success(readTreatmentStockItem(reader));
                }
                return ResponseGetItem<TreatmentStockItem>.NotFound();
            }
            catch (MySqlException ex)
            {
                return ResponseGetItem<TreatmentStockItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all treatment stock items from the database.
    /// </summary>
    public ResponseGetAllItems<TreatmentStockItem> getAllItems()
    {
        return executeWithConnection(connection =>
        {
            try
            {
                string query = $@"
                    SELECT ts.{_m_COL_ID}, ts.{_m_COL_PURCHASE_DATE}, ts.{_m_COL_INITIAL_QUANTITY},
                           ts.{_m_COL_FIRST_USED_DATE}, ts.{_m_COL_LAST_USED_DATE},
                           ts.{_m_COL_FK_PRODUCT}, ts.{_m_COL_FK_DOSE_UNIT}, ts.{_m_COL_FK_SUPPLIER},
                           p.{_m_COL_PRODUCT_NAME},
                           COALESCE(du.{_m_COL_DOSE_UNIT_NAME}, '') AS {_m_COL_DOSE_UNIT_NAME},
                           COALESCE(e.{_m_COL_ENTITY_NAME}, '') AS {_m_COL_ENTITY_NAME},
                           COALESCE(SUM(t.{_m_COL_TREATMENT_HIVE_COUNT} * t.{_m_COL_TREATMENT_DOSE_PER_HIVE}), 0) AS used_quantity
                    FROM {_m_TBL_NAME} ts
                    JOIN {_m_TBL_PRODUCT} p ON ts.{_m_COL_FK_PRODUCT} = p.{_m_COL_PRODUCT_ID}
                    LEFT JOIN {_m_TBL_DOSE_UNIT} du ON ts.{_m_COL_FK_DOSE_UNIT} = du.{_m_COL_DOSE_UNIT_ID}
                    LEFT JOIN {_m_TBL_SUPPLIER} s ON ts.{_m_COL_FK_SUPPLIER} = s.{_m_COL_SUPPLIER_ID}
                    LEFT JOIN {_m_TBL_ENTITY} e ON s.{_m_COL_FK_ENTITY} = e.{_m_COL_ENTITY_ID}
                    LEFT JOIN {_m_TBL_TREATMENT} t ON t.{_m_COL_FK_TREATMENT_STOCK} = ts.{_m_COL_ID}
                    GROUP BY ts.{_m_COL_ID}
                    ORDER BY ts.{_m_COL_PURCHASE_DATE} DESC;";

                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                ObservableCollection<TreatmentStockItem> items = [];

                while (reader.Read())
                {
                    items.Add(readTreatmentStockItem(reader));
                }

                return ResponseGetAllItems<TreatmentStockItem>.Success(items);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return ResponseGetAllItems<TreatmentStockItem>.Success([]);
            }
        });
    }

    /// <summary>
    /// Retrieves all treatment stocks that still have remaining quantity.
    /// </summary>
    public ObservableCollection<TreatmentStockItem> getAvailableStocks()
    {
        return executeWithConnection(connection =>
        {
            ObservableCollection<TreatmentStockItem> items = [];
            try
            {
                string query = $@"
                    SELECT ts.{_m_COL_ID}, ts.{_m_COL_PURCHASE_DATE}, ts.{_m_COL_INITIAL_QUANTITY},
                           ts.{_m_COL_FIRST_USED_DATE}, ts.{_m_COL_LAST_USED_DATE},
                           ts.{_m_COL_FK_PRODUCT}, ts.{_m_COL_FK_DOSE_UNIT}, ts.{_m_COL_FK_SUPPLIER},
                           p.{_m_COL_PRODUCT_NAME},
                           COALESCE(du.{_m_COL_DOSE_UNIT_NAME}, '') AS {_m_COL_DOSE_UNIT_NAME},
                           COALESCE(e.{_m_COL_ENTITY_NAME}, '') AS {_m_COL_ENTITY_NAME},
                           COALESCE(SUM(t.{_m_COL_TREATMENT_HIVE_COUNT} * t.{_m_COL_TREATMENT_DOSE_PER_HIVE}), 0) AS used_quantity
                    FROM {_m_TBL_NAME} ts
                    JOIN {_m_TBL_PRODUCT} p ON ts.{_m_COL_FK_PRODUCT} = p.{_m_COL_PRODUCT_ID}
                    LEFT JOIN {_m_TBL_DOSE_UNIT} du ON ts.{_m_COL_FK_DOSE_UNIT} = du.{_m_COL_DOSE_UNIT_ID}
                    LEFT JOIN {_m_TBL_SUPPLIER} s ON ts.{_m_COL_FK_SUPPLIER} = s.{_m_COL_SUPPLIER_ID}
                    LEFT JOIN {_m_TBL_ENTITY} e ON s.{_m_COL_FK_ENTITY} = e.{_m_COL_ENTITY_ID}
                    LEFT JOIN {_m_TBL_TREATMENT} t ON t.{_m_COL_FK_TREATMENT_STOCK} = ts.{_m_COL_ID}
                    GROUP BY ts.{_m_COL_ID}
                    HAVING ts.{_m_COL_INITIAL_QUANTITY} - COALESCE(SUM(t.{_m_COL_TREATMENT_HIVE_COUNT} * t.{_m_COL_TREATMENT_DOSE_PER_HIVE}), 0) > 0
                    ORDER BY p.{_m_COL_PRODUCT_NAME} ASC, ts.{_m_COL_PURCHASE_DATE} DESC;";

                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(readTreatmentStockItem(reader));
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            }
            return items;
        });
    }

    /// <summary>
    /// Reads a TreatmentStockItem from the current position of a MySqlDataReader.
    /// </summary>
    private TreatmentStockItem readTreatmentStockItem(MySqlDataReader reader)
    {
        var dateValue = reader.getSafeValue(_m_COL_PURCHASE_DATE, DateOnly.MinValue);
        string dateStr = dateValue.ToString("dd.MM.yyyy");

        var firstUsedValue = reader.getSafeValue(_m_COL_FIRST_USED_DATE, DateOnly.MinValue);
        string firstUsedStr = firstUsedValue == DateOnly.MinValue ? string.Empty : firstUsedValue.ToString("dd.MM.yyyy");

        var lastUsedValue = reader.getSafeValue(_m_COL_LAST_USED_DATE, DateOnly.MinValue);
        string lastUsedStr = lastUsedValue == DateOnly.MinValue ? string.Empty : lastUsedValue.ToString("dd.MM.yyyy");

        return new TreatmentStockItem
        {
            treatment_stock_id = reader.getSafeValue<int>(_m_COL_ID),
            treatment_stock_purchase_date = dateStr,
            treatment_stock_initial_quantity = reader.getSafeValue<decimal>(_m_COL_INITIAL_QUANTITY),
            treatment_stock_first_used_date = firstUsedStr,
            treatment_stock_last_used_date = lastUsedStr,
            fk_product_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT),
            fk_dose_unit_id = reader.getSafeValue<int>(_m_COL_FK_DOSE_UNIT),
            fk_supplier_id = reader.getSafeValue<int>(_m_COL_FK_SUPPLIER),
            product_name = reader.getSafeValue(_m_COL_PRODUCT_NAME, string.Empty),
            dose_unit_name = reader.getSafeValue(_m_COL_DOSE_UNIT_NAME, string.Empty),
            supplier_name = reader.getSafeValue(_m_COL_ENTITY_NAME, string.Empty),
            treatment_stock_used_quantity = reader.getSafeValue<decimal>("used_quantity")
        };
    }

    /// <summary>
    /// Updates the first and last used dates on a treatment stock by computing
    /// MIN/MAX treatment_date from the treatments table.
    /// </summary>
    public void updateUsageDates(int stockId)
    {
        if (stockId <= 0) return;

        executeWithConnection(connection =>
        {
            try
            {
                string query = $@"
                    UPDATE {_m_TBL_NAME}
                    SET {_m_COL_FIRST_USED_DATE} = (
                            SELECT MIN({_m_COL_TREATMENT_DATE}) FROM {_m_TBL_TREATMENT} WHERE {_m_COL_FK_TREATMENT_STOCK} = @id
                        ),
                        {_m_COL_LAST_USED_DATE} = (
                            SELECT MAX({_m_COL_TREATMENT_DATE}) FROM {_m_TBL_TREATMENT} WHERE {_m_COL_FK_TREATMENT_STOCK} = @id
                        )
                    WHERE {_m_COL_ID} = @id;";

                using MySqlCommand cmd = new(query, connection, m_transaction);
                cmd.Parameters.AddWithValue("@id", stockId);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                // Usage dates update is non-critical
            }
        });
    }
}
