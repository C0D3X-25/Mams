using Mams.src.models;
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

namespace Mams.src.views.userControls;

/// <summary>
/// Interaction logic for UCHeader.xaml
/// </summary>
public partial class UCHeader : UserControl {

    private readonly ABaseSearchModel _m_model = new InventoryModel();
    private readonly SearchBarController? _m_search_bar;



    public string title {
        get { return (string)GetValue(titleProperty); }
        set { SetValue(titleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for title.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty titleProperty =
        DependencyProperty.Register("title", typeof(string), typeof(UCHeader), new PropertyMetadata("Page Title"));



    public UCHeader() {
        InitializeComponent();
        _m_search_bar = new SearchBarController(_m_model, this);
    }

    private void searchbar_txtBox_TextChanged(object sender, TextChangedEventArgs e) {
        _m_search_bar?.txtBox_TextChanged(sender, e);
    }

    private void searchbar_listView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        _m_search_bar?.listView_SelectionChanged(sender, e);
        //_m_search_bar?.clearSearchBar(); // Uncomment later !
    }
}
