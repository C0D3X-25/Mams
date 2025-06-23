using Mams.src.items;
using Mams.src.models;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace Mams.src.databaseOperations;

/// <summary>
/// Provides static database operations for handling data models in the application.
/// Contains methods for retrieving, deleting, and managing database records.
/// </summary>
public abstract class SDatabaseModel : ABaseModel {

    /// <summary>
    /// Retrieves all records from a specified database table and converts them into a collection of typed objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to create, must inherit from ABaseItem and have a parameterless constructor.</typeparam>
    /// <param name="table">The name of the database table to query.</param>
    /// <returns>
    /// An ObservableCollection of type T containing the converted database records.
    /// Returns an empty collection if an error occurs during data retrieval or conversion.
    /// </returns>
    public static ObservableCollection<T> getAllRowsInTable<T>(string table) where T : ABaseItem, new() {

        ObservableCollection<T> items = new();
        DataTable? data_table = getDataTable(table);

        if (data_table != null) {
            try {
                foreach (DataRow row in data_table.Rows) {
                    T item = new();
                    foreach (DataColumn col in data_table.Columns) {
                        var value = row[col.ColumnName];
                        if (value != DBNull.Value) {
                            var property = typeof(T).GetProperty(col.ColumnName);
                            if (property != null) {
                                property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                            }
                        }
                    }
                    items.Add(item);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Error converting data: {ex.Message}");
                return new ObservableCollection<T>();
            }
        }
        return items;
    }


    /// <summary>
    /// Performs a delete operation on a specified database table based on the provided delete type.
    /// </summary>
    /// <param name="id">The identifier of the record to be deleted or modified.</param>
    /// <param name="field_name_id">The name of the ID field in the database table.</param>
    /// <param name="field_name_archive">The name of the archive date field used for soft deletes.</param>
    /// <param name="table_name">The name of the database table to perform the operation on.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to SOFT_DELETE.</param>
    /// <returns>
    /// Returns true if the operation was successful; otherwise, false.
    /// </returns>
    /// <remarks>
    /// The method supports three types of delete operations:
    /// - Soft Delete: Sets an archive date without removing the record
    /// - Hard Delete: Permanently removes the record from the database
    /// - Restore: Clears the archive date to restore a soft-deleted record
    /// </remarks>
    /// <exception cref="MySqlException">Thrown when a database error occurs during the operation.</exception>
    public static bool deleteRow(string id, string field_name_id, string field_name_archive, string table_name, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {

        string query = string.Empty;

        switch (delete_type) {
            // Archive the data
            case EDeleteItemOperation.SOFT_DELETE:
                query = $"UPDATE {table_name} " +
                    $"SET {field_name_archive} = CURDATE() " +
                    $"WHERE {field_name_id} = @id";
                break;
            // Complete delete of the data
            case EDeleteItemOperation.HARD_DELETE:
                query = $"DELETE FROM {table_name} WHERE {field_name_id} = @id;";
                break;
            // Check if the data is linked in another table, then SOFT_DELETE or HARD_DELETE
            case EDeleteItemOperation.SAFE_DELETE:
                // TODO: check if the linked tables is used, then SOFT_DELETE or HARD_DELETE
                break;
            // Restore the data from the archive
            case EDeleteItemOperation.RESTORE:
                query = $"UPDATE {table_name} " +
                    $"SET {field_name_archive} = NULL " +
                    $"WHERE {field_name_id} = @id";
                break;
        }

        bool need_transaction = false;
        if (isTransactionActive()) {
            need_transaction = true;
        }
        if (need_transaction) {
            transaction = conn?.BeginTransaction();
        }

        try {
            using MySqlCommand cmd = new(query, conn, transaction);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            if (need_transaction) {
                transaction?.Commit();
            }
            return true;
        }
        catch (MySqlException ex) {
            if (need_transaction) {
                transaction?.Rollback();
            }
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }


    /// <summary>
    /// Retrieves all records from a specified database table.
    /// </summary>
    /// <param name="table">The name of the database table to query.</param>
    /// <returns>
    /// A DataTable containing all records from the specified table.
    /// Returns null if an error occurs during the database operation.
    /// </returns>
    private static DataTable? getDataTable(string table) {

        //using MySqlConnection? conn = ABaseModel.conn;

        DataTable data_table = new();

        try {
            using MySqlCommand cmd = new($"SELECT * FROM {table};", conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            data_table.Load(reader);

            return data_table;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }
}
