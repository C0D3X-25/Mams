using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.productsCategories;

/// <summary>
/// Interaction logic for SaveProductCategoryPage.xaml
/// </summary>
public partial class SaveProductCategoryPage : Page {

    private readonly int _m_id_to_load = 0;

    public SaveProductCategoryPage(int id_to_load = 0) {

        InitializeComponent();
        Loaded += pageLoaded;
        _m_id_to_load = id_to_load;
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            SaveProductCategoryController controller = new(_m_window.m_page_navigation, _m_id_to_load);
            DataContext = controller;
        }
    }
}
