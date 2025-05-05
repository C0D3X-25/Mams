using Mams.src.databaseConnections;
using MySqlConnector;
using System.Windows;

namespace Mams.src.models;

/// <summary>
/// Base class for all models to inherit from.
/// Holds the SQL connection object for all models to use.
/// </summary>
public abstract class BaseModel {


    /// <summary>
    /// SQL connection model instance used to manage database connections.
    /// </summary>
    public readonly SQLConnectionModel _m_conn = new();



    /// <summary>
    /// Checks if a specific item exists in a given table and column in the database.
    /// </summary>
    /// <param name="table_name">The name of the table to search in.</param>
    /// <param name="colomn_to_search">The name of the column to search within.</param>
    /// <param name="item_to_find">The value to search for in the specified column.</param>
    /// <returns>
    /// Returns <c>true</c> if the item exists in the table; otherwise, <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method uses a parameterized query to prevent SQL injection.
    /// If a MySQL exception occurs, an error message is displayed, and <c>false</c> is returned.
    /// </remarks>
    protected bool checkIfItemExist(string table_name, string colomn_to_search, string item_to_find) {

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            using MySqlCommand cmd = new(
                $"SELECT COUNT(*) FROM {table_name} WHERE {colomn_to_search} = @data",
                conn
            );
            cmd.Parameters.AddWithValue("@data", item_to_find);
            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return false;
        }
    }
}
