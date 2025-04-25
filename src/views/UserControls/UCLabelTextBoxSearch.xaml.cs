using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Mams.src.clients;
using Mams.src.entities;
using Mams.src.items;
using Mams.src.searchBars;

namespace Mams.src.views.userControls;

/// <summary>
/// Interaction logic for UCLabelTextBoxSearch.xaml
/// </summary>
public partial class UCLabelTextBoxSearch : UserControl {

    public UCLabelTextBoxSearch() {
        InitializeComponent();
    }


    public string label {
        get { return (string)GetValue(labelProperty); }
        set { SetValue(labelProperty, value); }
    }
    public static readonly DependencyProperty labelProperty =
        DependencyProperty.Register("label", typeof(string), typeof(UCLabelTextBoxSearch),
            new PropertyMetadata("use 'label' to change text"));



    public list<string> list_items {
        get { return (list<string>)GetValue(list_itemsProperty); }
        set { SetValue(list_itemsProperty, value); }
    }
    public static readonly DependencyProperty list_itemsProperty =
        DependencyProperty.Register("list_items", typeof(list<string>), typeof(UCLabelTextBoxSearch));



    public string selected_item {
        get { return (string)GetValue(selected_itemProperty); }
        set { SetValue(selected_itemProperty, value); }
    }
    public static readonly DependencyProperty selected_itemProperty =
        DependencyProperty.Register("selected_item", typeof(string), typeof(UCLabelTextBoxSearch));
}
