using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.entities;

/// <summary>
/// Represents a model for managing entities in a database./>
/// </summary>
public class EntityModel : ABaseModel,
    ICrudOperation<EntityItem> {

    private const string _m_TBL_NAME  = "entities";
    private const string _m_COL_ID = "entity_id";
    private const string _m_COL_NAME = "entity_name";
    private const string _m_COL_PHONE = "entity_phone";
    private const string _m_COL_EMAIL = "entity_email";
    private const string _m_COL_CITY = "entity_city";
    private const string _m_COL_ADDRESS = "entity_address";
    private const string _m_COL_ARCHIVE = "entity_archive";

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
    /// Retrieves an entity item from the database by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity item to retrieve. This value must not be null or empty.</param>
    /// <returns>An <see cref="EntityItem"/> object representing the entity item if found; otherwise, <see langword="null"/>.</returns>
    public EntityItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}, {_m_COL_ARCHIVE} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id ",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new EntityItem {
                    entity_id = reader.getSafeValue<int>(_m_COL_ID),
                    entity_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                    entity_phone = reader.getSafeValue(_m_COL_PHONE, string.Empty),
                    entity_email = reader.getSafeValue(_m_COL_EMAIL, string.Empty),
                    entity_city = reader.getSafeValue(_m_COL_CITY, string.Empty),
                    entity_address = reader.getSafeValue(_m_COL_ADDRESS, string.Empty),
                    entity_archive = reader.getSafeValue(_m_COL_ARCHIVE, DateOnly.MinValue).ToString()
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
    /// Retrieves all rows from the specified table as an observable collection of <see cref="EntityItem"/>.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing all rows in the table represented as <see
    /// cref="EntityItem"/> objects. If the table is empty, the collection will be empty.</returns>
    public ObservableCollection<EntityItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<EntityItem>(_m_TBL_NAME, _m_COL_NAME);
    }

    /// <summary>
    /// Saves the specified <see cref="EntityItem"/> to the database.
    /// </summary>
    /// <param name="item">The <see cref="EntityItem"/> to save. Cannot be <c>null</c>.</param>
    /// <returns>The ID of the saved item. Returns <c>0</c> if the operation fails, the item is <c>null</c>, or a duplicate item
    /// is detected.</returns>
    public int saveItem(EntityItem item) {

        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.entity_id;

        if (item_id == 0) {
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_NAME, item.entity_name)) {
                return 0;
            }
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}) " +
                $"VALUES (@name, @phone, @email, @city, @address); " +
                $"SELECT LAST_INSERT_ID(); ";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_PHONE} = @phone, {_m_COL_EMAIL} = @email, {_m_COL_CITY} = @city, {_m_COL_ADDRESS} = @address " +
                $"WHERE {_m_COL_ID} = @id";
        }

        startTransaction();
        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@name", item.entity_name.Trim());
            cmd.Parameters.AddWithValue("@phone", item.entity_phone.Trim());
            cmd.Parameters.AddWithValue("@email", item.entity_email.Trim());
            cmd.Parameters.AddWithValue("@city", item.entity_city.Trim());
            cmd.Parameters.AddWithValue("@address", item.entity_address.Trim());

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
