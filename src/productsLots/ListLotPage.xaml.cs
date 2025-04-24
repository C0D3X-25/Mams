using Mams.src.products;
using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.productsLots;

/// <summary>
/// Interaction logic for ListLotPage.xaml
/// </summary>
public partial class ListProductLotPage : Page {
    public ListProductLotPage() {

        InitializeComponent();
        Loaded += pageLoaded; // need to wait for the window to be loaded
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            ListLotController controller = new(_m_window.m_page_navigation);
            DataContext = controller;
        }
    }
}
