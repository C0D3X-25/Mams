using Mams_App.src.globals;

namespace Mams_App.src.helpers;

/// <summary>
/// Provides utility methods for validating data information.
/// </summary>
public static class SDataValidation
{

    /// <summary>
    /// Determines whether the specified string represents a valid identifier for insert or update operations.
    /// </summary>
    /// <remarks>A valid identifier is a non-null, non-empty string that represents a non-negative integer.
    /// Zero is allowed to indicate a new record insertion.
    /// This method returns <see langword="false"/> if the string is null, empty, not an integer, or represents a
    /// negative value.</remarks>
    /// <param name="id">The string to evaluate as an identifier.</param>
    /// <returns><see langword="true"/> if the string is a valid non-negative integer identifier; otherwise, <see
    /// langword="false"/>. </returns>
    public static bool isIdValid(string id)
    {

        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        if (!isInteger(id))
        {
            return false;
        }
        if (Convert.ToInt64(id) < 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Determines whether the specified string represents a valid positive identifier for retrieval operations.
    /// </summary>
    /// <remarks>A valid identifier for retrieval must be a positive integer (greater than zero).
    /// This method returns <see langword="false"/> if the string is null, empty, not an integer, zero, or negative.</remarks>
    /// <param name="id">The string to evaluate as an identifier.</param>
    /// <returns><see langword="true"/> if the string is a valid positive integer identifier; otherwise, <see
    /// langword="false"/>. </returns>
    public static bool isIdValidForRetrieval(string id)
    {

        if (string.IsNullOrEmpty(id))
        {
            return false;
        }
        if (!isInteger(id))
        {
            return false;
        }
        if (Convert.ToInt64(id) <= 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Determines whether the specified integer represents a valid identifier for insert or update operations.
    /// </summary>
    /// <remarks>A valid identifier is a non-null, non-negative integer.
    /// Zero is allowed to indicate a new record insertion.
    /// This method returns <see langword="false"/> if the integer is null or negative.</remarks>
    /// <param name="id">The integer to evaluate as an identifier.</param>
    /// <returns><see langword="true"/> if the integer is a valid non-negative identifier; otherwise, <see
    /// langword="false"/>. </returns>
    public static bool isIdValid(int? id)
    {

        if (id == null)
        {
            return false;
        }
        if (id < 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Determines whether the specified integer represents a valid positive identifier for retrieval operations.
    /// </summary>
    /// <remarks>A valid identifier for retrieval must be a positive integer (greater than zero).
    /// This method returns <see langword="false"/> if the integer is null, zero, or negative.</remarks>
    /// <param name="id">The integer to evaluate as an identifier.</param>
    /// <returns><see langword="true"/> if the integer is a valid positive identifier; otherwise, <see
    /// langword="false"/>. </returns>
    public static bool isIdValidForRetrieval(int? id)
    {

        if (id == null)
        {
            return false;
        }
        if (id <= 0)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Determines whether the specified string represents a valid integer value.
    /// </summary>
    /// <remarks>This method checks if the input string can be parsed as a 64-bit signed integer  (<see
    /// cref="System.Int64"/>). It returns <see langword="false"/> for null, empty, or  non-numeric strings.</remarks>
    /// <param name="value">The string to evaluate.</param>
    /// <returns><see langword="true"/> if the string can be successfully converted to an integer;  otherwise, <see
    /// langword="false"/>. </returns>
    public static bool isInteger(string value)
    {
        try
        {
            Convert.ToInt64(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the specified string represents a positive integer.
    /// </summary>
    /// <remarks>A positive integer is defined as a non-negative whole number greater than zero. This method
    /// returns <see langword="false"/> for null, empty, or non-numeric strings.</remarks>
    /// <param name="value">The string to evaluate.</param>
    /// <returns><see langword="true"/> if the string represents a positive integer;  otherwise, <see langword="false"/>. </returns>
    public static bool isPositiveInteger(string value)
    {
        try
        {
            Convert.ToUInt64(value);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Determines whether a given year is within a specified range.
    /// </summary>
    /// <param name="year">The year to validate minimum is 1900.</param>
    /// <param name="min">The minimum year in the range. Defaults to 1900 if not specified.</param>
    /// <param name="max">The maximum year in the range. Defaults to the current year plus 10 if not specified.</param>
    /// <returns>
    /// <see langword="true"/> if the year is within the range; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool isYearInRange(int year, int min = 0, int max = 0)
    {
        if (year < 1900
            || min < 0
            || max < 0)
        {
            return false;
        }

        if (min == 0)
        {
            min = 1900;
        }
        if (max == 0)
        {
            max = DateTime.Now.Year + 10;
        }
        return year >= min && year <= max;
    }


    ///<summary>
    /// Validates whether a given string represents a valid date in the format 'dd/MM/yyyy' .
    /// </summary>
    /// <param name="date">The date string to validate.</param>
    /// <returns>
    /// <see langword="true"/> if the date string is valid and matches the required format; otherwise, <see langword="false"/>.
    /// Returns <see langword="false"/> if the input is null, empty, or whitespace.
    /// </returns>
    public static bool isDateValidFormatEU(string date)
    {

        if (string.IsNullOrWhiteSpace(date))
        {
            return false;
        }

        bool valid = DateTime.TryParseExact(
            date,
            SGlobals.g_EU_DATE_FORMAT,
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out DateTime parsedDate
        );

        return valid;
    }
}
