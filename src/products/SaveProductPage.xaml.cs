using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.products;

/// <summary>
/// Interaction logic for SaveProductPage.xaml
/// </summary>
public partial class SaveProductPage : Page {

    private readonly int _m_id_product = 0;
    public SaveProductPage(int id_product = 0) {

        InitializeComponent();
        Loaded += pageLoaded;
        _m_id_product = id_product;
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            SaveProductController controller = new(_m_window.m_page_navigation, _m_id_product);
            DataContext = controller;
        }
    }
}
