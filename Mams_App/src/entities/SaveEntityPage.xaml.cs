using System.Windows.Controls;

namespace Mams_App.src.entities;

/// <summary>
/// Interaction logic for SaveClientsPage.xaml
/// </summary>
public partial class SaveEntityPage : Page
{

    public SaveEntityPage(int id_client = 0)
    {
        InitializeComponent();
        DataContext = new SaveEntityController(id_client);
    }
}
