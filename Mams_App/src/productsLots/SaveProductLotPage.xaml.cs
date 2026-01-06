using Mams_App.src.products;
using Mams_App.src.views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mams_App.src.productsLots;

/// <summary>
/// Interaction logic for SaveProductLotPage.xaml
/// </summary>
public partial class SaveProductLotPage : Page {

    public SaveProductLotPage(int id_to_load = 0) {

        InitializeComponent();
        DataContext = new SaveProductLotController(id_to_load);
    }
}
