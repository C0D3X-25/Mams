using System.Windows.Controls;

namespace Mams.src.resumes;

/// <summary>
/// Interaction logic for ResumePage.xaml
/// </summary>
public partial class ResumePage : Page {
    public ResumePage() {
        InitializeComponent();
        DataContext = new ResumeController();
    }
}
