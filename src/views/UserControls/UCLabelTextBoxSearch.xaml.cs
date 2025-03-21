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
/// Interaction logic for UCLabelTextBoxSearch.xaml
/// </summary>
public partial class UCLabelTextBoxSearch : UserControl {

    private readonly ABaseSearchModel _m_model = new InventoryModel();
    private readonly SearchBarModel? _m_search_bar;

    public string label {
        get { return (string)GetValue(labelProperty); }
        set { SetValue(labelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for label.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty labelProperty =
        DependencyProperty.Register("label", typeof(string), typeof(UCLabelTextBoxSearch), new PropertyMetadata("Label to change text"));



    public UCLabelTextBoxSearch() {
        InitializeComponent();
        _m_search_bar = new SearchBarModel(_m_model, this);

    }

    private void textBoxSearch_txtBox_TextChanged(object sender, TextChangedEventArgs e) {
        _m_search_bar?.txtBox_TextChanged(sender, e);
    }

    private void searchbar_listView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        _m_search_bar?.listView_SelectionChanged(sender, e);
    }

}
