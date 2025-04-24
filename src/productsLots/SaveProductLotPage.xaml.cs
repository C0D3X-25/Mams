using Mams.src.products;
using Mams.src.views;
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

namespace Mams.src.productsLots;

/// <summary>
/// Interaction logic for SaveProductLotPage.xaml
/// </summary>
public partial class SaveProductLotPage : Page {

    private readonly int _m_id_product = 0;
    public SaveProductLotPage(int id_product = 0) {

        InitializeComponent();
        Loaded += pageLoaded;
        _m_id_product = id_product;
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            SaveProductLotController controller = new(_m_window.m_page_navigation, _m_id_product);
            DataContext = controller;
        }
    }
}
