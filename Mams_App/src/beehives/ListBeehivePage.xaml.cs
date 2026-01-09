using System.Windows.Controls;

namespace Mams_App.src.beehives;

/// <summary>
/// Interaction logic for ListBeehivePage.xaml
/// </summary>
public partial class ListBeehivePage : Page
{
    public ListBeehivePage()
    {
        InitializeComponent();
        DataContext = new ListBeehiveController();
    }
}
