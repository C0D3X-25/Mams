using Mams.models;
using Mams.src.imp;
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
    private readonly SearchBar? _m_search_bar;

    public UCHeader() {
        InitializeComponent();
        _m_search_bar = new SearchBar(_m_model, searchbar_txtBox, searchbar_listView);
    }

    private void searchbar_txtBox_TextChanged(object sender, TextChangedEventArgs e) {
        _m_search_bar?.txtBox_TextChanged(sender, e);
        searchPopup.IsOpen = !string.IsNullOrEmpty(searchbar_txtBox.Text);
    }
}
