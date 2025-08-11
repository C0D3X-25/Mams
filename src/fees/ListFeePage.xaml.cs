using Mams.src.profits;
using System.Windows.Controls;
using System.Windows.Input;

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
