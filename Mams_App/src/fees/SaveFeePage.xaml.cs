using System.Windows.Controls;

namespace Mams.src.fees; 
/// <summary>
/// Interaction logic for SaveFeePage.xaml
/// </summary>
public partial class SaveFeePage : Page {
    public SaveFeePage(int id_to_load = 0) {
        InitializeComponent();
        DataContext = new SaveFeeController(id_to_load);
    }
}
