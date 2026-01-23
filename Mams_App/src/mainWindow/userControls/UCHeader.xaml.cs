using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Controls;

namespace Mams_App.src.mainWindow.UserControls;

/// <summary>
/// Interaction logic for UCHeader.xaml
/// </summary>
public partial class UCHeader : UserControl
{

    public string title
    {
        get { return (string)GetValue(titleProperty); }
        set { SetValue(titleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for title.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty titleProperty =
        DependencyProperty.Register("title", typeof(string), typeof(UCHeader), new PropertyMetadata("Page Title"));

    public UCHeader()
    {
        InitializeComponent();
        DataContext = new UCHeaderController();
    }
}
