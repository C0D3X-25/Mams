using System.Windows.Controls;

namespace Mams.src.entities;

/// <summary>
/// Interaction logic for ListClientPage.xaml
/// </summary>
public partial class ListEntityPage : Page {
    public ListEntityPage() {
        InitializeComponent();
        DataContext = new ListEntityController();
    }
}
