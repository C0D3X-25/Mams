using Mams_App.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams_App.src.products;

/// <summary>
/// Interaction logic for SaveProductPage.xaml
/// </summary>
public partial class SaveProductPage : Page {

    public SaveProductPage(int id_to_load = 0) {

        InitializeComponent();
        DataContext = new SaveProductController(id_to_load);
    }
}
