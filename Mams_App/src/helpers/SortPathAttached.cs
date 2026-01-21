using System.Windows;
using System.Windows.Controls;

namespace Mams_App.src.helpers;

/// <summary>
/// Provides an attached property to specify a custom sort path for GridViewColumn.
/// This is used when the sort path differs from the DisplayMemberBinding path (e.g., for date columns).
/// </summary>
public static class SortPathAttached
{
    public static readonly DependencyProperty SortPathProperty =
        DependencyProperty.RegisterAttached(
            "SortPath",
            typeof(string),
            typeof(SortPathAttached),
            new PropertyMetadata(null));

    public static string GetSortPath(DependencyObject obj)
    {
        return (string)obj.GetValue(SortPathProperty);
    }

    public static void SetSortPath(DependencyObject obj, string value)
    {
        obj.SetValue(SortPathProperty, value);
    }
}
