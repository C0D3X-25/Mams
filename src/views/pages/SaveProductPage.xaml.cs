using Mams.src.controllers;
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

namespace Mams.src.views.pages;

/// <summary>
/// Interaction logic for SaveProductPage.xaml
/// </summary>
public partial class SaveProductPage : Page {

    private readonly int _m_id_product = 0;
    public SaveProductPage(int id_product = 0) {

        InitializeComponent();
        this.Loaded += pageLoaded;
        this._m_id_product = id_product;
    }


    private void pageLoaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            SaveProductController controller = new(_m_window.m_page_navigation, _m_id_product);
            this.DataContext = controller;
        }
    }
}
