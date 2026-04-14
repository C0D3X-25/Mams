using Mams_App.src.databaseOperations;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.models;
using Mams_App.src.treatmentStocks;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams_App.src.treatments;

/// <summary>
/// Represents a model for managing treatment data in the database.
/// </summary>
public class TreatmentModel : ABaseModel
{
    private readonly TreatmentStockModel _m_treatment_stock_model = new();

    private const string _m_TBL_NAME = "treatments";
    private const string _m_COL_ID = "treatment_id";
    private const string _m_COL_DATE = "treatment_date";
    private const string _m_COL_HIVE_COUNT = "treatment_hive_count";
    private const string _m_COL_DOSE_PER_HIVE = "treatment_dose_per_hive";
    private const string _m_COL_FK_BEEHIVE = "fk_beehive_id";
    private const string _m_COL_FK_TREATMENT_STOCK = "fk_treatment_stock_id";

    private const string _m_TBL_BEEHIVE = "beehives";
    private const string _m_COL_BEEHIVE_ID = "beehive_id";
    private const string _m_COL_BEEHIVE_NAME = "beehive_name";
    private const string _m_COL_BEEHIVE_NUMBER = "beehive_number";
    private const string _m_COL_FK_REGION = "fk_region_id";

    private const string _m_TBL_TREATMENT_STOCK = "treatment_stocks";
    private const string _m_COL_TREATMENT_STOCK_ID = "treatment_stock_id";
    private const string _m_COL_STOCK_INITIAL_QUANTITY = "treatment_stock_initial_quantity";
    private const string _m_COL_STOCK_FK_PRODUCT = "fk_product_id";
    private const string _m_COL_STOCK_FK_DOSE_UNIT = "fk_dose_unit_id";
    private const string _m_COL_STOCK_FK_SUPPLIER = "fk_supplier_id";

    private const string _m_TBL_PRODUCT = "products";
    private const string _m_COL_PRODUCT_ID = "product_id";
    private const string _m_COL_PRODUCT_NAME = "product_name";

    private const string _m_TBL_REGION = "regions";
    private const string _m_COL_REGION_ID = "region_id";
    private const string _m_COL_REGION_NAME = "region_name";

    private const string _m_TBL_DOSE_UNIT = "dose_units";
    private const string _m_COL_DOSE_UNIT_ID = "dose_unit_id";
    private const string _m_COL_DOSE_UNIT_NAME = "dose_unit_name";

    private const string _m_TBL_SUPPLIER = "suppliers";
    private const string _m_COL_SUPPLIER_ID = "supplier_id";
    private const string _m_COL_FK_ENTITY = "fk_entity_id";

    private const string _m_TBL_ENTITY = "entities";
    private const string _m_COL_ENTITY_ID = "entity_id";
    private const string _m_COL_ENTITY_NAME = "entity_name";

    /// <summary>
    /// Common SELECT + FROM + JOIN clause used by all retrieval queries.
    /// </summary>
    private string getBaseSelectQuery()
    {
        return $@"
            SELECT t.{_m_COL_ID}, t.{_m_COL_DATE}, t.{_m_COL_HIVE_COUNT},
                   t.{_m_COL_DOSE_PER_HIVE}, t.{_m_COL_FK_BEEHIVE}, t.{_m_COL_FK_TREATMENT_STOCK},
                   b.{_m_COL_BEEHIVE_NAME}, b.{_m_COL_BEEHIVE_NUMBER},
                   p.{_m_COL_PRODUCT_NAME},
                   COALESCE(r.{_m_COL_REGION_NAME}, '') AS {_m_COL_REGION_NAME},
                   COALESCE(du.{_m_COL_DOSE_UNIT_NAME}, '') AS {_m_COL_DOSE_UNIT_NAME},
                   COALESCE(e.{_m_COL_ENTITY_NAME}, '') AS {_m_COL_ENTITY_NAME},
                   ts.{_m_COL_STOCK_INITIAL_QUANTITY},
                   COALESCE(stock_usage.used_quantity, 0) AS used_quantity
            FROM {_m_TBL_NAME} t
            JOIN {_m_TBL_BEEHIVE} b ON t.{_m_COL_FK_BEEHIVE} = b.{_m_COL_BEEHIVE_ID}
            JOIN {_m_TBL_TREATMENT_STOCK} ts ON t.{_m_COL_FK_TREATMENT_STOCK} = ts.{_m_COL_TREATMENT_STOCK_ID}
            JOIN {_m_TBL_PRODUCT} p ON ts.{_m_COL_STOCK_FK_PRODUCT} = p.{_m_COL_PRODUCT_ID}
            LEFT JOIN {_m_TBL_REGION} r ON b.{_m_COL_FK_REGION} = r.{_m_COL_REGION_ID}
            LEFT JOIN {_m_TBL_DOSE_UNIT} du ON ts.{_m_COL_STOCK_FK_DOSE_UNIT} = du.{_m_COL_DOSE_UNIT_ID}
            LEFT JOIN {_m_TBL_SUPPLIER} sup ON ts.{_m_COL_STOCK_FK_SUPPLIER} = sup.{_m_COL_SUPPLIER_ID}
            LEFT JOIN {_m_TBL_ENTITY} e ON sup.{_m_COL_FK_ENTITY} = e.{_m_COL_ENTITY_ID}
            LEFT JOIN (
                SELECT {_m_COL_FK_TREATMENT_STOCK},
                       SUM({_m_COL_HIVE_COUNT} * {_m_COL_DOSE_PER_HIVE}) AS used_quantity
                FROM {_m_TBL_NAME}
                GROUP BY {_m_COL_FK_TREATMENT_STOCK}
            ) stock_usage ON ts.{_m_COL_TREATMENT_STOCK_ID} = stock_usage.{_m_COL_FK_TREATMENT_STOCK}";
    }

