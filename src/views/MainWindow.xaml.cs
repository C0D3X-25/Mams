using Mams.src.controllers;
using System.Windows;

namespace Mams.src.views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

    public readonly PageNavigationController m_page_navigation;

    public MainWindow() {
        StartApplication.launchServices();
        InitializeComponent();
        m_page_navigation = new(MainFrame);
    }
}
