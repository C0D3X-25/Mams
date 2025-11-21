using Mams.src.navigations;
using Mams.src.search;
using System.Windows;

namespace Mams.src.views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();
        SPageNavigationController.initialize(MainFrame);
        SSearchModel.initialize();
    }
}
