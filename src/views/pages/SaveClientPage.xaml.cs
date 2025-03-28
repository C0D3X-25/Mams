using Mams.src.controllers;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.views.pages;

/// <summary>
/// Interaction logic for SaveClientsPage.xaml
/// </summary>
public partial class SaveClientPage : Page {

    public SaveClientPage() {
        InitializeComponent();
        this.Loaded += pageLoaded; // need to wait for the window to be loaded
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            SaveClientController controller = new(_m_window.m_page_navigation);
            this.DataContext = controller;
        }
    }
}
