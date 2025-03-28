using Mams.src.controllers;
using Mams.src.models;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.views.pages;

/// <summary>
/// Interaction logic for ListClientPage.xaml
/// </summary>
public partial class ListClientPage : Page {
    public ListClientPage() {
        InitializeComponent();
        this.Loaded += pageLoaded; // need to wait for the window to be loaded
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            ListClientPageController controller = new(_m_window.m_page_navigation);
            this.DataContext = controller;
        }
    }
}
