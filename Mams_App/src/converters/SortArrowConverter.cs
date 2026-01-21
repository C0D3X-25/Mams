using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Mams_App.src.converters;

/// <summary>
/// Multi-value converter that returns an arrow symbol based on sort state.
/// Values[0]: current column display binding path (string)
/// Values[1]: sorted column name (string)
/// Values[2]: sort direction (ListSortDirection)
/// Values[3]: (optional) column Tag for custom sort path (string)
/// </summary>
public class SortArrowConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length < 3 ||
            values[0] == DependencyProperty.UnsetValue ||
            values[1] == DependencyProperty.UnsetValue ||
            values[2] == DependencyProperty.UnsetValue)
        {
            return string.Empty;
        }

        var displayPath = values[0] as string;
        var sortedColumn = values[1] as string;
        var sortDirection = (ListSortDirection)values[2];

        // Use Tag (sort path) if available, otherwise use display path
        string? currentColumn = displayPath;
        if (values.Length >= 4 && values[3] != DependencyProperty.UnsetValue && values[3] is string tag && !string.IsNullOrEmpty(tag))
        {
            currentColumn = tag;
        }

        if (string.IsNullOrEmpty(currentColumn) || currentColumn != sortedColumn)
        {
            return string.Empty;
        }

        return sortDirection == ListSortDirection.Ascending ? " \u2191" : " \u2193";
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
