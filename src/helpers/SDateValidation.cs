using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.helpers;

/// <summary>
/// Provides utility methods for validating date-related information.
/// </summary>
public static class SDateValidation {

    /// <summary>
    /// Determines whether a given year is within a specified range.
    /// </summary>
    /// <param name="year">The year to validate.</param>
    /// <param name="min">The minimum year in the range. Defaults to 1900 if not specified.</param>
    /// <param name="max">The maximum year in the range. Defaults to the current year plus 1 if not specified.</param>
    /// <returns>
    /// <c>true</c> if the year is within the range; otherwise, <c>false</c>.
    /// </returns>
    public static bool isYearInRange(int year, int min = 0, int max = 0) {
        if (min == 0) {
            min = 1900;
        }
        if (max == 0) {
            max = DateTime.Now.Year + 1;
        }
        return year >= min && year <= max;
    }
}
