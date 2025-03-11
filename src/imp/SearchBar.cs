using Mams.models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace Mams.src.imp;

/// <summary>
/// Handles search functionality with a text box input and ListView display of results.
/// </summary>
public class SearchBar {

    private readonly TextBox _m_txt_box;
    private readonly ListView _m_list_view;
    private ABaseSearchModel? _m_model;
    private readonly DispatcherTimer _m_search_timer;
    private const UInt16 _m_min_text_length_to_start_search = 1;
    private const UInt16 _m_delay_ms_between_search = 100;
    private bool _m_search_selected = false;
    private bool _m_is_item_selected = false;

    /// <summary>
    /// Initializes a new instance of the SearchBar class.
    /// </summary>
    /// <param name="model">The data model to use for searches</param>
    /// <param name="txt_box">The TextBox control for search input</param>
    /// <param name="list_view">The ListView control to display search results</param>
    public SearchBar(ABaseSearchModel model, TextBox txt_box, ListView list_view) {
        _m_model = model;
        _m_txt_box = txt_box;
        _m_list_view = list_view;
        _m_search_timer = new DispatcherTimer();
        setSearchTimer();
    }

    /// <summary>
    /// Initializes a new instance of the SearchBar class, only the Controls.
    /// </summary>
    /// <param name="txt_box">The TextBox control for search input</param>
    /// <param name="list_view">The ListView control to display search results</param>
    public SearchBar(TextBox txt_box, ListView list_view) {
        _m_txt_box = txt_box;
        _m_list_view = list_view;
        _m_search_timer = new DispatcherTimer();
        setSearchTimer();
    }

    public void setModel(ABaseSearchModel model) {
        _m_model = model;
    }

    public void clearSearchBar() {
        _m_txt_box.Text = string.Empty;
        _m_search_selected = false;
        _m_is_item_selected = false;
    }

    public void txtBox_TextChanged(object sender, TextChangedEventArgs e) {
        if (_m_is_item_selected) {
            _m_is_item_selected = false;
            return;
        }

        if (_m_txt_box.Text.Length >= _m_min_text_length_to_start_search
            && !_m_search_selected) {

            _m_search_timer.Stop();
            _m_search_timer.Start();
        }
        else {
            _m_search_selected = false;
            _m_list_view.Visibility = Visibility.Collapsed;
        }
    }

    public void listView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (_m_list_view.SelectedItem != null) {
            _m_search_selected = true;
            _m_is_item_selected = true;
            _m_txt_box.Text = _m_list_view.SelectedItem.ToString();
            _m_list_view.Items.Clear();
            _m_list_view.Visibility = Visibility.Collapsed;
            clearSearchBar();
        }
    }

    public void listView_MouseMove(object sender, MouseEventArgs e) {
        Point mousePosition = e.GetPosition(_m_list_view);
        HitTestResult result = VisualTreeHelper.HitTest(_m_list_view, mousePosition);

        ListViewItem? hoveredItem = null;
        DependencyObject? current = result?.VisualHit;
        while (current != null) {
            if (current is ListViewItem item) {
                hoveredItem = item;
                break;
            }
            current = VisualTreeHelper.GetParent(current);
        }

        // Reset all items to default style
        foreach (object item in _m_list_view.Items) {
            ListViewItem? container = _m_list_view.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;
            if (container != null && container != hoveredItem) {
                container.Background = Brushes.Transparent;
                container.Foreground = SystemColors.WindowTextBrush;
            }
        }

        // Highlight hovered item
        if (hoveredItem != null) {
            hoveredItem.Background = SystemColors.HighlightBrush;
            hoveredItem.Foreground = SystemColors.HighlightTextBrush;
        }
    }

    private void setSearchTimer() {
        _m_search_timer.Interval = TimeSpan.FromMilliseconds(_m_delay_ms_between_search);
        _m_search_timer.Tick += searchTimerTick;
    }

    private void searchTimerTick(object? sender, EventArgs e) {
        _m_search_timer.Stop();
        updateSearchResults();
    }

    private void updateSearchResults() {
        string search_text = _m_txt_box.Text.Trim();

        if (search_text.Length >= _m_min_text_length_to_start_search && _m_model != null) {
            var results = _m_model.searchItems(search_text);

            _m_list_view.Items.Clear();

            foreach (var result in results) {
                _m_list_view.Items.Add(result);
            }

            if (results.Count > 0) {
                // Calculate desired height based on items (using approximate item height)
                const double estimated_item_height = 25;
                double desired_height = Math.Min(results.Count * estimated_item_height, 9 * estimated_item_height);

                _m_list_view.Height = desired_height;
                _m_list_view.Visibility = Visibility.Visible;
                Panel.SetZIndex(_m_list_view, 1000);
            }
            else {
                _m_list_view.Visibility = Visibility.Collapsed;
            }
        }
    }
}
