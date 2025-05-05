using System.Windows.Controls;

namespace Mams.src.clients;

/// <summary>
/// Interaction logic for ListClientPage.xaml
/// </summary>
public partial class ListClientPage : Page {
    public ListClientPage() {
        InitializeComponent();
        DataContext = new ListClientController();
    }
}
