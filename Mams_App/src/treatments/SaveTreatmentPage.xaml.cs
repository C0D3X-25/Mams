using System.Windows.Controls;

namespace Mams_App.src.treatments;

/// <summary>
/// Interaction logic for SaveTreatmentPage.xaml
/// </summary>
public partial class SaveTreatmentPage : Page
{
    public SaveTreatmentPage(int id_to_load = 0)
    {
        InitializeComponent();
        DataContext = new SaveTreatmentController(id_to_load);
    }
}
