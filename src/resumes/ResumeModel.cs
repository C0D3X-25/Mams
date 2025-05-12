using Mams.src.beehives;
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

namespace Mams.src.resumes;

// This class is mainely used to manage the filter of the page Resume.
public class ResumeModel {

    private readonly ProfitModel _m_profit_model;
    private readonly FeeModel _m_fee_model;
    private readonly EntityModel _m_entity_model;
    private readonly ProductModel _m_product_model;
    private readonly ProductShapeModel _m_product_shape_model;
    private readonly ProductCategoryModel _m_product_category_model;
    private readonly ProductTypeModel _m_product_type_model;
    private readonly ProductLotModel _m_product_lot_model;
    private readonly BeehiveModel _m_beehive_model;

    private ObservableCollection<ProfitItem>? _m_list_profit_items;
    private ObservableCollection<FeeItem>? _m_list_fee_items;

    private ObservableCollection<EntityItem>? _m_list_entity_all;
    private ObservableCollection<EntityItem>? _m_list_entity_not_archived;

    private ObservableCollection<ProductItem>? _m_list_product_all;
    private ObservableCollection<ProductItem>? _m_list_product_not_archived;

    private ObservableCollection<ProductShapeItem>? _m_list_product_shape_all;
    private ObservableCollection<ProductShapeItem>? _m_list_product_shape_not_archived;

    private ObservableCollection<ProductCategoryItem>? _m_list_product_category_all;
    private ObservableCollection<ProductCategoryItem>? _m_list_product_category_not_archived;

    private ObservableCollection<ProductTypeItem>? _m_list_product_type_all;
    private ObservableCollection<ProductTypeItem>? _m_list_product_type_not_archived;

    private ObservableCollection<ProductLotItem>? _m_list_product_lot_all;
    private ObservableCollection<ProductLotItem>? _m_list_product_lot_not_archived;

    private ObservableCollection<BeehiveItem>? _m_list_beehive_all;
    private ObservableCollection<BeehiveItem>? _m_list_beehive_not_archived;


    public readonly ObservableCollection<DatabaseTablesNameItem> m_search_tables = new() {
        new(){ m_name_in_database = "", m_name_to_display = "" }, // Search all
        new(){ m_name_in_database = EntityModel.m_TBL_NAME, m_name_to_display = "Client/Fournisseur" },
        new(){ m_name_in_database = BeehiveModel._m_TBL_NAME, m_name_to_display = "Rucher" },
        new(){ m_name_in_database = ProductModel.m_TBL_NAME, m_name_to_display = "Produit" },
        new(){ m_name_in_database = ProductShapeModel.m_TBL_NAME, m_name_to_display = "Forme" },
        new(){ m_name_in_database = ProductCategoryModel.m_TBL_NAME, m_name_to_display = "Catégorie" },
        new(){ m_name_in_database = ProductTypeModel.m_TBL_NAME, m_name_to_display = "Type" },
        new(){ m_name_in_database = ProductLotModel.m_TBL_NAME, m_name_to_display = "Lot" }
    };


    public ResumeModel() {

        _m_profit_model = new();
        _m_fee_model = new();
        _m_entity_model = new();
        _m_product_model = new();
        _m_product_shape_model = new();
        _m_product_category_model = new();
        _m_product_type_model = new();
        _m_product_lot_model = new();
        _m_beehive_model = new();

        populateListOfItems();
    }


    public ObservableCollection<ProfitItem> getFilteredProfits(SearchItem? search) {

        if (_m_list_profit_items == null) {
            return new ObservableCollection<ProfitItem>();
        }

        var filtered_data = filterProfitBy(search);

        return sortProfitByYear(filtered_data, search);
    }


    public ObservableCollection<FeeItem> getFilteredFees(SearchItem? search) {

        if (_m_list_fee_items == null) {
            return new ObservableCollection<FeeItem>();
        }

        var filtered_data = filterFeeBy(search);

        return sortFeeByYear(filtered_data, search);
    }


    private ObservableCollection<ProfitItem> filterProfitBy(SearchItem? search) {
        // TODO: sort by other criteria

        return _m_list_profit_items;
    }


    private ObservableCollection<FeeItem> filterFeeBy(SearchItem? search) {
        // TODO: sort by other criteria

        return _m_list_fee_items;
    }

    private ObservableCollection<ProfitItem> sortProfitByYear(ObservableCollection<ProfitItem> data_to_sort, SearchItem? search) {

        if (data_to_sort.Count == 0) {
            return new ObservableCollection<ProfitItem>();
        }

        var query = data_to_sort.AsQueryable();

        // Filter by year if specified in search
        if (search != null) {
            if (search.search_year != string.Empty) {
                string year_to_match = getYearFromDate(search.search_year);
                query = query.Where(item => getYearFromDate(item.profit_date) == year_to_match);
            }
        }

        // Sort by parsed date for correct chronological order
        var sorted_items = query.OrderByDescending(item => DateTime.ParseExact(item.profit_date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture));

        return new ObservableCollection<ProfitItem>(sorted_items);
    }


