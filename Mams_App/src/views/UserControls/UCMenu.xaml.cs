using Mams_App.src.navigations;
using System.Windows;
using System.Windows.Controls;

namespace Mams_App.src.views.UserControls;

/// <summary>
/// Interaction logic for UCMenu.xaml
/// </summary>
public partial class UCMenu : UserControl {

    public UCMenu() {
        InitializeComponent();
        DataContext = new UCMenuController();
    }
}
