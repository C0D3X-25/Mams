using Mams.src.beehives;
using Mams.src.controllers;
using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.items;
using Mams.src.models;
using Mams.src.products;
using Mams.src.productsCategories;
using Mams.src.productsLots;
using Mams.src.productsShapes;
using Mams.src.productsTypes;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.resumes;

public class ResumeController : ABaseController {

    private readonly EntityModel _m_entity_model;
    private readonly ProductModel _m_product_model;
    private readonly ProductShapeModel _m_product_shape_model;
    private readonly ProductCategoryModel _m_product_category_model;
    private readonly ProductTypeModel _m_product_type_model;
    private readonly ProductLotModel _m_product_lot_model;
    private readonly BeehiveModel _m_beehive_model;


    private ObservableCollection<DatabaseTablesNameItem>? _m_list_table;
    public ObservableCollection<DatabaseTablesNameItem>? m_list_table {
        get { return _m_list_table; }
        set {
            _m_list_table = value;
            onPropertyChanged();
        }
    }


    private DatabaseTablesNameItem? _m_selected_table;
    public DatabaseTablesNameItem? m_selected_table {
        get { return _m_selected_table; }
        set {
            _m_selected_table = value;
            onPropertyChanged();
            updateListSearchItems();
        }
    }


    private ObservableCollection<SearchItem>? _m_list_search_item;
    public ObservableCollection<SearchItem>? m_list_search_item {
        get { return _m_list_search_item; }
        set {
            _m_list_search_item = value;
            onPropertyChanged();
        }
    }


    private SearchItem? _m_selected_search_item;
    public SearchItem? m_selected_search_item {
        get { return _m_selected_search_item; }
        set {
            _m_selected_search_item = value;
            onPropertyChanged();
        }
    }


    public ResumeController() {

        _m_entity_model = new();
        _m_product_model = new();
        _m_product_shape_model = new();
        _m_product_category_model = new();
        _m_product_type_model = new();
        _m_product_lot_model = new();
        _m_beehive_model = new();

        m_list_table = new ObservableCollection<DatabaseTablesNameItem> {
            new DatabaseTablesNameItem{ m_name_in_database = "", m_name_to_display = "" }, // search all
            //new DatabaseTablesNameItem{ m_name_in_database = EntityModel.m_TBL_NAME, m_name_to_display = "Client" },
            //new DatabaseTablesNameItem{ m_name_in_database = EntityModel.m_TBL_NAME, m_name_to_display = "Fournisseur" },
            new DatabaseTablesNameItem{ m_name_in_database = EntityModel.m_TBL_NAME, m_name_to_display = "Client/Fournisseur" },
            new DatabaseTablesNameItem{ m_name_in_database = BeehiveModel._m_TBL_NAME, m_name_to_display = "Rucher" },
            new DatabaseTablesNameItem{ m_name_in_database = ProductModel.m_TBL_NAME, m_name_to_display = "Produit" },
            new DatabaseTablesNameItem{ m_name_in_database = ProductShapeModel.m_TBL_NAME, m_name_to_display = "Forme" },
            new DatabaseTablesNameItem{ m_name_in_database = ProductCategoryModel.m_TBL_NAME, m_name_to_display = "Catégorie" },
            new DatabaseTablesNameItem{ m_name_in_database = ProductTypeModel.m_TBL_NAME, m_name_to_display = "Type" },
            new DatabaseTablesNameItem{ m_name_in_database = ProductLotModel.m_TBL_NAME, m_name_to_display = "Lot" }
            //new DatabaseTablesNameItem{ m_name_in_database = .m_TBL_NAME, m_name_to_display = "Bon de livraison" },
            //new DatabaseTablesNameItem{ m_name_in_database = EntityModel.m_TBL_NAME, m_name_to_display = "Produit de bon de livraison" }
        };

        updateListItems();
        updateListSearchItems();
    }


    private void updateListSearchItems() {

        m_list_search_item = new ObservableCollection<SearchItem>();

        if (_m_selected_table == null) {
            return;
        }

        // TODO: refactor this part to have a single method to get the items
        // maybe get all tables and then filter them
        switch (_m_selected_table.m_name_in_database) {
            case EntityModel.m_TBL_NAME:

                ObservableCollection<EntityItem> items = _m_entity_model.getTable();

                if (items != null) {
                    foreach (EntityItem item in items) {
                        m_list_search_item.Add(new SearchItem {
                            m_id = item.entity_id,
                            m_search_name_to_display = item.entity_name
                        });
                    }
                }
                break;
            case BeehiveModel._m_TBL_NAME:
                break;
            case ProductModel.m_TBL_NAME:
                break;
            case ProductShapeModel.m_TBL_NAME:
                break;
            case ProductCategoryModel.m_TBL_NAME:
                break;
            case ProductTypeModel.m_TBL_NAME:
                break;
            case ProductLotModel.m_TBL_NAME:
                break;
        }
    }

    //private ObservableCollection<SearchItem> toSearchItem(ObservableCollection<ABaseItem> source_items) {

    //    ObservableCollection<SearchItem> items = new();

    //    if (source_items == null) {
    //        return items;
    //    }

    //    foreach (ABaseItem item in source_items) {
    //        SearchItem search_item = new();
    //        search_item.m_id = item.m_id;
    //        search_item.m_search_name_to_display = item.m_search_name_to_display;
    //        items.Add(search_item);
    //    }

    //    return items;
    //}


    private void updateListItems() {
        //var all_items = _m_item_model.getTable();

        //if (!_m_is_show_archived_checked) {
        //    m_list_items = new ObservableCollection<ProductItem>(
        //        all_items.Where(item => item.product_archive == string.Empty)
        //    );
        //    m_delete_button_text = "Supprimer";
        //}
        //else {
        //    m_list_items = new ObservableCollection<ProductItem>(
        //        all_items.Where(item => item.product_archive != string.Empty)
        //    );
        //    m_delete_button_text = "Restaurer";
        //}
    }
}