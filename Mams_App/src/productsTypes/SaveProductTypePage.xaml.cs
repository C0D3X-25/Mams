using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.productsTypes; 
/// <summary>
/// Interaction logic for SaveProductTypePage.xaml
/// </summary>
public partial class SaveProductTypePage : Page {

    public SaveProductTypePage(int id_to_load = 0) {

        InitializeComponent();
        DataContext = new SaveProductTypeController(id_to_load);
    }
}
