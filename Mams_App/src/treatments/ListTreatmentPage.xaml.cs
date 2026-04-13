using System.Windows.Controls;

namespace Mams_App.src.treatments;

/// <summary>
/// Interaction logic for ListTreatmentPage.xaml
/// </summary>
public partial class ListTreatmentPage : Page
{
    public ListTreatmentPage()
    {
        InitializeComponent();
        DataContext = new ListTreatmentController();
    }
}
