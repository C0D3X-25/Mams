using System.Windows.Controls;

namespace Mams_App.src.productsTypes;

/// <summary>
/// Interaction logic for ListProductTypePage.xaml
/// </summary>
public partial class ListProductTypePage : Page {
    public ListProductTypePage() {
        InitializeComponent();
        DataContext = new ListProductTypeController();
    }
}