    private ObservableCollection<FeeItem> sortFeeByYear(ObservableCollection<FeeItem> data_to_sort, SearchItem? search) {

        if (data_to_sort.Count == 0) {
            return new ObservableCollection<FeeItem>();
        }

        var query = data_to_sort.AsQueryable();

        // Filter by year if specified in search
        if (search != null) {
            if (search.search_year != string.Empty) {
                string year_to_match = getYearFromDate(search.search_year);
                query = query.Where(item => getYearFromDate(item.fee_date) == year_to_match);
            }
        }

        // Sort by parsed date for correct chronological order
        var sorted_items = query.OrderByDescending(item => DateTime.ParseExact(item.fee_date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture));

        return new ObservableCollection<FeeItem>(sorted_items);
    }


    private string getYearFromDate(string date) {

        if (date == string.Empty) {
            return string.Empty;
        }
        DateTime parsed_date = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        return parsed_date.Year.ToString();
    }


    private void populateListOfItems() {

        // Entity
        _m_list_entity_all = _m_entity_model.getTable();
        _m_list_entity_not_archived = new ObservableCollection<EntityItem>(
            _m_list_entity_all.Where(item => item.entity_archive == string.Empty)
        );

        // Product 
        _m_list_product_all = _m_product_model.getTable();
        _m_list_product_not_archived = new ObservableCollection<ProductItem>(
            _m_list_product_all.Where(item => item.product_archive == string.Empty)
        );

        // Product Shape
        _m_list_product_shape_all = _m_product_shape_model.getTable();
        _m_list_product_shape_not_archived = new ObservableCollection<ProductShapeItem>(
            _m_list_product_shape_all.Where(item => item.product_shape_archive == string.Empty)
        );

        // Product Category
        _m_list_product_category_all = _m_product_category_model.getTable();
        _m_list_product_category_not_archived = new ObservableCollection<ProductCategoryItem>(
            _m_list_product_category_all.Where(item => item.product_category_archive == string.Empty)
        );

        // Product Type
        _m_list_product_type_all = _m_product_type_model.getTable();
        _m_list_product_type_not_archived = new ObservableCollection<ProductTypeItem>(
            _m_list_product_type_all.Where(item => item.product_type_archive == string.Empty)
        );

        // Product Lot
        _m_list_product_lot_all = _m_product_lot_model.getTable();
        _m_list_product_lot_not_archived = new ObservableCollection<ProductLotItem>(
            _m_list_product_lot_all.Where(item => item.product_lot_archive == string.Empty)
        );

        // Beehive
        _m_list_beehive_all = _m_beehive_model.getTable();
        _m_list_beehive_not_archived = new ObservableCollection<BeehiveItem>(
            _m_list_beehive_all.Where(item => item.beehive_archive == string.Empty)
        );

        // Profit
        _m_list_profit_items = _m_profit_model.getTable();

        // Fee
        _m_list_fee_items = _m_fee_model.getTable();
    }


    public ObservableCollection<SearchItem> getListSearchItems(DatabaseTablesNameItem _m_selected_table) {

        var list_search_item = new ObservableCollection<SearchItem>();

        switch (_m_selected_table.m_name_in_database) {
            case EntityModel.m_TBL_NAME:
                if (_m_list_entity_not_archived != null) {
                    foreach (EntityItem item in _m_list_entity_not_archived) {
                        list_search_item.Add(new SearchItem {
                            id = item.entity_id,
                            search_name_to_display = item.entity_name
                        });
                    }
                }
                break;
            case BeehiveModel._m_TBL_NAME:
                if (_m_list_beehive_not_archived != null) {
                    foreach (BeehiveItem item in _m_list_beehive_not_archived) {
                        list_search_item.Add(new SearchItem {
                            id = item.beehive_id,
                            search_name_to_display = item.beehive_name
                        });
                    }
                }
                break;
            case ProductModel.m_TBL_NAME:
                if (_m_list_product_not_archived != null) {
                    foreach (ProductItem item in _m_list_product_not_archived) {
                        list_search_item.Add(new SearchItem {
                            id = item.product_id,
                            search_name_to_display = item.product_name
                        });
                    }
                }
                break;
            case ProductShapeModel.m_TBL_NAME:
                if (_m_list_product_shape_not_archived != null) {
                    foreach (ProductShapeItem item in _m_list_product_shape_not_archived) {
                        list_search_item.Add(new SearchItem {
                            id = item.product_shape_id,
                            search_name_to_display = item.product_shape_name
                        });
                    }
                }
                break;
            case ProductCategoryModel.m_TBL_NAME:
                if (_m_list_product_category_not_archived != null) {
                    foreach (ProductCategoryItem item in _m_list_product_category_not_archived) {
                        list_search_item.Add(new SearchItem {
                            id = item.product_category_id,
                            search_name_to_display = item.product_category_name
                        });
                    }
                }
                break;
            case ProductTypeModel.m_TBL_NAME:
                if (_m_list_product_type_not_archived != null) {
                    foreach (ProductTypeItem item in _m_list_product_type_not_archived) {
                        list_search_item.Add(new SearchItem {
                            id = item.product_type_id,
                            search_name_to_display = item.product_type_name
                        });
                    }
                }
                break;
            case ProductLotModel.m_TBL_NAME:
                if (_m_list_product_lot_not_archived != null) {
                    foreach (ProductLotItem item in _m_list_product_lot_not_archived) {
                        list_search_item.Add(new SearchItem {
                            id = item.product_lot_id,
                            search_name_to_display = item.product_lot_name
                        });
                    }
                }
                break;
        }

        return list_search_item;
    }
}
