using Mams.src.controllers;
using System.Windows.Controls;

namespace Mams.src.views.pages
{
    /// <summary>
    /// Interaction logic for ListClientPage.xaml
    /// </summary>
    public partial class ListClientPage : Page
    {
        public ListClientPage()
        {
            InitializeComponent();
            ListClientPageController controller = new();
            this.DataContext = controller;
        }
    }
}
