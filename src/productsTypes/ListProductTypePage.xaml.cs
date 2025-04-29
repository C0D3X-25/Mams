using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.productsTypes;

/// <summary>
/// Interaction logic for ListProductTypePage.xaml
/// </summary>
public partial class ListProductTypePage : Page {
    public ListProductTypePage() {
        InitializeComponent();
        Loaded += pageLoaded; // need to wait for the window to be loaded
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            ListProductTypeController controller = new(_m_window.m_page_navigation);
            DataContext = controller;
        }
    }
}
