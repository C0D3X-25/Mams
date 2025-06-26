using Mams.src.beehives;
using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.fees;
using Mams.src.helpers;
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

    private readonly ReceiptProfitDetailedModel _m_profit_model = new();
    private readonly ReceiptFeeDetailedModel _m_fee_model = new();
    private readonly ReceiptModel _m_receipt_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly ProductModel _m_product_model = new();
    private readonly ProductShapeModel _m_product_shape_model = new();
    private readonly ProductCategoryModel _m_product_category_model = new();
    private readonly ProductTypeModel _m_product_type_model = new();
    private readonly ProductLotModel _m_product_lot_model = new();
    private readonly BeehiveModel _m_beehive_model = new();

    private ObservableCollection<ReceiptProfitDetailedItem>? _m_list_profit_items;
    private ObservableCollection<ReceiptFeeDetailedItem>? _m_list_fee_items;

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

    /// <summary>
    /// Represents a predefined collection of database table names and their corresponding display names.
    /// </summary>
    public readonly ObservableCollection<DatabaseTablesNameItem> m_search_tables = new() {
        new(){ m_name_in_database = EDatabaseTableName.NONE, m_name_to_display = string.Empty }, // Search all
        new(){ m_name_in_database = EDatabaseTableName.ENTITY, m_name_to_display = "Client/Fournisseur" },
        new(){ m_name_in_database = EDatabaseTableName.BEEHIVE, m_name_to_display = "Rucher" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT, m_name_to_display = "Produit" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_SHAPE, m_name_to_display = "Forme" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_CATEGORY, m_name_to_display = "Catégorie" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_TYPE, m_name_to_display = "Type" },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_LOT, m_name_to_display = "Lot" }
    };


    public ResumeModel() {
        populateListOfItems();
    }

    /// <summary>
    /// Retrieves a filtered and sorted resume item based on the specified search criteria.
    /// </summary>
    /// <param name="search">The search criteria used to filter and sort the resume item. Can be null.</param>
    /// <returns>A <see cref="ResumeItem"/> object that matches the specified search criteria, sorted by year. If no filtering
    /// criteria are provided or the internal data is unavailable, an empty <see cref="ResumeItem"/> is returned.</returns>
    public ResumeItem getFilteredResume(SearchItem? search) {

        var resume_item = new ResumeItem();

        if (_m_list_profit_items == null 
            || _m_list_fee_items == null)
            {
            return resume_item;
        }

        resume_item = filterBy(search);

        return sortByYear(resume_item, search);
    }

    /// <summary>
    /// Retrieves a collection of search items based on the specified database table.
    /// </summary>
    /// <param name="selected_table">The database table from which to retrieve search items. The table is identified by its name in the database.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="SearchItem"/> objects, where each item represents an
    /// entity from the specified table. The collection will be empty if the table contains no items or if the
    /// corresponding data source is null.</returns>
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

    /// <summary>
    /// Retrieves a collection of search items representing available years.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="SearchItem"/> objects, where each item represents a year
    /// retrieved from the underlying data source. If no years are available, the collection will be empty except for a
    /// default item.</returns>
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

    /// <summary>
    /// Filters profit and fee items based on the specified search criteria.
    /// </summary>
    /// <remarks>The filtering behavior depends on the <see cref="EDatabaseTableName"/> specified in the
    /// <paramref name="search"/> parameter: <list type="bullet"> <item> <description> If <see
    /// cref="EDatabaseTableName.NONE"/> is specified, all profit and fee items are returned. </description> </item>
    /// <item> <description> For other table names, filtering is applied based on the corresponding identifier in the
    /// <paramref name="search"/> parameter. </description> </item> </list> If the identifier in <paramref
    /// name="search"/> is zero, no filtering is applied for that table.</remarks>
    /// <param name="search">The search criteria used to filter the items. If <paramref name="search"/> is <see langword="null"/>, all profit
    /// and fee items are returned. If <paramref name="search"/> contains specific criteria, the filtering is applied
    /// based on the <see cref="EDatabaseTableName"/> and associated identifiers.</param>
    /// <returns>A <see cref="ResumeItem"/> containing the filtered profit and fee items. If no matching items are found or the
    /// input parameters are invalid, the returned <see cref="ResumeItem"/> will contain empty collections.</returns>
    private ResumeItem filterBy(SearchItem? search) {

        var filtered_data = new ResumeItem();

        if (_m_list_profit_items == null
            || _m_list_fee_items == null) 
            {
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
                if (search.search_id == 0) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit => 
                        profit.entity.entity_id == search.search_id
                    )
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee => 
                        fee.entity.entity_id == search.search_id
                    )
                );
                break;
            case EDatabaseTableName.PRODUCT:
                if (search.search_id == 0) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                        profit.receipt_products.Any(product =>
                            product.product_item.product_id == search.search_id
                        )
                    )
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                        fee.receipt_products.Any(product =>
                            product.product_item.product_id == search.search_id
                        )
                    )
                );
                break;
            case EDatabaseTableName.PRODUCT_TYPE:
                if (search.search_id == 0) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                        profit.receipt_products.Any(product =>
                            product.product_item.fk_product_type_id == search.search_id
                        )
                    )
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                        fee.receipt_products.Any(product =>
                            product.product_item.fk_product_type_id == search.search_id
                        )
                    )
                );
                break;
            case EDatabaseTableName.PRODUCT_CATEGORY:
                if (search.search_id == 0) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                        profit.receipt_products.Any(product =>
                            product.product_item.fk_product_category_id == search.search_id
                        )
                    )
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                        fee.receipt_products.Any(product =>
                            product.product_item.fk_product_category_id == search.search_id
                        )
                    )
                );
                break;
            case EDatabaseTableName.PRODUCT_SHAPE:
                if (search.search_id == 0) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                        profit.receipt_products.Any(product =>
                            product.product_item.fk_product_shape_id == search.search_id
                        )
                    )
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                        fee.receipt_products.Any(product =>
                            product.product_item.fk_product_shape_id == search.search_id
                        )
                    )
                );
                break;
            case EDatabaseTableName.PRODUCT_LOT:
                if (search.search_id == 0) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                        profit.receipt_products.Any(product =>
                            product.product_lot_item.product_lot_id == search.search_id
                        )
                    )
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                        fee.receipt_products.Any(product =>
                            product.product_lot_item.product_lot_id == search.search_id
                        )
                    )
                );
                break;
            case EDatabaseTableName.BEEHIVE:
                if (search.search_id == 0) {
                    break;
                }
                filtered_data.profit_items = new(
                    _m_list_profit_items.Where(profit =>
                        profit.receipt_products.Any(product =>
                            product.product_lot_item.fk_beehive_id == search.search_id
                        )
                    )
                );
                filtered_data.fee_items = new(
                    _m_list_fee_items.Where(fee =>
                        fee.receipt_products.Any(product =>
                            product.product_lot_item.fk_beehive_id == search.search_id
                        )
                    )
                );
                break;
        }

        return filtered_data;
    }

    /// <summary>
    /// Sorts the profit and fee items within a <see cref="ResumeItem"/> by year and date in descending order.
    /// </summary>
    /// <param name="data_to_sort">The <see cref="ResumeItem"/> containing the profit and fee items to be sorted.</param>
    /// <param name="search">An optional <see cref="SearchItem"/> specifying the year to filter the items by. If <c>null</c>, no filtering is
    /// applied.</param>
    /// <returns>A new <see cref="ResumeItem"/> containing the sorted profit and fee items. If both profit and fee items are
    /// empty, an empty <see cref="ResumeItem"/> is returned.</returns>
    private ResumeItem sortByYear(ResumeItem data_to_sort, SearchItem? search) {

        if (data_to_sort.profit_items.Count == 0 && data_to_sort.fee_items.Count == 0) {
            return new ResumeItem();
        }

        var query_profit = data_to_sort.profit_items.AsQueryable();
        var query_fee = data_to_sort.fee_items.AsQueryable();

        // Filter by year if specified in search
        if (search != null) {
            if (search.search_year != string.Empty) {
                string year_to_match = SFormatData.getYearFromDate(search.search_year);
                query_profit = query_profit.Where(item => SFormatData.getYearFromDate(item.receipt.receipt_date_created) == year_to_match);
                query_fee = query_fee.Where(item => SFormatData.getYearFromDate(item.receipt.receipt_date_created) == year_to_match);
            }
        }

        // Sort by parsed date for correct chronological order
        var sorted_items = new ResumeItem();

        if (data_to_sort.profit_items.Count != 0) {
            sorted_items.profit_items = new ObservableCollection<ReceiptProfitDetailedItem>(
                query_profit.OrderByDescending(item =>
                DateTime.ParseExact(item.receipt.receipt_date_created,
                globals.SGlobals.g_EU_DATE_FORMAT,
                System.Globalization.CultureInfo.InvariantCulture).Date)
            );
        }
        if (data_to_sort.fee_items.Count != 0) {

            sorted_items.fee_items = new ObservableCollection<ReceiptFeeDetailedItem>(
                query_fee.OrderByDescending(item =>
                DateTime.ParseExact(item.receipt.receipt_date_created,
                globals.SGlobals.g_EU_DATE_FORMAT,
                System.Globalization.CultureInfo.InvariantCulture).Date)
            );
        }

        return sorted_items;
    }

    /// <summary>
    /// Populates various collections with data retrieved from their respective models, filtering out archived items
    /// where applicable.
    /// </summary>
    /// <remarks>This method initializes collections for entities, products, product shapes, product
    /// categories, product types, product lots, beehives, profits, and fees. For collections that support archiving,
    /// only non-archived items are included. Archived items are identified by an empty string in their respective
    /// archive fields.</remarks>
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
