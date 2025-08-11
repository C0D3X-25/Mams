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
        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (sender is ListView listView && listView.SelectedItem != null) {
                ((ListFeeController)DataContext).navigateToModifyPage(listView.SelectedItem);
            }
        }
    }
}
