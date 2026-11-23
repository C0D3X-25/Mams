using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.commands;

/// <summary>
/// Provides attached properties for binding mouse-related commands to WPF controls.
/// </summary>
public static class MouseCommand
{

    /// <summary>
    /// Identifies the MouseDoubleClickCommand attached dependency property.
    /// </summary>
    /// <remarks>
    /// This property allows binding of commands to the MouseDoubleClick event on UI elements.
    /// </remarks>
    public static readonly DependencyProperty MouseDoubleClickCommandProperty =
        DependencyProperty.RegisterAttached(
            "MouseDoubleClickCommand",
            typeof(ICommand),
            typeof(MouseCommand),
            new PropertyMetadata(null, OnMouseDoubleClickCommandChanged)
        );

    /// <summary>
    /// Gets the value of the MouseDoubleClickCommand attached property from the specified object.
    /// </summary>
    /// <param name="obj">The dependency object from which to retrieve the command.</param>
    /// <returns>The ICommand associated with the MouseDoubleClick event.</returns>
    public static ICommand GetMouseDoubleClickCommand(DependencyObject obj)
    {
        return (ICommand)obj.GetValue(MouseDoubleClickCommandProperty);
    }

    /// <summary>
    /// Sets the value of the MouseDoubleClickCommand attached property on the specified object.
    /// </summary>
    /// <param name="obj">The dependency object on which to set the command.</param>
    /// <param name="value">The ICommand to execute when the MouseDoubleClick event occurs.</param>
    public static void SetMouseDoubleClickCommand(DependencyObject obj, ICommand value)
    {
        obj.SetValue(MouseDoubleClickCommandProperty, value);
    }

    private static void OnMouseDoubleClickCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Control element)
        {
            if (e.OldValue != null)
            {
                element.MouseDoubleClick -= Element_MouseDoubleClick;
            }
            if (e.NewValue != null)
            {
                element.MouseDoubleClick += Element_MouseDoubleClick;
            }
        }
    }

    private static void Element_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not UIElement element)
        {
            return;
        }

        if (e.OriginalSource is not DependencyObject source)
        {
            return;
        }

        // Check if the click originated from a GridViewColumnHeader
        if (IsClickOnHeader(source))
        {
            return;
        }

        // Check if the click originated from an actual ListViewItem
        if (!IsClickOnItem(source))
        {
            return;
        }

        var command = GetMouseDoubleClickCommand(element);

        if (command != null && command.CanExecute(null))
        {
            command.Execute(null);
        }
    }

    /// <summary>
    /// Determines whether the click originated from within a ListViewItem.
    /// </summary>
    private static bool IsClickOnItem(DependencyObject source)
    {
        // Handle non-visual elements like Run (from TextBlock.Inlines)
        while (source is FrameworkContentElement contentElement)
        {
            source = contentElement.Parent;
            if (source == null)
            {
                return false;
            }
        }

        // Traverse the visual tree to find a ListViewItem
        while (source != null)
        {
            if (source is ListViewItem)
            {
                return true;
            }
            source = VisualTreeHelper.GetParent(source);
        }
        return false;
    }

    /// <summary>
    /// Determines whether the click originated from a GridViewColumnHeader.
    /// </summary>
    private static bool IsClickOnHeader(DependencyObject source)
    {
        // Handle non-visual elements like Run (from TextBlock.Inlines)
        // by first navigating to their visual parent via the logical tree
        while (source is FrameworkContentElement contentElement)
        {
            source = contentElement.Parent;
            if (source == null)
            {
                return false;
            }
        }

        // Now traverse the visual tree
        while (source != null)
        {
            if (source is GridViewColumnHeader)
            {
                return true;
            }
            source = VisualTreeHelper.GetParent(source);
        }
        return false;
    }
}