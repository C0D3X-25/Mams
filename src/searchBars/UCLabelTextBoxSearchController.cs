//using Mams.src.controllers;
//using Mams.src.entities;
//using Mams.src.models;
//using Mams.src.views.userControls;
//using System.Collections.ObjectModel;
//using System.Windows;

//namespace Mams.src.searchBars; 

// class UCLabelTextBoxSearchController : ABaseController {

//    public UCLabelTextBoxSearchController() {
//        EntityModel model = new();
//        _m_selected_item = string.Empty;
//        _m_combobox_items = model.getTable();
//    }

//    private ObservableCollection<EntityItem> _m_combobox_items;
//    public ObservableCollection<EntityItem> m_combobox_items {
//        get => _m_combobox_items;
//        set {
//            _m_combobox_items = value;
//            onPropertyChanged();
//        }
//    }

//    //private List<string> _m_combobox_items;
//    //public List<string> m_combobox_items {
//    //    get => _m_combobox_items;
//    //    set {
//    //        _m_combobox_items = value;
//    //        onPropertyChanged();
//    //    }
//    //}


//    private string _m_selected_item;
//    public string m_selected_item {
//        get => _m_selected_item;
//        set {
//            _m_selected_item = value;
//            onPropertyChanged();
//        }
//    }
//}
