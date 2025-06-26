using Mams.src.navigations;
using System.Windows.Controls;

namespace Mams.src.views.UserControls
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
