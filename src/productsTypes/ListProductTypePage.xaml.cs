using System.Windows.Controls;

namespace Mams.src.productsTypes;

/// <summary>
/// Interaction logic for ListProductTypePage.xaml
/// </summary>
public partial class ListProductTypePage : Page {
    public ListProductTypePage() {
        InitializeComponent();
        DataContext = new ListProductTypeController();
    }
}
