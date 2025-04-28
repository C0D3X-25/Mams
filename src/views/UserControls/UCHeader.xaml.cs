using System.Windows;
using System.Windows.Controls;
using Mams.src.navigations;

namespace Mams.src.views.userControls;

/// <summary>
/// Interaction logic for UCHeader.xaml
/// </summary>
public partial class UCHeader : UserControl {

    public string title {
        get { return (string)GetValue(titleProperty); }
        set { SetValue(titleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for title.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty titleProperty =
        DependencyProperty.Register("title", typeof(string), typeof(UCHeader), new PropertyMetadata("Page Title"));

    public UCHeader() {
        InitializeComponent();
        this.Loaded += UCHeader_Loaded; // need to wait for the window to be loaded
    }

    private void UCHeader_Loaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            UCHeaderController controller = new(_m_window.m_page_navigation);
            this.DataContext = controller;
        }
    }
}
