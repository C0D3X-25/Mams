using System.Windows.Controls;

namespace Mams_App.src.treatmentStocks;

/// <summary>
/// Interaction logic for SaveTreatmentStockPage.xaml
/// </summary>
public partial class SaveTreatmentStockPage : Page
{
    public SaveTreatmentStockPage(int id_to_load = 0)
    {
        InitializeComponent();
        DataContext = new SaveTreatmentStockController(id_to_load);
    }
}
