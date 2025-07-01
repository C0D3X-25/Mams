using Mams.src.databaseConnections;
using MySqlConnector;
using System.Windows;

namespace Mams.src.models;

/// <summary>
/// Base class for all models who directly interact with te Database to inherit from.
/// Holds the SQL connection object for all Classes to use.
/// </summary>
public abstract class ABaseModel {

    private static SQLConnectionModel _m_sql_connection_model = new();

    private static MySqlConnection? _m_connection = _m_sql_connection_model.openConnection();
    /// <summary>
    /// Every derived Class will use this session to interact with the database.
    /// </summary>
    protected static MySqlConnection? m_conn {
        get { return _m_connection; }
    }

    private static MySqlTransaction? _m_transaction = null;
    /// <summary>
    /// Represents the current MySQL transaction associated with the operation, if any.
    /// </summary>
    protected static MySqlTransaction? m_transaction {
        get { return _m_transaction; }
    }

    /// <summary>
    /// Starts a new database transaction on the current connection.
    /// </summary>
    /// <remarks>This method initializes a transaction on the active database connection.  Ensure that a valid
    /// connection is established before calling this method.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the database connection is not established.</exception>
    public static void startTransaction() {
        if (m_conn == null) {
            throw new InvalidOperationException("Database connection is not established.");
        }
        _m_transaction = m_conn.BeginTransaction();
    }

    /// <summary>
    /// Determines whether a transaction is currently active.
    /// </summary>
    /// <remarks>A transaction is considered active if it is not null, has an associated connection,  and the
    /// connection's state is <see cref="System.Data.ConnectionState.Open"/>.</remarks>
    /// <returns><see langword="true"/> if a transaction is active and its associated connection is open;  otherwise, <see
    /// langword="false"/>.</returns>
    public static bool isTransactionActive() {
        return m_transaction != null 
            && m_transaction.Connection != null 
            && m_transaction.Connection.State == System.Data.ConnectionState.Open;
    }

    /// <summary>
    /// Commits the current transaction, finalizing all changes made during the transaction.
    /// </summary>
    /// <remarks>This method ensures that all operations performed within the transaction are permanently
    /// applied. If no transaction is active, an exception is thrown. After committing, the transaction is
    /// cleared. 
    /// Transaction is set to <see langword="null"/> if the commit is a success.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if there is no active transaction to commit.</exception>
    public static void commitTransaction() {
        if (m_transaction == null) {
            throw new InvalidOperationException("No transaction to commit.");
        }

        try {
            m_transaction.Commit();
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            throw;
        }
        finally {
            _m_transaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction, if one exists.
    /// </summary>
    /// <remarks>This method reverts all changes made during the current transaction and resets the
    /// transaction state. If no transaction is active, an <see cref="InvalidOperationException"/> is thrown.
    /// Transaction is set to <see langword="null"/> if the rollback is a success.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if there is no active transaction to roll back.</exception>
    public static void rollbackTransaction() {
        if (m_transaction == null) {
            throw new InvalidOperationException("No transaction to roll back.");
        }
        m_transaction.Rollback();
        _m_transaction = null;
    }

    /// <summary>
    /// Clears the current transaction, releasing any associated resources.
    /// </summary>
    /// <remarks>If no transaction is active, the method does nothing. If a transaction is active, it is
    /// disposed and the reference is set to <see langword="null"/>.</remarks>
    public static void clearTransaction() {
        if (m_transaction == null) {
            return;
        }
        m_transaction.Dispose();
        _m_transaction = null;
    }


    /// <summary>
    /// Checks if a specific item exists in a given table and column in the database.
    /// </summary>
    /// <param name="table_name">The name of the table to search in.</param>
    /// <param name="column_to_search">The name of the column to search within.</param>
    /// <param name="item_to_find">The value to search for in the specified column.</param>
    /// <returns>
    /// Returns <c>true</c> if the item exists in the table; otherwise, <see langword="false"/>.
    /// </returns>
    protected bool isIdenticItemPresentInTable(string table_name, string column_to_search, string item_to_find) {

        if (string.IsNullOrEmpty(table_name) 
            || string.IsNullOrEmpty(column_to_search) 
            || string.IsNullOrEmpty(item_to_find)
            ){
            throw new ArgumentException("Table name, column to search, and item to find cannot be null or empty.");
        }

        try {
            using MySqlCommand cmd = new(
                $"SELECT COUNT(*) FROM {table_name} WHERE {column_to_search} = @item_to_find",
                m_conn, m_transaction
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
