using Mams.src.helpers;
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
/// Even if the Class is abstract, all methods are static and can be used without instantiation.
/// </summary>
public abstract class SDatabaseModel : ABaseModel {

    private static string _m_DEFAULT_ARCHIVE_DATE = "1901-01-01";

    /// <summary>
    /// Retrieves all records from a specified database table and converts them into a collection of typed objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to create, must inherit from ABaseItem and have a parameterless constructor.</typeparam>
    /// <param name="table">The name of the database table to query.</param>
    /// <param name="asc_column">The column who need to be order alphabetically.</param>
    /// <param name="archive_field">The column where the archive is set (can be null).</param>
    /// <returns>
    /// An ObservableCollection of type T containing the converted database records.
    /// Returns an empty collection if an error occurs during data retrieval or conversion.
    /// </returns>
    public static ObservableCollection<T> getAllRowsInTable<T>(string table, string? asc_column = null, string? archive_field = null) where T : ABaseItem, new() {
        DataTable? data_table = getDataTable(table, asc_column, archive_field);
        return populateColumnName<T>(data_table);
    }

    /// <summary>
    /// Performs a delete operation on a specified database table based on the provided delete type.
    /// </summary>
    /// <param name="id">The identifier of the record to be deleted or modified.</param>
    /// <param name="field_id">The name of the ID field in the database table.</param>
    /// <param name="field_archive">The name of the archive date field used for soft deletes.</param>
    /// <param name="table">The name of the database table to perform the operation on.</param>
    /// <param name="delete_type">The type of delete operation to perform.</param>
    /// <returns>
    /// Returns true if the operation was successful; otherwise, false.
    /// </returns>
    public static bool deleteRow(
        string id, 
        string field_id, 
        string field_archive,
        string table, 
        EDeleteItemOperation delete_type
        ) {
        if (!areDeleteParametersProvided(id, field_id, table, delete_type)) {
            return false;
        }

        return deleteOperation(id, field_id, field_archive, table, delete_type);
    }

    /// <summary>
    /// Performs a delete operation on a specified database table based on the provided delete type.
    /// </summary>
    /// <param name="id">The identifier of the record to be deleted or modified.</param>
    /// <param name="field_id">The name of the ID field in the database table.</param>
    /// <param name="table">The name of the database table to perform the operation on.</param>
    /// <param name="delete_type">The type of delete operation to perform.</param>
    /// <returns>
    /// Returns true if the operation was successful; otherwise, false.
    /// </returns>
    public static bool deleteRow(
        string id,
        string field_id,
        string table,
        EDeleteItemOperation delete_type
        ) {
        if (!areDeleteParametersProvided(id, field_id, table, delete_type)) {
            return false;
        }

        return deleteOperation(id, field_id, string.Empty, table, delete_type);
    }

    /// <summary>
    /// Converts data from a DataTable to a collection of typed objects by mapping column names to object properties.
    /// </summary>
    /// <typeparam name="T">The type of objects to create. Must inherit from ABaseItem and have a parameterless constructor.</typeparam>
    /// <param name="data_table">The DataTable containing the data to be converted. Can be null.</param>
    /// <returns>
    /// An ObservableCollection of type T containing the converted database records.
    /// </returns>
    private static ObservableCollection<T> populateColumnName<T>(DataTable? data_table) where T : ABaseItem, new() {
        if (data_table == null) {
            return new ObservableCollection<T>();
        }

        try {
            ObservableCollection<T> items = new();
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
            return items;
        }
        catch (Exception ex) {
            MessageBox.Show($"Error converting data: {ex.Message}");
            return new ObservableCollection<T>();
        }
    }

