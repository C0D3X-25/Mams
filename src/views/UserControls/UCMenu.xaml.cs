using Mams.src.navigations;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.views.userControls;

/// <summary>
/// Interaction logic for UCMenu.xaml
/// </summary>
public partial class UCMenu : UserControl {

    public UCMenu() {
        InitializeComponent();
        this.Loaded += UCMenu_Loaded; // need to wait for the window to be loaded
    }

    private void UCMenu_Loaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            UCMenuController controller = new(_m_window.m_page_navigation);
            this.DataContext = controller;
        }
    }
}
