using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.beehives;

/// <summary>
/// Interaction logic for ListBeehivePage.xaml
/// </summary>
public partial class ListBeehivePage : Page
{
    public ListBeehivePage() { 
        InitializeComponent();
        Loaded += pageLoaded; // need to wait for the window to be loaded
    }


private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            ListBeehiveController controller = new(_m_window.m_page_navigation);
            DataContext = controller;
        }
    }
}
