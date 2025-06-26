using Mams.src.navigations;
using System.Windows;

namespace Mams.src.views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

    public MainWindow() {
        //StartApplication.launchServices();
        InitializeComponent();

        SPageNavigationController.initialize(MainFrame);
    }
}
