using System.Windows;
using System.Windows.Controls;

namespace Mams_App.src.helpers;

/// <summary>
/// Attached behavior that automatically focuses a TextBox when it loads
/// and places the cursor at the end of any existing text.
/// </summary>
public static class AutoFocusBehavior
{
    /// <summary>
    /// Identifies the AutoFocus attached property.
    /// </summary>
    public static readonly DependencyProperty AutoFocusProperty =
        DependencyProperty.RegisterAttached(
            "AutoFocus",
            typeof(bool),
            typeof(AutoFocusBehavior),
            new PropertyMetadata(false, OnAutoFocusChanged));

    /// <summary>
    /// Gets the AutoFocus value for the specified element.
    /// </summary>
    public static bool GetAutoFocus(DependencyObject obj)
    {
        return (bool)obj.GetValue(AutoFocusProperty);
    }

    /// <summary>
    /// Sets the AutoFocus value for the specified element.
    /// </summary>
    public static void SetAutoFocus(DependencyObject obj, bool value)
    {
        obj.SetValue(AutoFocusProperty, value);
    }

    private static void OnAutoFocusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TextBox textBox && (bool)e.NewValue)
        {
            textBox.Loaded += TextBox_Loaded;
        }
    }

    private static void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.Loaded -= TextBox_Loaded;
            textBox.Focus();
            textBox.CaretIndex = textBox.Text?.Length ?? 0;
        }
    }
}
