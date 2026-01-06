using Mams_App.src.databaseConnections;
using MySqlConnector;
using System.Windows;

namespace Mams_App.src.models;

/// <summary>
/// Base class for all models who directly interact with the Database to inherit from.
/// Manages database connections and transactions through connection pooling.
/// </summary>
public abstract class ABaseModel {
    private static SQLConnectionModel _m_sql_connection_model = new();
    
    // Connection used for the current transaction - only active during transactions
    private static MySqlConnection? _m_sql_connection = null;
    private static MySqlTransaction? _m_sql_transaction = null;
    
    // Track transaction nesting depth - only the outermost caller commits/rollbacks
    private static int _m_transaction_depth = 0;

    /// <summary>
    /// Gets a connection from the pool or returns the active transaction connection if in a transaction
    /// </summary>
    protected static MySqlConnection getConnection() {
        // If we're in a transaction, use the transaction connection
        if (_m_sql_connection != null && _m_sql_transaction != null) {
            return _m_sql_connection;
        }
        
        // Otherwise get a new connection from the pool
        return SQLConnectionModel.GetConnection();
    }

    /// <summary>
    /// For executing queries that don't return a value
    /// </summary>
    protected static void executeWithConnection(Action<MySqlConnection> action) {
        var is_transaction_connected = (_m_sql_connection != null);
        var connection = getConnection();
        
        try {
            action(connection);
        }
        finally {
            // Only dispose the connection if it's not the transaction connection
            if (!is_transaction_connected) {
                connection.Dispose();
            }
        }
    }

    /// <summary>
    /// For executing queries that return a value
    /// </summary>
    protected static T executeWithConnection<T>(Func<MySqlConnection, T> func) {
        var is_transaction_connected = (_m_sql_connection != null);
        var connection = getConnection();
        
        try {
            return func(connection);
        }
        finally {
            // Only dispose the connection if it's not the transaction connection
            if (!is_transaction_connected) {
                connection.Dispose();
            }
        }
    }

    /// <summary>
    /// Gets the current transaction
    /// </summary>
    protected static MySqlTransaction? m_transaction {
        get { return _m_sql_transaction; }
    }

    /// <summary>
    /// Starts a new database transaction or increments the nesting depth if one is already active.
    /// </summary>
    /// <remarks>
    /// Uses a nesting counter to support nested transaction calls. Only the first call
    /// actually starts a database transaction. Subsequent calls increment the depth counter.
    /// </remarks>
    public static void startTransaction() {
        _m_transaction_depth++;
        
        // Only start a real transaction if this is the first/outermost call
        if (_m_transaction_depth == 1) {
            _m_sql_connection = SQLConnectionModel.GetConnection();
            _m_sql_transaction = _m_sql_connection.BeginTransaction();
        }
    }

    /// <summary>
    /// Determines whether a transaction is currently active.
    /// </summary>
    /// <returns>True if a transaction is active and its connection is open; otherwise, false.</returns>
    public static bool isTransactionActive() {
        return _m_transaction_depth > 0
            && _m_sql_transaction != null 
            && _m_sql_connection != null 
            && _m_sql_connection.State == System.Data.ConnectionState.Open;
    }

    /// <summary>
    /// Commits the current transaction if this is the outermost caller, otherwise decrements the nesting depth.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no transaction is active.</exception>
    public static void commitTransaction() {
        if (_m_transaction_depth <= 0 || _m_sql_transaction == null || _m_sql_connection == null) {
            throw new InvalidOperationException("No transaction to commit.");
        }

        _m_transaction_depth--;
        
        // Only commit if this is the outermost caller
        if (_m_transaction_depth == 0) {
            try {
                _m_sql_transaction.Commit();
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                throw;
            }
            finally {
                cleanupTransaction();
            }
        }
    }

    /// <summary>
    /// Rolls back the current transaction regardless of nesting depth.
    /// </summary>
    /// <remarks>
    /// A rollback always affects the entire transaction, regardless of which nested level initiated it.
    /// This resets the nesting depth to zero. This method is safe to call multiple times - 
    /// subsequent calls after the first rollback will be no-ops.
    /// </remarks>
    public static void rollbackTransaction() {
        // If no transaction is active, just return (already rolled back or never started)
        if (_m_transaction_depth <= 0 || _m_sql_transaction == null || _m_sql_connection == null) {
            return;
        }
        
        try {
            _m_sql_transaction.Rollback();
        }
        finally {
            _m_transaction_depth = 0;
            cleanupTransaction();
        }
    }

    /// <summary>
    /// Cleans up transaction resources.
    /// </summary>
    private static void cleanupTransaction() {
        if (_m_sql_transaction != null) {
            try {
                _m_sql_transaction.Dispose();
            }
            catch { /* Ignore errors when cleaning up */ }
            finally {
                _m_sql_transaction = null;
            }
        }
        
        if (_m_sql_connection != null) {
            try {
                _m_sql_connection.Dispose();
            }
            catch { /* Ignore errors when cleaning up */ }
            finally {
                _m_sql_connection = null;
            }
        }
    }

    /// <summary>
    /// Clears the current transaction, releasing any associated resources.
    /// </summary>
    public static void clearTransaction() {
        _m_transaction_depth = 0;
        cleanupTransaction();
    }

    /// <summary>
    /// Checks if a specific item exists in a given table and column in the database.
    /// </summary>
    protected static bool isIdenticItemPresentInTable(string table_name, string column_to_search, string item_to_find)
    {
        if (string.IsNullOrEmpty(table_name) 
            || string.IsNullOrEmpty(column_to_search) 
            || string.IsNullOrEmpty(item_to_find)
            )
        {
            throw new ArgumentException("Table name, column to search, and item to find cannot be null or empty.");
        }

        return executeWithConnection(connection =>
        {
            try 
            {
                using MySqlCommand cmd = new(
                    $"SELECT COUNT(*) FROM {table_name} WHERE {column_to_search} = @item_to_find",
                    connection, _m_sql_transaction
                );
                cmd.Parameters.AddWithValue("@item_to_find", item_to_find);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return false;
            }
        });
    }
}
