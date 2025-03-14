using Mams.models;
using Mams.src.ctrl;
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

namespace Mams.views.userControls;

/// <summary>
/// Interaction logic for UCHeader.xaml
/// </summary>
public partial class UCHeader : UserControl {

    private readonly ABaseSearchModel _m_model = new InventoryModel();
    private readonly SearchBarController? _m_search_bar;

    public UCHeader() {
        InitializeComponent();
        _m_search_bar = new SearchBarController(_m_model, this);
    }

    public string? pageTitle {
        get { return title_page_lbl.Content.ToString(); }
        set { title_page_lbl.Content = value; }
    }

    private void searchbar_txtBox_TextChanged(object sender, TextChangedEventArgs e) {
        _m_search_bar?.txtBox_TextChanged(sender, e);
    }

    private void searchbar_listView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        _m_search_bar?.listView_SelectionChanged(sender, e);
        //_m_search_bar?.clearSearchBar(); // Uncomment later !
    }
}
