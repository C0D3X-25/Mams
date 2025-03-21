using Mams.src.models;
using Mams.src.controllers;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.views.userControls;

/// <summary>
/// Interaction logic for UCHeader.xaml
/// </summary>
public partial class UCHeader : UserControl {

    private readonly ABaseSearchModel _m_model = new InventoryModel();
    private readonly SearchBarModel? _m_search_bar;

    public string title {
        get { return (string)GetValue(titleProperty); }
        set { SetValue(titleProperty, value); }
    }

    // Using a DependencyProperty as the backing store for title.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty titleProperty =
        DependencyProperty.Register("title", typeof(string), typeof(UCHeader), new PropertyMetadata("Page Title"));

    public UCHeader() {
        InitializeComponent();
        this.Loaded += UCHeader_Loaded; // need to wait for the window to be loaded

        _m_search_bar = new SearchBarModel(_m_model, this);
    }

    private void UCHeader_Loaded(object sender, RoutedEventArgs e) {
        MainWindow? _m_window = Window.GetWindow(this) as MainWindow;

        if (_m_window != null) {
            UCHeaderController header_controller = new(_m_window.m_page_navigation);
            this.DataContext = header_controller;
        }
    }

    private void searchbar_txtBox_TextChanged(object sender, TextChangedEventArgs e) {
        _m_search_bar?.txtBox_TextChanged(sender, e);
    }

    private void searchbar_listView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        _m_search_bar?.listView_SelectionChanged(sender, e);
        //_m_search_bar?.clearSearchBar(); // Uncomment later !
    }
}
