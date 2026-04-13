using Mams_App.src.databaseOperations;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams_App.src.treatments;

/// <summary>
/// Represents a model for managing treatment data in the database.
/// </summary>
public class TreatmentModel : ABaseModel
{
    private const string _m_TBL_NAME = "treatments";
    private const string _m_COL_ID = "treatment_id";
    private const string _m_COL_DATE = "treatment_date";
    private const string _m_COL_HIVE_COUNT = "treatment_hive_count";
    private const string _m_COL_DOSE_PER_HIVE = "treatment_dose_per_hive";
    private const string _m_COL_FK_BEEHIVE = "fk_beehive_id";
    private const string _m_COL_FK_PRODUCT = "fk_product_id";

    private const string _m_TBL_BEEHIVE = "beehives";
    private const string _m_COL_BEEHIVE_ID = "beehive_id";
    private const string _m_COL_BEEHIVE_NAME = "beehive_name";
    private const string _m_COL_BEEHIVE_NUMBER = "beehive_number";
    private const string _m_COL_FK_REGION = "fk_region_id";

    private const string _m_TBL_PRODUCT = "products";
    private const string _m_COL_PRODUCT_ID = "product_id";
    private const string _m_COL_PRODUCT_NAME = "product_name";

    private const string _m_TBL_REGION = "regions";
    private const string _m_COL_REGION_ID = "region_id";
    private const string _m_COL_REGION_NAME = "region_name";

    /// <summary>
    /// Saves the specified <see cref="TreatmentItem"/> to the database (INSERT or UPDATE).
    /// </summary>
    /// <param name="item">The <see cref="TreatmentItem"/> to save.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved item and any error message.</returns>
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
        int item_fk_product = item.fk_product_id;
        string query;

        bool isInsert = (item_id == 0);

