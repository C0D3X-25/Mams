using System.Windows.Controls;

namespace Mams_App.src.regions;

/// <summary>
/// Interaction logic for SaveRegionPage.xaml
/// </summary>
public partial class SaveRegionPage : Page
{

    public SaveRegionPage(int id_to_load = 0)
    {

        InitializeComponent();
        DataContext = new SaveRegionController(id_to_load);
    }
}
