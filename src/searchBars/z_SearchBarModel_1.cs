//using Mams.src.views.UserControls;
//using System.Windows;
//using System.Windows.Controls;

//namespace Mams.src.searchBars;

///// <summary>
///// Handles dropdown functionality with a ComboBox for selecting predefined items from database.
///// </summary>
//public class z_SearchBarModel {

//    private readonly ComboBox _m_combo_box;
//    private ABaseSearchModel? _m_model;

//    /// <summary>
//    /// Initializes a new instance of the SearchBar class.
//    /// </summary>
//    /// <param name="model">The data model to use for getting items</param>
//    /// <param name="combo_box">The ComboBox control for item selection</param>
//    public z_SearchBarModel(ABaseSearchModel model, ComboBox combo_box) {
//        _m_model = model;
//        _m_combo_box = combo_box;
//        loadItems();
//    }

//    public z_SearchBarModel(ABaseSearchModel model, UCLabelTextBoxSearch control) {
//        _m_model = model;
//        _m_combo_box = control.searchComboBox;
//        loadItems();
//    }

//    public void setModel(ABaseSearchModel model) {
//        _m_model = model;
//        loadItems();
//    }

//    public void clearSelection() {
//        _m_combo_box.SelectedItem = null;
//    }

//    private void loadItems() {
//        if (_m_model != null) {
//            var items = _m_model.searchItems(string.Empty); // Get all items
//            _m_combo_box.ItemsSource = items;
//        }
//    }
//}