        if (isInsert)
        {
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_DATE}, {_m_COL_HIVE_COUNT}, {_m_COL_DOSE_PER_HIVE}, {_m_COL_FK_BEEHIVE}, {_m_COL_FK_PRODUCT}) " +
                $"VALUES (@date, @hive_count, @dose_per_hive, @fk_beehive, @fk_product); " +
                $"SELECT LAST_INSERT_ID();";
        }
        else
        {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_DATE} = @date, {_m_COL_HIVE_COUNT} = @hive_count, {_m_COL_DOSE_PER_HIVE} = @dose_per_hive, " +
                $"{_m_COL_FK_BEEHIVE} = @fk_beehive, {_m_COL_FK_PRODUCT} = @fk_product " +
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
                    cmd.Parameters.AddWithValue("@fk_product", item_fk_product);
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
                    cmd.Parameters.AddWithValue("@fk_product", item_fk_product);
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
    /// Deletes a treatment from the database (hard delete).
    /// </summary>
    /// <param name="id">The unique identifier of the treatment to delete.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation.</returns>
    public ResponseDeleteItem deleteItem(string id)
    {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_TBL_NAME, EDeleteItemOperation.HARD_DELETE);
    }

    /// <summary>
    /// Retrieves a <see cref="TreatmentItem"/> by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the treatment to retrieve.</param>
    /// <returns>A <see cref="ResponseGetItem{TreatmentItem}"/> containing the item.</returns>
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
                string query = $@"
                    SELECT t.{_m_COL_ID}, t.{_m_COL_DATE}, t.{_m_COL_HIVE_COUNT},
                           t.{_m_COL_DOSE_PER_HIVE}, t.{_m_COL_FK_BEEHIVE}, t.{_m_COL_FK_PRODUCT},
                           b.{_m_COL_BEEHIVE_NAME}, b.{_m_COL_BEEHIVE_NUMBER}, p.{_m_COL_PRODUCT_NAME},
                           COALESCE(r.{_m_COL_REGION_NAME}, '') AS {_m_COL_REGION_NAME}
                    FROM {_m_TBL_NAME} t
                    JOIN {_m_TBL_BEEHIVE} b ON t.{_m_COL_FK_BEEHIVE} = b.{_m_COL_BEEHIVE_ID}
                    JOIN {_m_TBL_PRODUCT} p ON t.{_m_COL_FK_PRODUCT} = p.{_m_COL_PRODUCT_ID}
                    LEFT JOIN {_m_TBL_REGION} r ON b.{_m_COL_FK_REGION} = r.{_m_COL_REGION_ID}
                    WHERE t.{_m_COL_ID} = @id;";

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
    /// <returns>A <see cref="ResponseGetAllItems{TreatmentItem}"/> containing all treatments.</returns>
    public ResponseGetAllItems<TreatmentItem> getAllItems()
    {
        return executeWithConnection(connection =>
        {
            try
            {
                string query = $@"
                    SELECT t.{_m_COL_ID}, t.{_m_COL_DATE}, t.{_m_COL_HIVE_COUNT},
                           t.{_m_COL_DOSE_PER_HIVE}, t.{_m_COL_FK_BEEHIVE}, t.{_m_COL_FK_PRODUCT},
                           b.{_m_COL_BEEHIVE_NAME}, b.{_m_COL_BEEHIVE_NUMBER}, p.{_m_COL_PRODUCT_NAME},
                           COALESCE(r.{_m_COL_REGION_NAME}, '') AS {_m_COL_REGION_NAME}
                    FROM {_m_TBL_NAME} t
                    JOIN {_m_TBL_BEEHIVE} b ON t.{_m_COL_FK_BEEHIVE} = b.{_m_COL_BEEHIVE_ID}
                    JOIN {_m_TBL_PRODUCT} p ON t.{_m_COL_FK_PRODUCT} = p.{_m_COL_PRODUCT_ID}
                    LEFT JOIN {_m_TBL_REGION} r ON b.{_m_COL_FK_REGION} = r.{_m_COL_REGION_ID}
                    ORDER BY t.{_m_COL_DATE} DESC;";

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
    /// <param name="filterTable">The table to filter by.</param>
    /// <param name="filterId">The ID of the filter item.</param>
    /// <param name="yearFilter">The year to filter by.</param>
    /// <returns>A <see cref="ResponseGetAllItems{TreatmentItem}"/> containing filtered treatments.</returns>
    public ResponseGetAllItems<TreatmentItem> getFilteredItems(EDatabaseTableName filterTable, int filterId, string yearFilter)
    {
        return executeWithConnection(connection =>
        {
            try
            {
                string query = $@"
                    SELECT t.{_m_COL_ID}, t.{_m_COL_DATE}, t.{_m_COL_HIVE_COUNT},
                           t.{_m_COL_DOSE_PER_HIVE}, t.{_m_COL_FK_BEEHIVE}, t.{_m_COL_FK_PRODUCT},
                           b.{_m_COL_BEEHIVE_NAME}, b.{_m_COL_BEEHIVE_NUMBER}, p.{_m_COL_PRODUCT_NAME},
                           COALESCE(r.{_m_COL_REGION_NAME}, '') AS {_m_COL_REGION_NAME}
                    FROM {_m_TBL_NAME} t
                    JOIN {_m_TBL_BEEHIVE} b ON t.{_m_COL_FK_BEEHIVE} = b.{_m_COL_BEEHIVE_ID}
                    JOIN {_m_TBL_PRODUCT} p ON t.{_m_COL_FK_PRODUCT} = p.{_m_COL_PRODUCT_ID}
                    LEFT JOIN {_m_TBL_REGION} r ON b.{_m_COL_FK_REGION} = r.{_m_COL_REGION_ID}
                    WHERE 1=1";

                using MySqlCommand cmd = new();
                cmd.Connection = connection;

                if (filterTable == EDatabaseTableName.BEEHIVE && filterId > 0)
                {
                    query += $" AND t.{_m_COL_FK_BEEHIVE} = @filterId";
                    cmd.Parameters.AddWithValue("@filterId", filterId);
                }
                else if (filterTable == EDatabaseTableName.PRODUCT && filterId > 0)
                {
                    query += $" AND t.{_m_COL_FK_PRODUCT} = @filterId";
                    cmd.Parameters.AddWithValue("@filterId", filterId);
                }
                else if (filterTable == EDatabaseTableName.REGION && filterId > 0)
                {
                    query += $" AND b.{_m_COL_FK_REGION} = @filterId";
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
    /// <returns>An ObservableCollection of year strings.</returns>
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

        return new TreatmentItem
        {
            treatment_id = reader.getSafeValue<int>(_m_COL_ID),
            treatment_date = dateStr,
            treatment_hive_count = reader.getSafeValue<int>(_m_COL_HIVE_COUNT),
            treatment_dose_per_hive = reader.getSafeValue<decimal>(_m_COL_DOSE_PER_HIVE),
            fk_beehive_id = reader.getSafeValue<int>(_m_COL_FK_BEEHIVE),
            fk_product_id = reader.getSafeValue<int>(_m_COL_FK_PRODUCT),
            beehive_name = reader.getSafeValue(_m_COL_BEEHIVE_NAME, string.Empty),
            beehive_number = reader.getSafeValue(_m_COL_BEEHIVE_NUMBER, string.Empty),
            product_name = reader.getSafeValue(_m_COL_PRODUCT_NAME, string.Empty),
            region_name = reader.getSafeValue(_m_COL_REGION_NAME, string.Empty)
        };
    }
}
