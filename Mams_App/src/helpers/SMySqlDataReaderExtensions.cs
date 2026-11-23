using MySqlConnector;

namespace Mams_App.src.helpers;

public static class SMySqlDataReaderExtensions
{
    /// <summary>
    /// Retrieves the value of the specified column from a <see cref="MySqlDataReader"/> as the specified type. If the
    /// column value is <see langword="null"/> or cannot be converted to the specified type, a default value is
    /// returned.
    /// </summary>
    /// <remarks>This method provides special handling for common types and their nullable equivalents including
    /// <see cref="string"/>, <see cref="int"/>, <see cref="long"/>, <see cref="bool"/>, <see cref="DateOnly"/>, <see
    /// cref="DateTime"/>, <see cref="float"/>, <see cref="double"/>, and <see cref="decimal"/>. For unsupported types,
    /// the method attempts to retrieve the value using <see cref="MySqlDataReader.GetValue(int)"/>.</remarks>
    /// <typeparam name="T">The type to which the column value should be converted.</typeparam>
    /// <param name="reader">The <see cref="MySqlDataReader"/> instance from which to retrieve the column value.</param>
    /// <param name="column_name">The name of the column whose value is to be retrieved.</param>
    /// <param name="default_value">The default value to return if the column value is <see langword="null"/> or cannot be converted. Defaults to
    /// <c>default</c> for the specified type.</param>
    /// <returns>The value of the specified column converted to the specified type <typeparamref name="T"/>. If the column value
    /// is <see langword="null"/>, or if the conversion is not supported, the <paramref name="default_value"/> is
    /// returned.</returns>
    public static T getSafeValue<T>(this MySqlDataReader reader, string column_name, T default_value = default!)
    {
        var ordinal = reader.GetOrdinal(column_name);
        if (reader.IsDBNull(ordinal))
        {
            return default_value;
        }

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        object value = targetType switch
        {
            Type t when t == typeof(string) => reader.GetString(ordinal),
            Type t when t == typeof(int) => reader.GetInt32(ordinal),
            Type t when t == typeof(long) => reader.GetInt64(ordinal),
            Type t when t == typeof(bool) => reader.GetBoolean(ordinal),
            Type t when t == typeof(DateOnly) => reader.GetDateOnly(ordinal),
            Type t when t == typeof(DateTime) => reader.GetDateTime(ordinal),
            Type t when t == typeof(float) => reader.GetFloat(ordinal),
            Type t when t == typeof(double) => reader.GetDouble(ordinal),
            Type t when t == typeof(decimal) => reader.GetDecimal(ordinal),
            _ => reader.GetValue(ordinal)
        };

        return (T)value;
    }
}
