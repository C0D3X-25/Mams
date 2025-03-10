//using Mams.models;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Controls;

//namespace Mams.src.imp;

///// <summary>
///// Handles search functionality with a text box input and ListView display of results.
///// </summary>
//public class SearchBar {
    
//    private readonly TextBox _m_txt_box;
//    private readonly ListView _m_list_view;
//    private ABaseSearchModel? _m_model;
//    private readonly System.Windows.Forms.Timer _m_search_timer = new();
//    private const UInt16 _m_min_text_length_to_start_search = 1;
//    private const UInt16 _m_delay_ms_between_search = 100;
//    private bool _m_search_selected = false;
//    private bool _m_is_item_selected = false;


//    /// <summary>
//    /// Initializes a new instance of the SearchBar class.
//    /// </summary>
//    /// <param name="model">The data model to use for searches</param>
//    /// <param name="txt_box">The TextBox control for search input</param>
//    /// <param name="list_view">The ListView control to display search results</param>
//    public SearchBar(ABaseSearchModel model, TextBox txt_box, ListView list_view) {
//        _m_model = model;
//        _m_txt_box = txt_box;
//        _m_list_view = list_view;

//        setSearchTimer();
//    }

//    /// <summary>
//    /// Initializes a new instance of the SearchBar class, only the Controls.
//    /// </summary>
//    /// <param name="txt_box">The TextBox control for search input</param>
//    /// <param name="list_view">The ListView control to display search results</param>
//    public SearchBar(TextBox txt_box, ListView list_view) {
//        _m_txt_box = txt_box;
//        _m_list_view = list_view;

//        setSearchTimer();
//    }

//    /// <summary>
//    /// Initializes the model of the instance.
//    /// </summary>
//    /// <param name="model">The data model to use for searches</param>
//    public void setModel(ABaseSearchModel model) {
//        _m_model = model;
//    }

//    /// <summary>
//    /// Clears the search bar and hides the ListView.
//    /// </summary>
//    public void clearSearchBar() {
//        _m_txt_box.Text = string.Empty;
//        _m_search_selected = false;
//        _m_is_item_selected = false;
//    }


//    /// <summary>
//    /// Event handler for TextBox text changes. Initiates search after delay if conditions are met.
//    /// </summary>
//    /// <param name="sender">The source of the event</param>
//    /// <param name="e">Event arguments</param>
//    public void txtBox_TextChanged(object sender, EventArgs e) {

//        // This is here because when selecting an item, the txtBox change and start a new search
//        if (_m_is_item_selected) {
//            _m_is_item_selected = false;
//            return;
//        }

//        if (_m_txt_box.Text.Length >= _m_min_text_length_to_start_search
//            && !_m_search_selected) {

//            _m_search_timer.Stop();
//            _m_search_timer.Start(); // trigger the search after delay
//        }
//        else {
//            _m_search_selected = false;
//            _m_list_view.Visible = false;
//        }
//    }


//    /// <summary>
//    /// Event handler for ListView item selection changes.
//    /// Updates TextBox with selected item and hides the ListView.
//    /// </summary>
//    /// <param name="sender">The source of the event</param>
//    /// <param name="e">Event arguments containing the selected item information</param>
//    public void listView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e) {
//        if (e.IsSelected && e.Item != null) {
//            _m_search_selected = true;
//            _m_is_item_selected = true;
//            _m_txt_box.Text = e.Item.Text;
//            _m_list_view.EndUpdate();
//            _m_list_view.Items.Clear();
//            _m_list_view.Visible = false;
//            clearSearchBar();
//        }
//    }


//    /// <summary>
//    /// Event handler for mouse movement over the ListView.
//    /// Highlights the item under the mouse cursor and resets others.
//    /// </summary>
//    /// <param name="sender">The source of the event</param>
//    /// <param name="e">Mouse event arguments containing cursor position</param>
//    public void listView_MouseMove(object sender, MouseEventArgs e) {

//        ListViewItem? hovered_item = _m_list_view.HitTest(e.Location).Item;

//        // Reset all items to default colors first
//        foreach (ListViewItem item in _m_list_view.Items) {
//            if (item != hovered_item) {
//                item.BackColor = _m_list_view.BackColor;
//                item.ForeColor = _m_list_view.ForeColor;
//                item.UseItemStyleForSubItems = true;
//            }
//        }

//        // Highlight the hovered item if any
//        if (hovered_item != null) {
//            hovered_item.BackColor = SystemColors.Highlight;
//            hovered_item.ForeColor = SystemColors.HighlightText;
//            hovered_item.UseItemStyleForSubItems = true;
//        }
//    }


//    /// <summary>
//    /// Configures the search timer with specified interval and event handler.
//    /// </summary>
//    private void setSearchTimer() {
//        _m_search_timer.Interval = _m_delay_ms_between_search;
//        _m_search_timer.Tick += searchTimerTick;
//    }


//    /// <summary>
//    /// Event handler for search timer tick events.
//    /// Stops the timer and triggers search result update.
//    /// </summary>
//    /// <param name="sender">The source of the event</param>
//    /// <param="e">Event arguments</param>
//    private void searchTimerTick(object? sender, EventArgs e) {
//        _m_search_timer.Stop();
//        updateSearchResults();
//    }


//    /// <summary>
//    /// Updates the ListView with search results from the database based on the current TextBox content.
//    /// Adjusts the ListView size and visibility based on the results.
//    /// </summary>
//    /// <remarks>
//    /// This method:
//    /// - Retrieves search results from the model if search text meets minimum length
//    /// - Updates the ListView items with the results
//    /// - Adjusts ListView height based on number of results
//    /// - Shows/hides ListView based on whether results were found
//    /// </remarks>
//    private void updateSearchResults() {
//        string search_text = _m_txt_box.Text.Trim();

//        if (search_text.Length >= _m_min_text_length_to_start_search && _m_model != null) {
//            var results = _m_model.searchItems(search_text);

//            _m_list_view.BeginUpdate();
//            _m_list_view.Items.Clear();

//            foreach (var result in results) {
//                _m_list_view.Items.Add(new ListViewItem(result));
//            }

//            if (results.Count > 0) {
//                // Calculate item height including padding
//                int itemHeight = _m_list_view.GetItemRect(0).Height;

//                // Calculate total height (items + borders)
//                int totalHeight = (itemHeight * Math.Min(results.Count, 10)) + 4;

//                // Update size while maintaining width
//                _m_list_view.Size = new Size(_m_list_view.Width, totalHeight);

//                _m_list_view.Visible = true;
//                _m_list_view.BringToFront();
//            }
//            else {
//                _m_list_view.Visible = false;
//            }

//            _m_list_view.EndUpdate();
//        }
//    }
//}
