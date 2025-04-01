using Mams.src.enums;
using MySqlConnector;
using System.Windows;

namespace Mams.src.models;

/// <summary>
/// Provides static functionality for deleting items from database tables with support for soft delete, hard delete, and restore operations.
/// </summary>
public static class DeleteItemModel {

    /// <summary>
    /// Performs a delete operation on a specified database table based on the provided delete type.
    /// </summary>
    /// <param name="model">The base model instance containing the database connection.</param>
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
    public static bool deleteItem(ABaseModel model, string id, string field_name_id, string field_name_archive, string table_name, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        using MySqlConnection? conn = model._m_conn.openConnection();

        string query = String.Empty;

        switch (delete_type) {
            case EDatabaseDeleteItem.SOFT_DELETE:
                query = $"UPDATE {table_name} " +
                    $"SET {field_name_archive} = CURDATE() " +
                    $"WHERE {field_name_id} = @id";
                break;
            case EDatabaseDeleteItem.HARD_DELETE:
                query = $"DELETE FROM {table_name} WHERE {field_name_id} = @id;";
                break;
            case EDatabaseDeleteItem.RESTORE:
                query = $"UPDATE {table_name} " +
                    $"SET {field_name_archive} = NULL " +
                    $"WHERE {field_name_id} = @id";
                break;
        }

        try {
            using MySqlCommand cmd =
                new(query, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}
