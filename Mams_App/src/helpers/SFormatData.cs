using Mams_App.src.globals;

namespace Mams_App.src.helpers;


public static class SFormatData
{

    /// <summary>
    /// Converts a date string in the European date format to the MySQL date format.
    /// </summary>
    /// <param name="date">The date string in the European format (e.g., "dd/MM/yyyy"). Cannot be null or empty.</param>
    /// <returns>A string representing the date in the MySQL format ("yyyy-MM-dd").  Returns an empty string if the input
    /// <paramref name="date"/> is null or empty.</returns>
    public static string formatEUDateToMySQLDate(string date)
    {

        if (string.IsNullOrEmpty(date))
        {
            return string.Empty;
        }

        DateTime parsed_date = DateTime.ParseExact(date, SGlobals.g_EU_DATE_FORMAT, null);
        return parsed_date.ToString("yyyy-MM-dd");
    }

    /// <summary>
    /// Extracts the year from a given date string.
    /// </summary>
    /// <param name="date">A string representing the date. The string can either be a 4-digit year or a date formatted according to  the
    /// format specified in <see cref="globals.SGlobals.g_EU_DATE_FORMAT"/>.</param>
    /// <returns>A string containing the 4-digit year extracted from the input. Returns an empty string if the input is empty.</returns>
    public static string getYearFromDate(string date)
    {

        if (date == string.Empty)
        {
            return string.Empty;
        }
        // If the date is already a year (4 digits)
        if (date.Length == 4)
        {
            return date;
        }

        DateTime parsed_date = DateTime.ParseExact(date,
            SGlobals.g_EU_DATE_FORMAT,
            System.Globalization.CultureInfo.InvariantCulture
        );

        return parsed_date.Year.ToString();
    }
}
