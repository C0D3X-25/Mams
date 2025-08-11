using System.Windows.Controls;
using System.Windows.Input;

namespace Mams.src.resumes;

/// <summary>
/// Interaction logic for ResumePage.xaml
/// </summary>
public partial class ResumePage : Page {
    public ResumePage() {
        InitializeComponent();
        DataContext = new ResumeController();
    }

    private void ProfitListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        if (sender is ListView listView && listView.SelectedItem != null) {
            ((ResumeController)DataContext).navigateToProfitDetails(listView.SelectedItem);
        }
    }

    private void FeeListView_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
        if (sender is ListView listView && listView.SelectedItem != null) {
            ((ResumeController)DataContext).navigateToFeeDetails(listView.SelectedItem);
        }
    }
}
