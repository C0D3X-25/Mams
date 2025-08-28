using Mams.src.databaseConnections;
using MySqlConnector;
using System.Windows;

namespace Mams.src.models;

/// <summary>
/// Base class for all models who directly interact with the Database to inherit from.
/// Manages database connections and transactions through connection pooling.
/// </summary>
public abstract class ABaseModel {
    private static SQLConnectionModel _m_sql_connection_model = new();
    
    // Connection used for the current transaction - only active during transactions
    private static MySqlConnection? _m_transaction_connection = null;
    private static MySqlTransaction? _m_transaction = null;

    /// <summary>
    /// Gets a connection from the pool or returns the active transaction connection if in a transaction
    /// </summary>
    protected static MySqlConnection GetConnection() {
        // If we're in a transaction, use the transaction connection
        if (_m_transaction_connection != null && _m_transaction != null) {
            return _m_transaction_connection;
        }
        
        // Otherwise get a new connection from the pool
        return _m_sql_connection_model.GetConnection();
    }

    /// <summary>
    /// For executing queries that don't return a value
    /// </summary>
    protected static void ExecuteWithConnection(Action<MySqlConnection> action) {
        var isTransactionConnection = (_m_transaction_connection != null);
        var connection = GetConnection();
        
        try {
            action(connection);
        }
        finally {
            // Only dispose the connection if it's not the transaction connection
            if (!isTransactionConnection) {
                connection.Dispose();
            }
        }
    }

    /// <summary>
    /// For executing queries that return a value
    /// </summary>
    protected static T ExecuteWithConnection<T>(Func<MySqlConnection, T> func) {
        var isTransactionConnection = (_m_transaction_connection != null);
        var connection = GetConnection();
        
        try {
            return func(connection);
        }
        finally {
            // Only dispose the connection if it's not the transaction connection
            if (!isTransactionConnection) {
                connection.Dispose();
            }
        }
    }

    /// <summary>
    /// Gets the current transaction
    /// </summary>
    protected static MySqlTransaction? m_transaction {
        get { return _m_transaction; }
    }

    /// <summary>
    /// Starts a new database transaction.
    /// </summary>
    /// <remarks>
    /// Creates a dedicated connection for the transaction that will be used
    /// for all database operations until the transaction is committed or rolled back.
    /// </remarks>
    public static void startTransaction() {
        // If there's already a transaction, throw an exception
        if (_m_transaction != null || _m_transaction_connection != null) {
            throw new InvalidOperationException("A transaction is already active.");
        }
        
        // Get a new connection for this transaction
        _m_transaction_connection = _m_sql_connection_model.GetConnection();
        _m_transaction = _m_transaction_connection.BeginTransaction();
    }

    /// <summary>
    /// Determines whether a transaction is currently active.
    /// </summary>
    /// <returns>True if a transaction is active and its connection is open; otherwise, false.</returns>
    public static bool isTransactionActive() {
        return _m_transaction != null 
            && _m_transaction_connection != null 
            && _m_transaction_connection.State == System.Data.ConnectionState.Open;
    }

    /// <summary>
    /// Commits the current transaction, finalizing all changes made during the transaction.
    /// </summary>
    public static void commitTransaction() {
        if (_m_transaction == null || _m_transaction_connection == null) {
            throw new InvalidOperationException("No transaction to commit.");
        }

        try {
            _m_transaction.Commit();
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            throw;
        }
        finally {
            // Cleanup after commit
            _m_transaction.Dispose();
            _m_transaction = null;
            _m_transaction_connection.Dispose();
            _m_transaction_connection = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction, if one exists.
    /// </summary>
    public static void rollbackTransaction() {
        if (_m_transaction == null || _m_transaction_connection == null) {
            throw new InvalidOperationException("No transaction to roll back.");
        }
        
        try {
            _m_transaction.Rollback();
        }
        finally {
            // Cleanup after rollback
            _m_transaction.Dispose();
            _m_transaction = null;
            _m_transaction_connection.Dispose();
            _m_transaction_connection = null;
        }
    }

    /// <summary>
    /// Clears the current transaction, releasing any associated resources.
    /// </summary>
    public static void clearTransaction() {
        if (_m_transaction == null) {
            return;
        }
        
        try {
            _m_transaction.Dispose();
        }
        catch { /* Ignore errors when clearing */ }
        finally {
            _m_transaction = null;
            
            if (_m_transaction_connection != null) {
                try {
                    _m_transaction_connection.Dispose();
                }
                catch { /* Ignore errors when clearing */ }
                finally {
                    _m_transaction_connection = null;
                }
            }
        }
    }

    /// <summary>
    /// Checks if a specific item exists in a given table and column in the database.
    /// </summary>
    protected bool isIdenticItemPresentInTable(string table_name, string column_to_search, string item_to_find) {
        if (string.IsNullOrEmpty(table_name) 
            || string.IsNullOrEmpty(column_to_search) 
            || string.IsNullOrEmpty(item_to_find)
            ){
            throw new ArgumentException("Table name, column to search, and item to find cannot be null or empty.");
        }

        return ExecuteWithConnection(connection => {
            try {
                using MySqlCommand cmd = new(
                    $"SELECT COUNT(*) FROM {table_name} WHERE {column_to_search} = @item_to_find",
                    connection, _m_transaction
                );
                cmd.Parameters.AddWithValue("@item_to_find", item_to_find);
                int count = Convert.ToInt32(cmd.ExecuteScalar());

                return count > 0;
            }
            catch (MySqlException ex) {
                MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
                return false;
            }
        });
    }
}
