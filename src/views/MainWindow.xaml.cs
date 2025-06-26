using Mams.src.navigations;
using Mams.src.views.globalView;
using System.Windows;

namespace Mams.src.views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();
        SPageNavigationController.initialize(MainFrame);
    }
}
