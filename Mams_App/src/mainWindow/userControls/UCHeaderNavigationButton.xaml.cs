using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams_App.src.mainWindow.userControls;

/// <summary>
/// Interaction logic for UCHeaderNavigationButton.xaml
/// </summary>
public partial class UCHeaderNavigationButton : UserControl
{
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register("Command", typeof(ICommand), typeof(UCHeaderNavigationButton),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IsCurrentPageProperty =
        DependencyProperty.Register("IsCurrentPage", typeof(bool), typeof(UCHeaderNavigationButton),
            new PropertyMetadata(false));

    public static readonly DependencyProperty IconDataProperty =
        DependencyProperty.Register("IconData", typeof(Geometry), typeof(UCHeaderNavigationButton),
            new PropertyMetadata(null));

    public new static readonly DependencyProperty ToolTipProperty =
        DependencyProperty.Register("ToolTip", typeof(object), typeof(UCHeaderNavigationButton),
            new PropertyMetadata(null));

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

    public Geometry IconData
    {
        get => (Geometry)GetValue(IconDataProperty);
        set => SetValue(IconDataProperty, value);
    }

    public new object ToolTip
    {
        get => GetValue(ToolTipProperty);
        set => SetValue(ToolTipProperty, value);
    }

    public UCHeaderNavigationButton()
    {
        InitializeComponent();
    }
}
