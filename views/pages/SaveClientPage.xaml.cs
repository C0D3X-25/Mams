using Mams.src.ctrl;
using System.Windows.Controls;

namespace Mams.views.pages;

/// <summary>
/// Interaction logic for SaveClientsPage.xaml
/// </summary>
public partial class SaveClientPage : Page {

    public SaveClientPage() {
        InitializeComponent();
        this.DataContext = new SaveClientController();
    }
}
