using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Mams_App.src.converters;

/// <summary>
/// Multi-value converter that returns an arrow symbol based on sort state.
/// Values[0]: current column name (string)
/// Values[1]: sorted column name (string)
/// Values[2]: sort direction (ListSortDirection)
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

        var currentColumn = values[0] as string;
        var sortedColumn = values[1] as string;
        var sortDirection = (ListSortDirection)values[2];

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
