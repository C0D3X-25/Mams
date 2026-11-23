using System.Windows.Controls;

namespace Mams_App.src.treatmentStocks;

/// <summary>
/// Interaction logic for ListTreatmentStockPage.xaml
/// </summary>
public partial class ListTreatmentStockPage : Page
{
    public ListTreatmentStockPage()
    {
        InitializeComponent();
        DataContext = new ListTreatmentStockController();
    }
}
