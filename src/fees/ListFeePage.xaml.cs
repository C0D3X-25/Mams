using System.Windows.Controls;

namespace Mams.src.fees {
    /// <summary>
    /// Interaction logic for ListFeePage.xaml
    /// </summary>
    public partial class ListFeePage : Page {
        public ListFeePage() {
            InitializeComponent();
            DataContext = new ListFeeController();
        }
    }
}
