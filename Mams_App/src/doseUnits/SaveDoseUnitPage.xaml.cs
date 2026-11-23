using System.Windows.Controls;

namespace Mams_App.src.doseUnits;

/// <summary>
/// Interaction logic for SaveDoseUnitPage.xaml
/// </summary>
public partial class SaveDoseUnitPage : Page
{

    public SaveDoseUnitPage(int id_to_load = 0)
    {

        InitializeComponent();
        DataContext = new SaveDoseUnitController(id_to_load);
    }
}
