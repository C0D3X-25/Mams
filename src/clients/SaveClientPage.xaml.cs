using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.clients;

/// <summary>
/// Interaction logic for SaveClientsPage.xaml
/// </summary>
public partial class SaveClientPage : Page {

    public SaveClientPage(int id_client = 0) {
        InitializeComponent();
        DataContext = new SaveClientController(id_client);
    }
}
