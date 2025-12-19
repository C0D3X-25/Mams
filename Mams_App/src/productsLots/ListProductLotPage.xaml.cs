using System.Windows.Controls;

namespace Mams.src.productsLots;

/// <summary>
/// Interaction logic for ListLotPage.xaml
/// </summary>
public partial class ListProductLotPage : Page {
    public ListProductLotPage() {
        InitializeComponent();
        DataContext = new ListProductLotController();
    }
}
