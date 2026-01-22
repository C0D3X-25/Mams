using System.Windows.Controls;

namespace Mams_App.src.searches;

/// <summary>
/// Interaction logic for ListSearchPage.xaml
/// </summary>
public partial class ListSearchPage : Page
{
    public ListSearchPage()
    {
        InitializeComponent();
        DataContext = new SearchController();
    }
}
