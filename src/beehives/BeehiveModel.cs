using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.beehives;

/// <summary>
/// Represents a model for managing beehive data in the database.
/// </summary>
public class BeehiveModel : ABaseModel,
    ICrudOperation<BeehiveItem> {

    private const string _m_TBL_NAME = "beehives";
    private const string _m_COL_ID = "beehive_id";
    private const string _m_COL_NAME = "beehive_name";
    private const string _m_COL_ARCHIVE = "beehive_archive";

    /// <summary>
    /// Deletes a beehive item from the database.
    /// </summary>
    /// <param name="id">The ID of the beehive to delete.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to soft delete.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    /// <summary>
    /// Retrieves a <see cref="BeehiveItem"/> object by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item to retrieve. Must be a valid ID string.</param>
    /// <returns>A <see cref="BeehiveItem"/> object representing the item with the specified ID,  or <see langword="null"/> if no
    /// matching item is found or if the ID is invalid.</returns>
    public BeehiveItem? getItemByID(string id) {

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
                return new BeehiveItem {
                    beehive_id = reader.getSafeValue<int>(_m_COL_ID),
                    beehive_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                    beehive_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
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
    /// Retrieves all rows from the beehive table as an observable collection.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing all rows in the beehive table. If the table is empty, the
    /// collection will be empty.</returns>
    public ObservableCollection<BeehiveItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<BeehiveItem>(_m_TBL_NAME, _m_COL_ARCHIVE);
    }

    /// <summary>
    /// Saves the specified <see cref="BeehiveItem"/> to the database.
    /// </summary>
    /// <param name="item">The <see cref="BeehiveItem"/> to save. Cannot be <see langword="null"/>.</param>
    /// <returns>The ID of the saved item. Returns <c>0</c> if the operation fails, the item is <see langword="null"/>, or the
    /// item name already exists in the database.</returns>
    public int saveItem(BeehiveItem item) {

        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.beehive_id;
        string item_name = item.beehive_name.Trim();

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
            using MySqlCommand cmd = new(query, m_conn);
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
