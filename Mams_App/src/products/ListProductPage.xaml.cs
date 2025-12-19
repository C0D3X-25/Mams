using System.Windows.Controls;

namespace Mams.src.products;

/// <summary>
/// Interaction logic for ListProductPage.xaml
/// </summary>
public partial class ListProductPage : Page {
    public ListProductPage() {
        InitializeComponent();
        DataContext = new ListProductController();
    }
}
