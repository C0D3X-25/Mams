using System.Windows.Controls;

namespace Mams.src.profits;

/// <summary>
/// Interaction logic for SaveProfitPage.xaml
/// </summary>
public partial class SaveProfitPage : Page {
    public SaveProfitPage(int id_to_load = 0) {
        InitializeComponent();
        DataContext = new SaveProfitController(id_to_load);
    }
}
