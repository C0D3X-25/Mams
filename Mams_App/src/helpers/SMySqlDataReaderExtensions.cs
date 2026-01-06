using MySqlConnector;

namespace Mams_App.src.helpers;

public static class SMySqlDataReaderExtensions {

    /// <summary>
    /// Retrieves the value of the specified column from a <see cref="MySqlDataReader"/> as the specified type. If the
    /// column value is <see langword="null"/> or cannot be converted to the specified type, a default value is
    /// returned.
    /// </summary>
    /// <remarks>This method provides special handling for common types such as <see cref="string"/>, <see
    /// cref="int"/>, <see cref="DateOnly"/>, <see cref="double"/>, and <see cref="decimal"/>. For unsupported types,
    /// the method attempts to retrieve the value using <see cref="MySqlDataReader.GetValue(int)"/>.</remarks>
    /// <typeparam name="T">The type to which the column value should be converted.</typeparam>
    /// <param name="reader">The <see cref="MySqlDataReader"/> instance from which to retrieve the column value.</param>
    /// <param name="column_name">The name of the column whose value is to be retrieved.</param>
    /// <param name="default_value">The default value to return if the column value is <see langword="null"/> or cannot be converted. Defaults to
    /// <c>default</c> for the specified type.</param>
    /// <returns>The value of the specified column converted to the specified type <typeparamref name="T"/>. If the column value
    /// is <see langword="null"/>, or if the conversion is not supported, the <paramref name="default_value"/> is
    /// returned.</returns>
    public static T getSafeValue<T>(this MySqlDataReader reader, string column_name, T default_value = default!) {
        var ordinal = reader.GetOrdinal(column_name);
        if (reader.IsDBNull(ordinal)) {
            return default_value;
        }

        // Special handling for different types
        if (typeof(T) == typeof(string))
            return (T)(object)reader.GetString(ordinal);
        if (typeof(T) == typeof(int))
            return (T)(object)reader.GetInt32(ordinal);
        if (typeof(T) == typeof(DateOnly))
            return (T)(object)reader.GetDateOnly(ordinal);
        if (typeof(T) == typeof(double))
            return (T)(object)reader.GetDouble(ordinal);
        if (typeof(T) == typeof(decimal))
            return (T)(object)reader.GetDecimal(ordinal);

        return (T)reader.GetValue(ordinal);
    }
}
