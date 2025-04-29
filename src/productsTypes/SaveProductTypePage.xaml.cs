using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.productsTypes; 
/// <summary>
/// Interaction logic for SaveProductTypePage.xaml
/// </summary>
public partial class SaveProductTypePage : Page {

    private readonly int _m_id_to_load = 0;

    public SaveProductTypePage(int id_to_load = 0) {

        InitializeComponent();
        Loaded += pageLoaded;
        _m_id_to_load = id_to_load;
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            SaveProductTypeController controller = new(_m_window.m_page_navigation, _m_id_to_load);
            DataContext = controller;
        }
    }
}
