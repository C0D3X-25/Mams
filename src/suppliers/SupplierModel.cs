using Mams.src.databaseOperations;
using Mams.src.helpers;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.suppliers;

/// <summary>
/// Represents a model for managing supplier data from the Database.
/// </summary>
public class SupplierModel : ABaseModel,
    ICrudOperation<SupplierItem> {

    private const string _m_TBL_NAME = "suppliers";
    private const string _m_COL_ID = "supplier_id";
    private const string _m_COL_FK_ENTITY = "fk_entity_id";

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The delete operation type determines whether the item is permanently removed (<see
    /// cref="EDeleteItemOperation.HARD_DELETE"/>) or marked as deleted (<see
    /// cref="EDeleteItemOperation.SOFT_DELETE"/>).</remarks>
    /// <param name="id">The unique identifier of the item to delete. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.HARD_DELETE"/>.</param>
    /// <returns><see langword="true"/> if the item was successfully deleted; otherwise, <see langword="false"/>.</returns>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE) {
        return SDatabaseModel.deleteRow(id, _m_COL_ID, string.Empty, _m_TBL_NAME, delete_type);
    }


    public SupplierItem? getItemByID(string id) {

        if (string.IsNullOrEmpty(id)) {
            return null;
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_ID} = @id;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@id", id);
            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                return new SupplierItem {
                    supplier_id = reader.getSafeValue<int>(_m_COL_ID),
                    fk_entity_id = reader.getSafeValue<int>(_m_COL_FK_ENTITY, 0)
                };
            }
            return null;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public ObservableCollection<SupplierItem> getTable() {
        return SDatabaseModel.getAllRowsInTable<SupplierItem>(_m_TBL_NAME);
    }


    public int saveItem(SupplierItem item) {

        if (item == null) {
            return 0;
        }

        string query = string.Empty;
        int item_id = item.supplier_id;

        if (item_id == 0) {
            if (isIdenticItemPresentInTable(_m_TBL_NAME, _m_COL_FK_ENTITY, item.fk_entity_id.ToString())) {
                return 0;
            }

            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_FK_ENTITY}) " +
                $"VALUES (@fk_entity); SELECT LAST_INSERT_ID();";
        }
        else {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_FK_ENTITY} = @fk_entity " +
                $"WHERE {_m_COL_ID} = @id;";
        }

        startTransaction();
        try {
            using MySqlCommand cmd = new(query, m_conn, m_transaction);

            if (item_id != 0) {
                cmd.Parameters.AddWithValue("@id", item_id);
            }
            cmd.Parameters.AddWithValue("@fk_entity", item.fk_entity_id);

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


    public SupplierItem? getSupplierWithEntityFK(string fk_entity) {

        if (string.IsNullOrEmpty(fk_entity)) {
            return null;
        }

        SupplierItem item = new();

        try {
            using MySqlCommand cmd = new(
                $"SELECT {_m_COL_ID}, {_m_COL_FK_ENTITY} " +
                $"FROM {_m_TBL_NAME} " +
                $"WHERE {_m_COL_FK_ENTITY} = @fk_entity;",
                m_conn
            );

            cmd.Parameters.AddWithValue("@fk_entity", fk_entity);

            using MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read()) {
                item.supplier_id = reader.getSafeValue<int>(_m_COL_ID);
                item.fk_entity_id = reader.getSafeValue<int>(_m_COL_FK_ENTITY);
            }

            return item;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }


    public bool deleteSupplierWithEntityFK(string fk_entity) {

        if (string.IsNullOrEmpty(fk_entity)) {
            return false;
        }

        SupplierItem? item = getSupplierWithEntityFK(fk_entity);

        if (item == null) {
            return false;
        }

        return deleteItem(item.supplier_id.ToString());
    }
}
