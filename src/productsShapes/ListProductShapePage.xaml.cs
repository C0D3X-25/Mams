using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.productsShapes;

/// <summary>
/// Interaction logic for ListProductShapePage.xaml
/// </summary>
public partial class ListProductShapePage : Page
{
    public ListProductShapePage() {
        InitializeComponent();
        Loaded += pageLoaded; // need to wait for the window to be loaded
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            ListProductShapeController controller = new(_m_window.m_page_navigation);
            DataContext = controller;
        }
    }
}
