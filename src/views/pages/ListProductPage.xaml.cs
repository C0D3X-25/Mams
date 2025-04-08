using Mams.src.controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mams.src.views.pages;

/// <summary>
/// Interaction logic for ListProductPage.xaml
/// </summary>
public partial class ListProductPage : Page {
    public ListProductPage() {
        InitializeComponent();
        this.Loaded += pageLoaded; // need to wait for the window to be loaded
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            ListProductPageController controller = new(_m_window.m_page_navigation);
            this.DataContext = controller;
        }
    }
}
