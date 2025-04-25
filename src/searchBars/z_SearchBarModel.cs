//using Mams.src.views.userControls;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Controls.Primitives;
//using System.Windows.Threading;


//namespace Mams.src.searchBars;

///// <summary>
///// Handles search functionality with a text box input and ListView display of results.
///// </summary>
//public class z_SearchBarModel {

//    private readonly TextBox _m_txt_box;
//    private readonly ListView _m_list_view;
//    private readonly Popup _m_popup;
//    private ABaseSearchModel? _m_model;
//    private readonly DispatcherTimer _m_search_timer;
//    private const ushort _m_min_text_length_to_start_search = 1;
//    private const ushort _m_delay_ms_between_search = 100;
//    private bool _m_search_selected = false;
//    private bool _m_is_item_selected = false;

//    /// <summary>
//    /// Initializes a new instance of the SearchBar class.
//    /// </summary>
//    /// <param name="model">The data model to use for searches</param>
//    /// <param name="txt_box">The TextBox control for search input</param>
//    /// <param name="list_view">The ListView control to display search results</param>
//    public z_SearchBarModel(ABaseSearchModel model, TextBox txt_box, ListView list_view, Popup popup) {
//        _m_model = model;
//        _m_txt_box = txt_box;
//        _m_list_view = list_view;
//        _m_popup = popup;
//        _m_search_timer = new DispatcherTimer();
//        setSearchTimer();
//    }

//    public z_SearchBarModel(ABaseSearchModel model, UCLabelTextBoxSearch textBox_search) {
//        _m_model = model;
//        _m_txt_box = textBox_search.labelTextBox_txtBox;
//        _m_list_view = textBox_search.searchbar_listView;
//        _m_popup = textBox_search.searchPopup;
//        _m_search_timer = new DispatcherTimer();
//        setSearchTimer();
//    }

//    public z_SearchBarModel(ABaseSearchModel model, UCHeader textBox_search) {
//        _m_model = model;
//        _m_txt_box = textBox_search.searchbar_txtBox;
//        _m_list_view = textBox_search.searchbar_listView;
//        _m_popup = textBox_search.searchPopup;
//        _m_search_timer = new DispatcherTimer();
//        setSearchTimer();
//    }

//    public void setModel(ABaseSearchModel model) {
//        _m_model = model;
//    }

//    public void clearSearchBar() {
//        _m_txt_box.Text = string.Empty;
//        _m_search_selected = false;
//        _m_is_item_selected = false;
//    }

//    public void txtBox_TextChanged(object sender, TextChangedEventArgs e) {

//        if (_m_is_item_selected) {
//            _m_is_item_selected = false;
//            return;
//        }

//        if (_m_txt_box.Text.Length >= _m_min_text_length_to_start_search
//            && !_m_search_selected) {

//            _m_search_timer.Stop();
//            _m_search_timer.Start();
//        }
//        else {
//            _m_search_selected = false;
//            _m_list_view.Visibility = Visibility.Collapsed;
//            _m_popup.IsOpen = false;
//        }
//    }

//    public void listView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
//        if (_m_list_view.SelectedItem != null) {
//            _m_search_selected = true;
//            _m_is_item_selected = true;
//            _m_txt_box.Text = _m_list_view.SelectedItem.ToString();
//            _m_list_view.Items.Clear();
//            _m_list_view.Visibility = Visibility.Collapsed;
//            _m_popup.IsOpen = false;
//        }
//    }

//    private void setSearchTimer() {
//        _m_search_timer.Interval = TimeSpan.FromMilliseconds(_m_delay_ms_between_search);
//        _m_search_timer.Tick += searchTimerTick;
//    }

//    private void searchTimerTick(object? sender, EventArgs e) {
//        _m_search_timer.Stop();
//        updateSearchResults();
//    }

//    private void updateSearchResults() {
//        string search_text = _m_txt_box.Text.Trim();

//        if (search_text.Length >= _m_min_text_length_to_start_search && _m_model != null) {
//            var results = _m_model.searchItems(search_text);

//            _m_list_view.Items.Clear();

//            foreach (var result in results) {
//                _m_list_view.Items.Add(result);
//            }

//            if (results.Count > 0) {
//                // Calculate desired height based on items
//                const double ESTIMATED_ITEM_HEIGHT = 27;

//                // Calculate height based on actual number of items, but cap at 10
//                int items_to_show = Math.Min(results.Count, 10);
//                double desired_height = items_to_show * ESTIMATED_ITEM_HEIGHT + 3;

//                _m_list_view.Height = desired_height;
//                _m_list_view.Visibility = Visibility.Visible;
//                _m_popup.IsOpen = true;
//                Panel.SetZIndex(_m_list_view, 1000);
//            }
//            else {
//                _m_list_view.Visibility = Visibility.Collapsed;
//                _m_popup.IsOpen = false;
//            }
//        }
//    }
//}
