using Mams.src.profits;
using System.Windows.Controls;
using System.Windows.Input;

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
