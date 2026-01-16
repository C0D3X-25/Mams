using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Mams_App.src.converters;

/// <summary>
/// Converts a boolean value to a GridViewColumn width.
/// Returns Auto width when true, 0 when false (hidden).
/// </summary>
public class BooleanToColumnWidthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isVisible && isVisible)
        {
            return double.NaN; // Auto width
        }
        return 0d; // Hidden
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
