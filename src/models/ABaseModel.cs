using MySqlConnector;
using System.Windows;

namespace Mams.src.models;

/// <summary>
/// Base class for all models to inherit from.
/// Holds the SQL connection object for all models to use.
/// </summary>
public abstract class ABaseModel {
    public readonly SQLConnectionModel _m_conn = new();


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
