using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.beehives;

/// <summary>
/// Represents a model for managing beehive data in the database.
/// Implements CRUD (Create, Read, Update, Delete) operations for beehive items.
/// </summary>
public class BeehiveModel
    : ABaseModel,
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
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    /// <summary>
    /// Retrieves a beehive item by its unique identifier.
    /// </summary>
    /// <param name="id">The ID of the beehive to retrieve.</param>
    /// <returns>The BeehiveItem if found, null otherwise.</returns>
    public BeehiveItem? getItemByID(string id) {
        
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
                    beehive_id = reader.GetSafeValue<int>(_m_COL_ID),
                    beehive_name = reader.GetSafeValue(_m_COL_NAME, string.Empty),
                    beehive_archive = reader.GetSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
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
    /// Retrieves all beehive items from the database.
    /// </summary>
    /// <returns>An observable collection of all beehive items.</returns>
    public ObservableCollection<BeehiveItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<BeehiveItem>(_m_TBL_NAME, _m_COL_ARCHIVE);
    }

    /// <summary>
    /// Creates new record if it doesn't exist in the Database,
    /// updates existing record if it does.
    /// </summary>
    /// <param name="item">The BeehiveItem to save.</param>
    /// <returns>The ID of the entry if the save operation was successful, 0 otherwise.</returns>
    public int saveItem(BeehiveItem item) {

        string query = string.Empty;
        int item_id = item.beehive_id;

        if (item_id == 0) {
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.beehive_name)) {
                return 0;
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}) " +
                $"VALUES (@name); SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        try {
            using MySqlCommand cmd = new(query, m_conn);
            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@name", item.beehive_name);

            if (item_id == 0) {
                item_id = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else {
                cmd.ExecuteNonQuery();
            }

            return item_id;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return 0;
        }
    }
}
