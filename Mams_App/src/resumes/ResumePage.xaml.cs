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
}
