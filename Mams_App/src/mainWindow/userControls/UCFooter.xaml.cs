using Mams_App.src.navigations;
using System.Windows.Controls;

namespace Mams_App.src.mainWindow.userControls
{
    /// <summary>
    /// Interaction logic for UCFooter.xaml
    /// </summary>
    public partial class UCFooter : UserControl
    {
        public UCFooter()
        {
            InitializeComponent();
            DataContext = new UCFooterController();
        }
    }
}
