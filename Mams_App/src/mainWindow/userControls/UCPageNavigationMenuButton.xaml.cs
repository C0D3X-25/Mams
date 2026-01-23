using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Mams_App.src.mainWindow.userControls;

/// <summary>
/// Converter that returns Collapsed when true and Visible when false.
/// </summary>
public class InverseBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return boolValue ? Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            return visibility != Visibility.Visible;
        }
        return false;
    }
}

/// <summary>
/// Interaction logic for UCPageNavigationMenuButton.xaml
/// </summary>
public partial class UCPageNavigationMenuButton : UserControl
{
    public new static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register("Content", typeof(object), typeof(UCPageNavigationMenuButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(UCPageNavigationMenuButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsCurrentPageProperty =
        DependencyProperty.Register("IsCurrentPage", typeof(bool), typeof(UCPageNavigationMenuButton),
            new PropertyMetadata(false));

    public new object Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public bool IsCurrentPage
    {
        get => (bool)GetValue(IsCurrentPageProperty);
        set => SetValue(IsCurrentPageProperty, value);
    }

    public UCPageNavigationMenuButton()
    {
        InitializeComponent();
    }
}
