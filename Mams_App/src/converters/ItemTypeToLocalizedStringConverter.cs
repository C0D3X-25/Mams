using Mams_App.src.localizations;
using System.Globalization;
using System.Windows.Data;

namespace Mams_App.src.converters;

/// <summary>
/// Converter that translates item type codes to localized display strings.
/// Used in the search results to display user-friendly type names.
/// </summary>
public class ItemTypeToLocalizedStringConverter : IValueConverter
{
    /// <summary>
    /// Converts an item type code to its localized string representation.
    /// </summary>
    /// <param name="value">The item type code (e.g., "Product", "Entity", "Fee").</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="culture">The culture info (not used).</param>
    /// <returns>The localized string for the item type, or the original value if no translation is found.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string itemType)
        {
            return value;
        }

        // Build the localization key from the item type
        string locKey = $"ItemType.{itemType}";
        string localized = Loc.Get(locKey);

        // If the localization returns the key in brackets, return the original value
        if (localized.StartsWith('[') && localized.EndsWith(']'))
        {
            return itemType;
        }

        return localized;
    }

    /// <summary>
    /// Not implemented - this is a one-way converter.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
