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
/// Evem if the Class is abstract, all methods are static and can be used without instantiation.
/// </summary>
public abstract class SDatabaseModel : ABaseModel {

    private static string _m_DEFAULT_ARCHIVE_DATE = "1901-01-01";

    /// <summary>
    /// Retrieves all records from a specified database table and converts them into a collection of typed objects.
    /// </summary>
    /// <typeparam name="T">The type of objects to create, must inherit from ABaseItem and have a parameterless constructor.</typeparam>
    /// <param name="table">The name of the database table to query.</param>
    /// <param name="asc_column">The column who need to be order alphabetically (can be null).</param>
    /// <param name="archive_field">The column where the archive is set (can be null).</param>
    /// <returns>
    /// An ObservableCollection of type T containing the converted database records.
    /// Returns an empty collection if an error occurs during data retrieval or conversion.
    /// </returns>
    public static ObservableCollection<T> getAllRowsInTable<T>(string table, string? asc_column = null, string? archive_field = null) where T : ABaseItem, new() {

        ObservableCollection<T> items = new();
        DataTable? data_table = getDataTable(table, asc_column, archive_field);

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
    /// <param name="field_id">The name of the ID field in the database table.</param>
    /// <param name="field_archive">The name of the archive date field used for soft deletes.</param>
    /// <param name="table">The name of the database table to perform the operation on.</param>
    /// <param name="delete_type">The type of delete operation to perform.</param>
    /// <returns>
    /// Returns true if the operation was successful; otherwise, false.
    /// </returns>
    /// <remarks>
    /// The method supports three types of delete operations:
    /// - Soft Delete: Sets an archive date without removing the record (work only if archive filed is provided)
    /// - Hard Delete: Permanently removes the record from the database
    /// - Safe Delete: Intended for checking linked records in other tables (work only if archive filed is provided)
    /// - Restore: Clears the archive date to restore a soft-deleted record (work only if archive filed is provided)
    /// - None: Will cause an error
    /// </remarks>
    /// <exception cref="MySqlException">Thrown when a database error occurs during the operation.</exception>
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
    /// <remarks>
    /// The method supports three types of delete operations:
    /// - Soft Delete: Sets an archive date without removing the record (work only if archive filed is provided)
    /// - Hard Delete: Permanently removes the record from the database
    /// - Safe Delete: Intended for checking linked records in other tables (work only if archive filed is provided)
    /// - Restore: Clears the archive date to restore a soft-deleted record (work only if archive filed is provided)
    /// - None: Will cause an error
    /// </remarks>
    /// <exception cref="MySqlException">Thrown when a database error occurs during the operation.</exception>
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
    /// Deletes or modifies a record in the specified database table based on the provided operation type.
    /// </summary>
    /// <remarks>This method supports multiple deletion operations, including soft delete, hard delete, safe
    /// delete, and restore. - **Soft Delete**: Archives the record by setting the archive field to the current date. -
    /// **Hard Delete**: Permanently deletes the record from the table. - **Safe Delete**: Deletes the record only if it
    /// is not referenced by other tables; otherwise, performs a soft delete. - **Restore**: Restores a previously
    /// archived record by clearing the archive field.  The method requires valid field names and table names to
    /// construct the query. If the archive field is not provided for operations that require it (e.g., soft delete or
    /// restore), the method will return <see langword="false"/>.  Transactions are used to ensure data integrity, and
    /// the method will automatically start, commit, or roll back transactions as needed. In case of a foreign key
    /// constraint violation during a hard delete, the method will attempt a soft delete if the archive field is
    /// provided.</remarks>
    /// <param name="id">The unique identifier of the record to be deleted or modified. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="field_id">The name of the field representing the record's unique identifier in the database table. Cannot be <see
    /// langword="null"/> or empty.</param>
    /// <param name="field_archive">The name of the field used for archiving records. Required for soft delete and restore operations. Can be <see
    /// langword="null"/> for hard delete.</param>
    /// <param name="table">The name of the database table containing the record. Cannot be <see langword="null"/> or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Must be one of the values defined in <see
    /// cref="EDeleteItemOperation"/>.</param>
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
            using MySqlCommand cmd = new(query, m_conn, m_transaction);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();

            if (need_transaction) {
                commitTransaction();
            }
            return true;
        }
        catch (MySqlException ex) {

            // In case of a foreign key constraint violation, we can try to soft delete the record
            // if the archive field is provided, otherwise there is a rollback
            if (ex.ErrorCode == MySqlErrorCode.RowIsReferenced2
                || ex.ErrorCode == MySqlErrorCode.RowIsReferenced
                && isArchiveFieldProvided(field_archive)
                ) {
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
    /// <param name="field_archive">The name of the archive field to validate. Must not be null, empty, or consist solely of whitespace.</param>
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
    /// <param name="id">The identifier of the item to be deleted. Cannot be null, empty, or whitespace.</param>
    /// <param name="field_id">The name of the field representing the item's identifier in the database. Cannot be null, empty, or whitespace.</param>
    /// <param name="table">The name of the database table where the item resides. Cannot be null, empty, or whitespace.</param>
    /// <param name="delete_type">The type of delete operation to perform. Must not be <see cref="EDeleteItemOperation.NONE"/>.</param>
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

        DataTable data_table = new();
        
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
            try {
                using MySqlCommand cmd = new(query, m_conn);
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
