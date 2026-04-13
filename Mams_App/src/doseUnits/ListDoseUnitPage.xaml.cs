using System.Windows.Controls;

namespace Mams_App.src.doseUnits;

/// <summary>
/// Interaction logic for ListDoseUnitPage.xaml
/// </summary>
public partial class ListDoseUnitPage : Page
{
    public ListDoseUnitPage()
    {
        InitializeComponent();
        DataContext = new ListDoseUnitController();
    }
}
