using System.Windows.Controls;

namespace Mams_App.src.productsLots;

/// <summary>
/// Interaction logic for SaveProductLotPage.xaml
/// </summary>
public partial class SaveProductLotPage : Page
{

    public SaveProductLotPage(int id_to_load = 0)
    {

        InitializeComponent();
        DataContext = new SaveProductLotController(id_to_load);
    }

    private void UCLabelTextBox_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {

    }
}
