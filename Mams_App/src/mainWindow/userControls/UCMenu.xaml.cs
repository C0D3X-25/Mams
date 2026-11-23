using Mams_App.src.navigations;
using System.Windows.Controls;

namespace Mams_App.src.mainWindow.userControls;

/// <summary>
/// Interaction logic for UCMenu.xaml
/// </summary>
public partial class UCMenu : UserControl
{

    public UCMenu()
    {
        InitializeComponent();
        DataContext = new UCMenuController();
    }
}
