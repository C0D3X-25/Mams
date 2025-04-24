using System.Windows;
using System.Windows.Controls;
using Mams.src.searchBars;

namespace Mams.src.views.userControls;

/// <summary>
/// Interaction logic for UCLabelTextBoxSearch.xaml
/// </summary>
public partial class UCLabelTextBoxSearch : UserControl {

    //private readonly ABaseSearchModel _m_model = new InventoryModel();
    private readonly SearchBarModel? _m_search_bar;

    public string label {
        get { return (string)GetValue(labelProperty); }
        set { SetValue(labelProperty, value); }
    }
    public static readonly DependencyProperty labelProperty =
        DependencyProperty.Register("label", typeof(string), typeof(UCLabelTextBox),
            new PropertyMetadata("label to change text"));

    public string text {
        get { return (string)GetValue(textProperty); }
        set { SetValue(textProperty, value); }
    }
    public static readonly DependencyProperty textProperty =
        DependencyProperty.Register("text", typeof(string), typeof(UCLabelTextBox),
            new FrameworkPropertyMetadata(default(string),
                FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                FrameworkPropertyMetadataOptions.Journal)
            );


    public UCLabelTextBoxSearch() {
        InitializeComponent();
        //_m_search_bar = new SearchBarModel(_m_model, this);

    }

    private void textBoxSearch_txtBox_TextChanged(object sender, TextChangedEventArgs e) {
        _m_search_bar?.txtBox_TextChanged(sender, e);
    }

    private void searchbar_listView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        _m_search_bar?.listView_SelectionChanged(sender, e);
    }

}
