using Mams_App.src.items;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mams_App.src.receipts;

public class ReceiptItem : ABaseItem, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public int receipt_id { get; set; } = 0;
    public string receipt_number { get; set; } = string.Empty;
    public decimal receipt_total_price { get; set; } = 0.0M;

    /// <summary>
    /// The date value used for sorting. Set this first, then receipt_date_created will be auto-formatted.
    /// </summary>
    private DateOnly _receipt_date_created_value = DateOnly.FromDateTime(DateTime.Today);

    public DateOnly receipt_date_created_value
    {
        get => _receipt_date_created_value;
        set
        {
            if (_receipt_date_created_value != value)
            {
                _receipt_date_created_value = value;
                onPropertyChanged();
                onPropertyChanged(nameof(receipt_date_created));
            }
        }
    }

    /// <summary>
    /// The formatted date string for display (dd.MM.yyyy format).
    /// User can type any string, but only valid dates will update the underlying value.
    /// </summary>
    private string _receipt_date_created_display = DateOnly.FromDateTime(DateTime.Today).ToString(globals.SGlobals.g_EU_DATE_FORMAT);

    public string receipt_date_created
    {
        get => _receipt_date_created_display;
        set
        {
            if (_receipt_date_created_display != value)
            {
                _receipt_date_created_display = value;
                onPropertyChanged();

                // Try to parse the input and update the underlying date value if valid
                if (DateOnly.TryParseExact(value, globals.SGlobals.g_EU_DATE_FORMAT, out var parsed))
                {
                    _receipt_date_created_value = parsed;
                }
            }
        }
    }

    /// <summary>
    /// Raises the PropertyChanged event to notify the UI of property value changes.
    /// </summary>
    /// <param name="property_name">The name of the property that changed. Automatically populated by the compiler.</param>
    protected void onPropertyChanged([CallerMemberName] string? property_name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property_name));
    }
}