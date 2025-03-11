using Mams.src.ctrl;
using System.Windows;

namespace Mams.views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

    public readonly PageNavigationController m_page_navigation;

    public MainWindow() {
        InitializeComponent();
        m_page_navigation = new(MainFrame);
    }
}
