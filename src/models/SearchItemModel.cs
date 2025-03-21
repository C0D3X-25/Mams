using MySqlConnector;
using System.Windows;

namespace Mams.src.models;

public class SearchItemModel : ABaseModel {

    public List<string> getItemList(string search, string column, string table, int limit = 10) {
        var results = new List<string>();

        using MySqlConnection? conn = _m_conn.openConnection();

        try {
            // Only search can use @ parameter, table and column must be known before
            using MySqlCommand cmd =
                new(
                    $"SELECT DISTINCT {column} " +
                    $"FROM {table} " +
                    $"WHERE {column} LIKE @search " +
                    $"ORDER BY LENGTH({column}) " +
                    $"LIMIT {limit};",
                    conn
                );

            cmd.Parameters.AddWithValue("@search", $"%{search}%");
            using MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read()) {
                var fieldType = reader.GetFieldType(0);
                switch (Type.GetTypeCode(fieldType)) {
                    case TypeCode.String:
                        results.Add(reader.GetString(0));
                        break;
                    case TypeCode.Int32:
                        results.Add(reader.GetInt32(0).ToString());
                        break;
                    case TypeCode.Int64:
                        results.Add(reader.GetInt64(0).ToString());
                        break;
                    case TypeCode.UInt64:
                        results.Add(reader.GetUInt64(0).ToString());
                        break;
                    case TypeCode.Double:
                        results.Add(reader.GetDouble(0).ToString());
                        break;
                    case TypeCode.Single:
                        results.Add(reader.GetFloat(0).ToString());
                        break;
                    case TypeCode.Decimal:
                        results.Add(reader.GetDecimal(0).ToString());
                        break;
                    case TypeCode.Boolean:
                        results.Add(reader.GetBoolean(0).ToString());
                        break;
                    case TypeCode.DateTime:
                        results.Add(reader.GetDateTime(0).ToString());
                        break;
                    default:
                        throw new InvalidCastException($"Unsupported data type: {fieldType}");
                }
            }
            return results;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return results;
        }
    }
}
