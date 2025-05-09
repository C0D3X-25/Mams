using Mams.src.beehives;
using Mams.src.controllers;
using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.fees;
using Mams.src.products;
using Mams.src.productsCategories;
using Mams.src.productsLots;
using Mams.src.productsShapes;
using Mams.src.productsTypes;
using Mams.src.profits;
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
    private readonly ProfitModel _m_profit_model;
    private readonly FeeModel _m_fee_model;

    private ObservableCollection<EntityItem>? _m_list_entity_all;
    private ObservableCollection<EntityItem>? _m_list_entity_not_archived;
    private ObservableCollection<EntityItem>? _m_list_entity_archived;

    private ObservableCollection<ProductItem>? _m_list_product_all;
    private ObservableCollection<ProductItem>? _m_list_product_not_archived;
    private ObservableCollection<ProductItem>? _m_list_product_archived;

    private ObservableCollection<ProductShapeItem>? _m_list_product_shape_all;
    private ObservableCollection<ProductShapeItem>? _m_list_product_shape_not_archived;
    private ObservableCollection<ProductShapeItem>? _m_list_product_shape_archived;

    private ObservableCollection<ProductCategoryItem>? _m_list_product_category_all;
    private ObservableCollection<ProductCategoryItem>? _m_list_product_category_not_archived;
    private ObservableCollection<ProductCategoryItem>? _m_list_product_category_archived;

    private ObservableCollection<ProductTypeItem>? _m_list_product_type_all;
    private ObservableCollection<ProductTypeItem>? _m_list_product_type_not_archived;
    private ObservableCollection<ProductTypeItem>? _m_list_product_type_archived;

    private ObservableCollection<ProductLotItem>? _m_list_product_lot_all;
    private ObservableCollection<ProductLotItem>? _m_list_product_lot_not_archived;
    private ObservableCollection<ProductLotItem>? _m_list_product_lot_archived;

    private ObservableCollection<BeehiveItem>? _m_list_beehive_all;
    private ObservableCollection<BeehiveItem>? _m_list_beehive_not_archived;
    private ObservableCollection<BeehiveItem>? _m_list_beehive_archived;


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
            updateListSearchItems();
            onPropertyChanged();
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
            updateProfits();
            updateFees();
            onPropertyChanged();
        }
    }


    private ObservableCollection<ProfitItem>? _m_list_profit_item;
    public ObservableCollection<ProfitItem>? m_list_profit_item {
        get { return _m_list_profit_item; }
        set {
            _m_list_profit_item = value;
            onPropertyChanged();
        }
    }


    private ObservableCollection<FeeItem>? _m_list_fee_item;
    public ObservableCollection<FeeItem>? m_list_fee_item {
        get { return _m_list_fee_item; }
        set {
            _m_list_fee_item = value;
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
        _m_profit_model = new();
        _m_fee_model = new();

        populateListOfItems();
        populateListTable();

        updateProfits();
        updateFees();
        updateListSearchItems();
    }


    private void updateProfits() {

        // search all
        if (_m_selected_search_item == null) {
            m_list_profit_item = _m_profit_model.getTable();
        }

    }


    private void updateFees() {

    }


    private void populateListTable() {
        m_list_table = new ObservableCollection<DatabaseTablesNameItem> {
            new(){ m_name_in_database = "", m_name_to_display = "" }, // search all
            new(){ m_name_in_database = EntityModel.m_TBL_NAME, m_name_to_display = "Client/Fournisseur" },
            new(){ m_name_in_database = BeehiveModel._m_TBL_NAME, m_name_to_display = "Rucher" },
            new(){ m_name_in_database = ProductModel.m_TBL_NAME, m_name_to_display = "Produit" },
            new(){ m_name_in_database = ProductShapeModel.m_TBL_NAME, m_name_to_display = "Forme" },
            new(){ m_name_in_database = ProductCategoryModel.m_TBL_NAME, m_name_to_display = "Catégorie" },
            new(){ m_name_in_database = ProductTypeModel.m_TBL_NAME, m_name_to_display = "Type" },
            new(){ m_name_in_database = ProductLotModel.m_TBL_NAME, m_name_to_display = "Lot" }
        };
    }


    private void populateListOfItems() {

        // Entity
        _m_list_entity_all = _m_entity_model.getTable();
        _m_list_entity_archived = new ObservableCollection<EntityItem>(
            _m_list_entity_all.Where(item => item.entity_archive != string.Empty)
        );
        _m_list_entity_not_archived = new ObservableCollection<EntityItem>(
            _m_list_entity_all.Where(item => item.entity_archive == string.Empty)
        );

        // Product 
        _m_list_product_all = _m_product_model.getTable();
        _m_list_product_archived = new ObservableCollection<ProductItem>(
            _m_list_product_all.Where(item => item.product_archive != string.Empty)
        );
        _m_list_product_not_archived = new ObservableCollection<ProductItem>(
            _m_list_product_all.Where(item => item.product_archive == string.Empty)
        );

        // Product Shape
        _m_list_product_shape_all = _m_product_shape_model.getTable();
        _m_list_product_shape_archived = new ObservableCollection<ProductShapeItem>(
            _m_list_product_shape_all.Where(item => item.product_shape_archive != string.Empty)
        );
        _m_list_product_shape_not_archived = new ObservableCollection<ProductShapeItem>(
            _m_list_product_shape_all.Where(item => item.product_shape_archive == string.Empty)
        );

        // Product Category
        _m_list_product_category_all = _m_product_category_model.getTable();
        _m_list_product_category_archived = new ObservableCollection<ProductCategoryItem>(
            _m_list_product_category_all.Where(item => item.product_category_archive != string.Empty)
        );
        _m_list_product_category_not_archived = new ObservableCollection<ProductCategoryItem>(
            _m_list_product_category_all.Where(item => item.product_category_archive == string.Empty)
        );

        // Product Type
        _m_list_product_type_all = _m_product_type_model.getTable();
        _m_list_product_type_archived = new ObservableCollection<ProductTypeItem>(
            _m_list_product_type_all.Where(item => item.product_type_archive != string.Empty)
        );
        _m_list_product_type_not_archived = new ObservableCollection<ProductTypeItem>(
            _m_list_product_type_all.Where(item => item.product_type_archive == string.Empty)
        );

        // Product Lot
        _m_list_product_lot_all = _m_product_lot_model.getTable();
        _m_list_product_lot_archived = new ObservableCollection<ProductLotItem>(
            _m_list_product_lot_all.Where(item => item.product_lot_archive != string.Empty)
        );
        _m_list_product_lot_not_archived = new ObservableCollection<ProductLotItem>(
            _m_list_product_lot_all.Where(item => item.product_lot_archive == string.Empty)
        );

        // Beehive
        _m_list_beehive_all = _m_beehive_model.getTable();
        _m_list_beehive_archived = new ObservableCollection<BeehiveItem>(
            _m_list_beehive_all.Where(item => item.beehive_archive != string.Empty)
        );
        _m_list_beehive_not_archived = new ObservableCollection<BeehiveItem>(
            _m_list_beehive_all.Where(item => item.beehive_archive == string.Empty)
        );
    }


    private void updateListSearchItems() {

        m_list_search_item = new ObservableCollection<SearchItem>();

        if (_m_selected_table == null) {
            return;
        }

        switch (_m_selected_table.m_name_in_database) {
            case EntityModel.m_TBL_NAME:
                if (_m_list_entity_not_archived != null) {
                    foreach (EntityItem item in _m_list_entity_not_archived) {
                        m_list_search_item.Add(new SearchItem {
                            m_id = item.entity_id,
                            m_search_name_to_display = item.entity_name
                        });
                    }
                }
            break;
            case BeehiveModel._m_TBL_NAME:
                if (_m_list_beehive_not_archived != null) {
                    foreach (BeehiveItem item in _m_list_beehive_not_archived) {
                        m_list_search_item.Add(new SearchItem {
                            m_id = item.beehive_id,
                            m_search_name_to_display = item.beehive_name
                        });
                    }
                }
            break;
            case ProductModel.m_TBL_NAME:
                if (_m_list_product_not_archived != null) {
                    foreach (ProductItem item in _m_list_product_not_archived) {
                        m_list_search_item.Add(new SearchItem {
                            m_id = item.product_id,
                            m_search_name_to_display = item.product_name
                        });
                    }
                }
            break;
            case ProductShapeModel.m_TBL_NAME:
                if (_m_list_product_shape_not_archived != null) {
                    foreach (ProductShapeItem item in _m_list_product_shape_not_archived) {
                        m_list_search_item.Add(new SearchItem {
                            m_id = item.product_shape_id,
                            m_search_name_to_display = item.product_shape_name
                        });
                    }
                }
            break;
            case ProductCategoryModel.m_TBL_NAME:
                if (_m_list_product_category_not_archived != null) {
                    foreach (ProductCategoryItem item in _m_list_product_category_not_archived) {
                        m_list_search_item.Add(new SearchItem {
                            m_id = item.product_category_id,
                            m_search_name_to_display = item.product_category_name
                        });
                    }
                }
            break;
            case ProductTypeModel.m_TBL_NAME:
                if (_m_list_product_type_not_archived != null) {
                    foreach (ProductTypeItem item in _m_list_product_type_not_archived) {
                        m_list_search_item.Add(new SearchItem {
                            m_id = item.product_type_id,
                            m_search_name_to_display = item.product_type_name
                        });
                    }
                }
            break;
            case ProductLotModel.m_TBL_NAME:
                if (_m_list_product_lot_not_archived != null) {
                    foreach (ProductLotItem item in _m_list_product_lot_not_archived) {
                        m_list_search_item.Add(new SearchItem {
                            m_id = item.product_lot_id,
                            m_search_name_to_display = item.product_lot_name
                        });
                    }
                }
            break;
        }
    }
}