    /// <summary>
    /// Deletes or modifies a record in the specified database table based on the provided operation type.
    /// </summary>
    /// <param name="id">The unique identifier of the record to be deleted or modified.</param>
    /// <param name="field_id">The name of the field representing the record's unique identifier.</param>
    /// <param name="field_archive">The name of the field used for archiving records.</param>
    /// <param name="table">The name of the database table containing the record.</param>
    /// <param name="delete_type">The type of delete operation to perform.</param>
    /// <returns><see langword="true"/> if the operation is successful; otherwise, <see langword="false"/>.</returns>
    private static bool deleteOperation(
        string id,
        string field_id,
        string field_archive,
        string table,
        EDeleteItemOperation delete_type
        ) {
        
        string query = string.Empty;

        switch (delete_type) {
            // Archive the record
            case EDeleteItemOperation.SOFT_DELETE:
                if (!isArchiveFieldProvided(field_archive)) {
                    return false;
                }
                query = $"UPDATE {table} " +
                    $"SET {field_archive} = CURDATE() " +
                    $"WHERE {field_id} = @id";
                break;
            // Complete delete of the record
            case EDeleteItemOperation.HARD_DELETE:
                query = $"DELETE FROM {table} WHERE {field_id} = @id;";
                break;
            // Check if the record is linked in another table, then SOFT_DELETE or HARD_DELETE
            case EDeleteItemOperation.SAFE_DELETE:
                if (!isArchiveFieldProvided(field_archive)) {
                    return false;
                }
                query = $"DELETE FROM {table} WHERE {field_id} = @id;";
                break;
            // Restore the record from the archive
            case EDeleteItemOperation.RESTORE:
                if (!isArchiveFieldProvided(field_archive)) {
                    return false;
                }
                query = $"UPDATE {table} " +
                    $"SET {field_archive} = NULL " +
                    $"WHERE {field_id} = @id";
                break;
        }

        bool need_transaction = false;
        if (!isTransactionActive()) {
            need_transaction = true;
            startTransaction();
        }

        try {
            // Use transaction connection directly since we already have a transaction started
            ExecuteWithConnection(connection => {
                using MySqlCommand cmd = new(query, connection, m_transaction);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            });

            if (need_transaction) {
                commitTransaction();
            }
            return true;
        }
        catch (MySqlException ex) {
            // In case of a foreign key constraint violation, try soft delete if archive field is provided
            if ((ex.ErrorCode == MySqlErrorCode.RowIsReferenced2 || 
                 ex.ErrorCode == MySqlErrorCode.RowIsReferenced) && 
                isArchiveFieldProvided(field_archive)) {
                clearTransaction();
                return deleteOperation(
                    id, 
                    field_id, 
                    field_archive, 
                    table, 
                    EDeleteItemOperation.SOFT_DELETE
                );
            }
            if (need_transaction) {
                rollbackTransaction();
            }
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Determines whether the provided archive field name is valid for a soft delete operation.
    /// </summary>
    /// <param name="field_archive">The name of the archive field to validate.</param>
    /// <returns><see langword="true"/> if the archive field name is valid; otherwise, <see langword="false"/>.</returns>
    private static bool isArchiveFieldProvided(string field_archive) {
        if (string.IsNullOrWhiteSpace(field_archive)) {
            MessageBox.Show("Archive field name cannot be empty for soft delete operation.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Determines whether the required parameters for a delete operation are valid and provided.
    /// </summary>
    /// <param name="id">The identifier of the item to be deleted.</param>
    /// <param name="field_id">The name of the field representing the item's identifier in the database.</param>
    /// <param name="table">The name of the database table where the item resides.</param>
    /// <param name="delete_type">The type of delete operation to perform.</param>
    /// <returns><see langword="true"/> if all required parameters are valid and provided; otherwise, <see langword="false"/>.</returns>
    private static bool areDeleteParametersProvided(string id, string field_id, string table, EDeleteItemOperation delete_type) {
        if (!SDataValidation.isIdValid(id)
            || string.IsNullOrWhiteSpace(field_id)
            || string.IsNullOrWhiteSpace(table)
            || delete_type == EDeleteItemOperation.NONE
            ) {
            MessageBox.Show("Invalid parameters provided for delete operation.");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Retrieves all records from a specified database table.
    /// </summary>
    /// <param name="table">The name of the database table to query.</param>
    /// <param name="asc_column">The column who need to be order alphabetically (can be null).</param>
    /// <param name="archive_field">The column where the archive is set (can be null).</param>
    /// <returns>
    /// A DataTable containing all records from the specified table.
    /// Returns null if an error occurs during the database operation.
    /// </returns>
    private static DataTable? getDataTable(string table, string? asc_column, string? archive_field) {
        if (string.IsNullOrWhiteSpace(table)) {
            return null;
        }

        string query = string.Empty;

        if (asc_column == null) {
            if (archive_field == null) {
                query = $"SELECT * FROM {table};";
            }
            else {
                query = $"SELECT * FROM {table} " +
                    $"WHERE {archive_field} != '{_m_DEFAULT_ARCHIVE_DATE}' " +
                    $"OR {archive_field} IS NULL;";
            }
        }
        else {
            if (archive_field == null) {
                query = $"SELECT * FROM {table} ORDER BY {asc_column} ASC;";
            }
            else {
                query = $"SELECT * FROM {table} " +
                    $"WHERE {archive_field} != '{_m_DEFAULT_ARCHIVE_DATE}' " +
                    $"OR {archive_field} IS NULL " +
                    $"ORDER BY {asc_column} ASC;";
            }
        }
            
        // Use ExecuteWithConnection to get a connection from the pool
        return ExecuteWithConnection<DataTable?>(connection => {
            try {
                DataTable data_table = new();
                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();
                data_table.Load(reader);

                return data_table;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return null;
            }
        });
    }

    /// <summary>
    /// Retrieves all records from a specified database table ordered by date.
    /// </summary>
    /// <param name="table">The name of the database table to query.</param>
    /// <param name="asc_column">The column who need to be order by date.</param>
    /// <param name="archive_field">The column where the archive is set (can be null).</param>
    /// <returns>
    /// A DataTable containing all records from the specified table.
    /// Returns null if an error occurs during the database operation.
    /// </returns>
    private static DataTable? getDataTableOrderByDate(string table, string asc_column, string? archive_field) {
        if (string.IsNullOrWhiteSpace(table)) {
            return null;
        }

        string query = string.Empty;

        if (asc_column == null) {
            if (archive_field == null) {
                query = $"SELECT * FROM {table};";
            }
            else {
                query = $"SELECT * FROM {table} " +
                    $"WHERE {archive_field} != '{_m_DEFAULT_ARCHIVE_DATE}' " +
                    $"OR {archive_field} IS NULL;";
            }
        }
        else {
            if (archive_field == null) {
                query = $"SELECT * FROM {table} ORDER BY {asc_column} ASC;";
            }
            else {
                query = $"SELECT * FROM {table} " +
                    $"WHERE {archive_field} != '{_m_DEFAULT_ARCHIVE_DATE}' " +
                    $"OR {archive_field} IS NULL " +
                    $"ORDER BY {asc_column} ASC;";
            }
        }
        
        // Use ExecuteWithConnection to get a connection from the pool
        return ExecuteWithConnection<DataTable?>(connection => {
            try {
                DataTable data_table = new();
                using MySqlCommand cmd = new(query, connection);
                using MySqlDataReader reader = cmd.ExecuteReader();
                data_table.Load(reader);

                return data_table;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return null;
            }
        });
    }
}