    /// <summary>
    /// Saves the specified <see cref="TreatmentItem"/> to the database (INSERT or UPDATE).
    /// </summary>
    public ResponseSaveItem saveItem(TreatmentItem item)
    {
        if (item == null)
        {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE,
                "TreatmentModel.saveItem: Item cannot be null");
        }

        int item_id = item.treatment_id;
        string item_date = SFormatData.formatEUDateToMySQLDate(item.treatment_date);
        int item_hive_count = item.treatment_hive_count;
        decimal item_dose_per_hive = item.treatment_dose_per_hive;
        int item_fk_beehive = item.fk_beehive_id;
        int item_fk_treatment_stock = item.fk_treatment_stock_id;
        string query;

        bool isInsert = (item_id == 0);

        if (isInsert)
        {
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_DATE}, {_m_COL_HIVE_COUNT}, {_m_COL_DOSE_PER_HIVE}, {_m_COL_FK_BEEHIVE}, {_m_COL_FK_TREATMENT_STOCK}) " +
                $"VALUES (@date, @hive_count, @dose_per_hive, @fk_beehive, @fk_treatment_stock); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else
        {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_DATE} = @date, {_m_COL_HIVE_COUNT} = @hive_count, {_m_COL_DOSE_PER_HIVE} = @dose_per_hive, " +
                $"{_m_COL_FK_BEEHIVE} = @fk_beehive, {_m_COL_FK_TREATMENT_STOCK} = @fk_treatment_stock " +
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
                    cmd.Parameters.AddWithValue("@hive_count", item_hive_count);
                    cmd.Parameters.AddWithValue("@dose_per_hive", item_dose_per_hive);
                    cmd.Parameters.AddWithValue("@fk_beehive", item_fk_beehive);
                    cmd.Parameters.AddWithValue("@fk_treatment_stock", item_fk_treatment_stock);
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
                    cmd.Parameters.AddWithValue("@hive_count", item_hive_count);
                    cmd.Parameters.AddWithValue("@dose_per_hive", item_dose_per_hive);
                    cmd.Parameters.AddWithValue("@fk_beehive", item_fk_beehive);
                    cmd.Parameters.AddWithValue("@fk_treatment_stock", item_fk_treatment_stock);
                    cmd.ExecuteNonQuery();
                });
            }

            commitTransaction();
            _m_treatment_stock_model.updateUsageDates(item_fk_treatment_stock);
            return ResponseSaveItem.Success(item_id);
        }
        catch (MySqlException ex)
        {
            rollbackTransaction();
            return ResponseSaveItem.MySqlFailure(ex.ErrorCode, ex.Message);
        }
    }

    /// <summary>
    /// Deletes a treatment from the database (hard delete).
    /// </summary>
    public ResponseDeleteItem deleteItem(string id)
    {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_TBL_NAME, EDeleteItemOperation.HARD_DELETE);
    }

    /// <summary>
    /// Retrieves a <see cref="TreatmentItem"/> by its unique identifier.
    /// </summary>
    public ResponseGetItem<TreatmentItem> getItemByID(string id)
    {
        if (!SDataValidation.isIdValidForRetrieval(id))
        {
            return ResponseGetItem<TreatmentItem>.Failure(EErrors.INVALID_INPUT,
                $"TreatmentModel.getItemByID: Invalid ID provided '{id}'");
        }

        return executeWithConnection(connection =>
        {
            try
            {
                string query = getBaseSelectQuery() + $" WHERE t.{_m_COL_ID} = @id;";

                using MySqlCommand cmd = new(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return ResponseGetItem<TreatmentItem>.Success(readTreatmentItem(reader));
                }
                return ResponseGetItem<TreatmentItem>.NotFound();
            }
            catch (MySqlException ex)
            {
                return ResponseGetItem<TreatmentItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Retrieves all treatment items from the database.
    /// </summary>
    public ResponseGetAllItems<TreatmentItem> getAllItems()
    {
        return executeWithConnection(connection =>
        {
            try
            {
                string query = getBaseSelectQuery() + $" ORDER BY t.{_m_COL_DATE} DESC;";

                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                ObservableCollection<TreatmentItem> items = [];

                while (reader.Read())
                {
                    items.Add(readTreatmentItem(reader));
                }

                return ResponseGetAllItems<TreatmentItem>.Success(items);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return ResponseGetAllItems<TreatmentItem>.Success([]);
            }
        });
    }

    /// <summary>
    /// Retrieves filtered treatment items from the database.
    /// </summary>
    public ResponseGetAllItems<TreatmentItem> getFilteredItems(EDatabaseTableName filterTable, int filterId, string yearFilter)
    {
        return executeWithConnection(connection =>
        {
            try
            {
                string query = getBaseSelectQuery() + " WHERE 1=1";

                using MySqlCommand cmd = new();
                cmd.Connection = connection;

                if (filterTable == EDatabaseTableName.BEEHIVE && filterId > 0)
                {
                    query += $" AND t.{_m_COL_FK_BEEHIVE} = @filterId";
                    cmd.Parameters.AddWithValue("@filterId", filterId);
                }
                else if (filterTable == EDatabaseTableName.PRODUCT && filterId > 0)
                {
                    query += $" AND ts.{_m_COL_STOCK_FK_PRODUCT} = @filterId";
                    cmd.Parameters.AddWithValue("@filterId", filterId);
                }
                else if (filterTable == EDatabaseTableName.REGION && filterId > 0)
                {
                    query += $" AND b.{_m_COL_FK_REGION} = @filterId";
                    cmd.Parameters.AddWithValue("@filterId", filterId);
                }
                else if (filterTable == EDatabaseTableName.DOSE_UNIT && filterId > 0)
                {
                    query += $" AND ts.{_m_COL_STOCK_FK_DOSE_UNIT} = @filterId";
                    cmd.Parameters.AddWithValue("@filterId", filterId);
                }
                else if (filterTable == EDatabaseTableName.TREATMENT_STOCK && filterId > 0)
                {
                    query += $" AND t.{_m_COL_FK_TREATMENT_STOCK} = @filterId";
                    cmd.Parameters.AddWithValue("@filterId", filterId);
                }

                if (!string.IsNullOrEmpty(yearFilter))
                {
                    query += $" AND YEAR(t.{_m_COL_DATE}) = @year";
                    cmd.Parameters.AddWithValue("@year", yearFilter);
                }

                query += $" ORDER BY t.{_m_COL_DATE} DESC;";
                cmd.CommandText = query;

                using MySqlDataReader reader = cmd.ExecuteReader();

                ObservableCollection<TreatmentItem> items = [];

                while (reader.Read())
                {
                    items.Add(readTreatmentItem(reader));
                }

                return ResponseGetAllItems<TreatmentItem>.Success(items);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return ResponseGetAllItems<TreatmentItem>.Success([]);
            }
        });
    }

    /// <summary>
    /// Retrieves a list of distinct years from treatment dates for filtering.
    /// </summary>
    public ObservableCollection<string> getDistinctYears()
    {
        return executeWithConnection(connection =>
        {
            ObservableCollection<string> years = [];
            try
            {
                string query = $"SELECT DISTINCT YEAR({_m_COL_DATE}) AS year FROM {_m_TBL_NAME} ORDER BY year DESC;";
                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    years.Add(reader.GetInt32(0).ToString());
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            }
            return years;
        });
    }

    /// <summary>
    /// Reads a TreatmentItem from the current position of a MySqlDataReader.
    /// </summary>
    private TreatmentItem readTreatmentItem(MySqlDataReader reader)
    {
        var dateValue = reader.getSafeValue(_m_COL_DATE, DateOnly.MinValue);
        string dateStr = dateValue.ToString("dd.MM.yyyy");

        decimal initialQty = reader.getSafeValue<decimal>(_m_COL_STOCK_INITIAL_QUANTITY);
        decimal usedQty = reader.getSafeValue<decimal>("used_quantity");

        return new TreatmentItem
        {
            treatment_id = reader.getSafeValue<int>(_m_COL_ID),
            treatment_date = dateStr,
            treatment_hive_count = reader.getSafeValue<int>(_m_COL_HIVE_COUNT),
            treatment_dose_per_hive = reader.getSafeValue<decimal>(_m_COL_DOSE_PER_HIVE),
            fk_beehive_id = reader.getSafeValue<int>(_m_COL_FK_BEEHIVE),
            fk_treatment_stock_id = reader.getSafeValue<int>(_m_COL_FK_TREATMENT_STOCK),
            beehive_name = reader.getSafeValue(_m_COL_BEEHIVE_NAME, string.Empty),
            beehive_number = reader.getSafeValue(_m_COL_BEEHIVE_NUMBER, string.Empty),
            product_name = reader.getSafeValue(_m_COL_PRODUCT_NAME, string.Empty),
            region_name = reader.getSafeValue(_m_COL_REGION_NAME, string.Empty),
            dose_unit_name = reader.getSafeValue(_m_COL_DOSE_UNIT_NAME, string.Empty),
            supplier_name = reader.getSafeValue(_m_COL_ENTITY_NAME, string.Empty),
            stock_initial_quantity = initialQty,
            stock_remaining_quantity = initialQty - usedQty
        };
    }
}
