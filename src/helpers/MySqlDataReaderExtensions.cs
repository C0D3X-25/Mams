
using MySqlConnector;

namespace Mams.src.helpers;

public static class MySqlDataReaderExtensions {
    public static T GetSafeValue<T>(this MySqlDataReader reader, string columnName, T defaultValue = default) {
        var ordinal = reader.GetOrdinal(columnName);
        if (reader.IsDBNull(ordinal))
            return defaultValue;

        // Special handling for different types
        if (typeof(T) == typeof(string))
            return (T)(object)reader.GetString(ordinal);
        if (typeof(T) == typeof(int))
            return (T)(object)reader.GetInt32(ordinal);
        if (typeof(T) == typeof(DateTime))
            return (T)(object)reader.GetDateTime(ordinal);

        return (T)reader.GetValue(ordinal);
    }
}
