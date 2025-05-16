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
using Mams.src.receipts;
using System.Collections.ObjectModel;
using System.Windows;

namespace Mams.src.resumes;

// This class is mainely used to manage the filter of the page Resume,
// that why this class doesn't inherit from ABaseModel
public class ResumeModel {

    private readonly ProfitModel _m_profit_model;
    private readonly FeeModel _m_fee_model;
    private readonly ReceiptModel _m_receipt_model;
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
        new(){ m_name_in_database = EDatabaseTableName.NONE, m_name_to_display = "" }, // Search all
        new(){ m_name_in_database = EDatabaseTableName.ENTITY, m_name_to_display = "Client/Fournisseur" },
        new(){ m_name_in_database = EDatabaseTableName.BEEHIVE, m_name_to_display = "Rucher" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT, m_name_to_display = "Produit" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_SHAPE, m_name_to_display = "Forme" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_CATEGORY, m_name_to_display = "Catégorie" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_TYPE, m_name_to_display = "Type" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_LOT, m_name_to_display = "Lot" }
    };


    public ResumeModel() {

        _m_profit_model = new();
        _m_fee_model = new();
        _m_receipt_model = new();
        _m_entity_model = new();
        _m_product_model = new();
        _m_product_shape_model = new();
        _m_product_category_model = new();
        _m_product_type_model = new();
        _m_product_lot_model = new();
        _m_beehive_model = new();

        populateListOfItems();
    }


    public ResumeItem getFilteredResume(SearchItem? search) {

        var resume_item = new ResumeItem();

        if (_m_list_profit_items == null || _m_list_fee_items == null) {
            return resume_item;
        }

        resume_item = filterBy(search);

        return sortByYear(resume_item, search);
    }


    public ObservableCollection<SearchItem> getListSearchItems(DatabaseTablesNameItem selected_table) {

        var list_search_item = new ObservableCollection<SearchItem>();

        switch (selected_table.m_name_in_database) {
            case EDatabaseTableName.ENTITY:
                if (_m_list_entity_not_archived != null) {
                    foreach (EntityItem item in _m_list_entity_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.entity_id,
                            search_item_to_display = item.entity_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.BEEHIVE:
                if (_m_list_beehive_not_archived != null) {
                    foreach (BeehiveItem item in _m_list_beehive_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.beehive_id,
                            search_item_to_display = item.beehive_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT:
                if (_m_list_product_not_archived != null) {
                    foreach (ProductItem item in _m_list_product_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_id,
                            search_item_to_display = item.product_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_SHAPE:
                if (_m_list_product_shape_not_archived != null) {
                    foreach (ProductShapeItem item in _m_list_product_shape_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_shape_id,
                            search_item_to_display = item.product_shape_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_CATEGORY:
                if (_m_list_product_category_not_archived != null) {
                    foreach (ProductCategoryItem item in _m_list_product_category_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_category_id,
                            search_item_to_display = item.product_category_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_TYPE:
                if (_m_list_product_type_not_archived != null) {
                    foreach (ProductTypeItem item in _m_list_product_type_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_type_id,
                            search_item_to_display = item.product_type_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_LOT:
                if (_m_list_product_lot_not_archived != null) {
                    foreach (ProductLotItem item in _m_list_product_lot_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_lot_id,
                            search_item_to_display = item.product_lot_name
                        });
                    }
                }
                break;
        }
        return list_search_item;
    }


    public ObservableCollection<SearchItem> getListYears() {

        var list_search_year = new ObservableCollection<SearchItem>();
        var years = _m_receipt_model.getExistingYear();

        if (years == null) {
            return list_search_year;
        }

        list_search_year.Add(new SearchItem {
            search_year = string.Empty,
            search_item_to_display = string.Empty
        });
        foreach (string year in years) {
            list_search_year.Add(new SearchItem {
                search_year = year,
                search_item_to_display = string.Empty
            });
        }

        return list_search_year;
    }


    private ResumeItem filterBy(SearchItem? search) {

        var filtered_data = new ResumeItem();

        if (_m_list_profit_items == null) {
            return filtered_data;
        }
        if (_m_list_fee_items == null) {
            return filtered_data;
        }
        if (search == null) {
            filtered_data.profit_items = _m_list_profit_items;
            filtered_data.fee_items = _m_list_fee_items;
            return filtered_data;
        }

        switch (search.search_table) {
            case EDatabaseTableName.NONE:
                filtered_data.profit_items = _m_list_profit_items;
                filtered_data.fee_items = _m_list_fee_items;
                break;
            case EDatabaseTableName.ENTITY:
                if (search.search_item_to_display == string.Empty) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit => profit.client_name == search.search_item_to_display)
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee => fee.supplier_name == search.search_item_to_display)
                );
                break;
            case EDatabaseTableName.PRODUCT:
                if (search.search_item_to_display == string.Empty) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit => profit.product_name == search.search_item_to_display)
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee => fee.product_name == search.search_item_to_display)
                );
                break;
            case EDatabaseTableName.PRODUCT_TYPE:
                if (search.search_id == 0) {
                    break;
                }
                var product_items_t = _m_product_model.getProductWithProductTypeId(new List<int> { search.search_id });

                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(fee =>
                    product_items_t.Any(product =>
                    product.product_name == fee.product_name))
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                    product_items_t.Any(product =>
                    product.product_name == fee.product_name))
                );
                break;
            case EDatabaseTableName.PRODUCT_CATEGORY:
                if (search.search_id == 0) {
                    break;
                }
                var product_items_c = _m_product_model.getProductWithProductCategoryId(new List<int> { search.search_id });

                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                    product_items_c.Any(product =>
                    product.product_name == profit.product_name))
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                    product_items_c.Any(product =>
                    product.product_name == fee.product_name))
                );
                break;
            case EDatabaseTableName.PRODUCT_SHAPE:
                if (search.search_id == 0) {
                    break;
                }
                var product_items_s = _m_product_model.getProductWithProductShapeId(new List<int> { search.search_id });

                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                    product_items_s.Any(product =>
                    product.product_name == profit.product_name))
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                    product_items_s.Any(product =>
                    product.product_name == fee.product_name))
                );
                break;
            case EDatabaseTableName.PRODUCT_LOT:
                if (search.search_id == 0) {
                    break;
                }
                var product_items_l = _m_product_model.getProductWithProductLotId(new List<int> { search.search_id });

                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                    product_items_l.Any(product =>
                    product.product_name == profit.product_name))
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                    product_items_l.Any(product =>
                    product.product_name == fee.product_name))
                );
                break;
            case EDatabaseTableName.BEEHIVE:
                if (search.search_id == 0) {
                    break;
                }
                var product_lot_p = _m_product_lot_model.getProductLotWithBeehiveId(new List<int> { search.search_id });
                var product_lot_ids = product_lot_p.Select(lot => lot.product_lot_id).ToList();
                var product_items_b = _m_product_model.getProductWithProductLotId(product_lot_ids);

                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                    product_items_b.Any(product =>
                    product.product_name == profit.product_name))
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                    product_items_b.Any(product =>
                    product.product_name == fee.product_name))
                );
                break;
        }

        return filtered_data;
    }


    private ResumeItem sortByYear(ResumeItem data_to_sort, SearchItem? search) {

        if (data_to_sort.profit_items.Count == 0 && data_to_sort.fee_items.Count == 0) {
            return new ResumeItem();
        }

        var query_profit = data_to_sort.profit_items.AsQueryable();
        var query_fee = data_to_sort.fee_items.AsQueryable();

        // Filter by year if specified in search
        if (search != null) {
            if (search.search_year != string.Empty) {
                string year_to_match = getYearFromDate(search.search_year);
                query_profit = query_profit.Where(item => getYearFromDate(item.profit_date) == year_to_match);
                query_fee = query_fee.Where(item => getYearFromDate(item.fee_date) == year_to_match);
            }
        }

        // Sort by parsed date for correct chronological order
        var sorted_items = new ResumeItem();

        if (data_to_sort.profit_items.Count != 0) {
            sorted_items.profit_items = new ObservableCollection<ProfitItem>(
                query_profit.OrderByDescending(item => DateTime.ParseExact(item.profit_date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture))
            );
        }
        if (data_to_sort.fee_items.Count != 0) {
            sorted_items.fee_items = new ObservableCollection<FeeItem>(
                query_fee.OrderByDescending(item => DateTime.ParseExact(item.fee_date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture))
            );
        }

        return sorted_items;
    }


    private string getYearFromDate(string date) {

        if (date == string.Empty) {
            return string.Empty;
        }
        // If the date is already a year (4 digits)
        if (date.Length == 4) {
            return date;
        }

        DateTime parsed_date = DateTime.ParseExact(date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);

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
}
