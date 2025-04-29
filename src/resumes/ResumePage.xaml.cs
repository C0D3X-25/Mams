using Mams.src.productsLots;
using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.resumes;

/// <summary>
/// Interaction logic for ResumePage.xaml
/// </summary>
public partial class ResumePage : Page {
    public ResumePage() {

        InitializeComponent();
        Loaded += pageLoaded; // need to wait for the window to be loaded
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            ResumeController controller = new(_m_window.m_page_navigation);
            DataContext = controller;
        }
    }
}
