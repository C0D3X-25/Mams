using System.Windows.Controls;

namespace Mams_App.src.beehives;

/// <summary>
/// Interaction logic for SaveBeehivePage.xaml
/// </summary>
public partial class SaveBeehivePage : Page {

    public SaveBeehivePage(int id_to_load = 0) {

        InitializeComponent();
        DataContext = new SaveBeehiveController(id_to_load);
    }
}
