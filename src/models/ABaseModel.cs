using Mams.src.databaseConnections;
using MySqlConnector;
using System.Windows;

namespace Mams.src.models;

/// <summary>
/// Base class for all models who directly interact with te Database to inherit from.
/// Holds the SQL connection object for all Classes to use.
/// </summary>
public abstract class ABaseModel {

    /// <summary>
    /// SQL connection model instance used to manage database connections.
    /// </summary>
    private static SQLConnectionModel m_sql_connection_model = new();

    /// <summary>
    /// Every derived Class will use this session to interact with the database.
    /// </summary>
    public static MySqlConnection? conn = m_sql_connection_model.openConnection();

    /// <summary>
    /// Represents the current MySQL transaction associated with the operation, if any.
    /// </summary>
    public static MySqlTransaction? transaction = null;


    public static void startTransaction() {
        if (conn == null) {
            throw new InvalidOperationException("Database connection is not established.");
        }
        transaction = conn.BeginTransaction();
    }

    public static bool isTransactionActive() {
        return transaction != null 
            && transaction.Connection != null 
            && transaction.Connection.State == System.Data.ConnectionState.Open;
    }

    public static void commitTransaction() {
        if (transaction == null) {
            throw new InvalidOperationException("No transaction to commit.");
        }
        transaction.Commit();
        transaction = null;
    }

    public static void rollbackTransaction() {
        if (transaction == null) {
            throw new InvalidOperationException("No transaction to roll back.");
        }
        transaction.Rollback();
        transaction = null;
    }

    /// <summary>
    /// Checks if a specific item exists in a given table and column in the database.
    /// </summary>
    /// <param name="table_name">The name of the table to search in.</param>
    /// <param name="column_to_search">The name of the column to search within.</param>
    /// <param name="item_to_find">The value to search for in the specified column.</param>
    /// <returns>
    /// Returns <c>true</c> if the item exists in the table; otherwise, <c>false</c>.
    /// </returns>
    protected bool isIdenticItemPresentInTable(string table_name, string column_to_search, string item_to_find) {

        try {
            using MySqlCommand cmd = new(
                $"SELECT COUNT(*) FROM {table_name} WHERE {column_to_search} = @item_to_find",
                conn, transaction
            );
            cmd.Parameters.AddWithValue("@item_to_find", item_to_find);
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            return count > 0;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}
