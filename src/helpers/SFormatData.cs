
namespace Mams.src.helpers; 


public static class SFormatData {

    /// <summary>
    /// Converts a date string in the European date format to the MySQL date format.
    /// </summary>
    /// <param name="date">The date string in the European format (e.g., "dd/MM/yyyy"). Cannot be null or empty.</param>
    /// <returns>A string representing the date in the MySQL format ("yyyy-MM-dd").  Returns an empty string if the input
    /// <paramref name="date"/> is null or empty.</returns>
    public static string formatEUDateToMySQLDate(string date) {

        if (string.IsNullOrEmpty(date)) {
            return string.Empty;
        }

        DateTime parsed_date = DateTime.ParseExact(date, globals.SGlobals.g_EU_DATE_FORMAT, null);
        return parsed_date.ToString("yyyy-MM-dd");
    }
}
