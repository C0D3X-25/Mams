using System.Windows.Controls;

namespace Mams_App.src.profits
{
    /// <summary>
    /// Interaction logic for ListProfitPage.xaml
    /// </summary>
    public partial class ListProfitPage : Page
    {
        public ListProfitPage()
        {
            InitializeComponent();
            DataContext = new ListProfitController();
        }
    }
}
