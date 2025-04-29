using Mams.src.productsCategories;
using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.beehives;

/// <summary>
/// Interaction logic for SaveBeehivePage.xaml
/// </summary>
public partial class SaveBeehivePage : Page
{
    private readonly int _m_id_to_load = 0;
    public SaveBeehivePage(int id_to_load = 0) {

        InitializeComponent();
        Loaded += pageLoaded;
        _m_id_to_load = id_to_load;
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            SaveBeehiveController controller = new(_m_window.m_page_navigation, _m_id_to_load);
            DataContext = controller;
        }
    }
}
