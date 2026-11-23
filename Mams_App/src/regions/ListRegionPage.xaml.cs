using System.Windows.Controls;

namespace Mams_App.src.regions;

/// <summary>
/// Interaction logic for ListRegionPage.xaml
/// </summary>
public partial class ListRegionPage : Page
{
    public ListRegionPage()
    {
        InitializeComponent();
        DataContext = new ListRegionController();
    }
}
