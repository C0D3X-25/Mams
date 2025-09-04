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
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
        if (_m_list_profit_items == null || _m_list_fee_items == null) {
            return new ResumeItem();
        }

        var resume_item = filterBy(search);
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
        ObservableCollection<SearchItem> list_search_item = new();

        switch (selected_table.m_name_in_database) {
            case EDatabaseTableName.ENTITY:
                if (_m_list_entity_not_archived != null) {
                    foreach (var item in _m_list_entity_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.entity_id,
                            search_item_to_display = item.entity_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.BEEHIVE:
                if (_m_list_beehive_not_archived != null) {
                    foreach (var item in _m_list_beehive_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.beehive_id,
                            search_item_to_display = item.beehive_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT:
                if (_m_list_product_not_archived != null) {
                    foreach (var item in _m_list_product_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_id,
                            search_item_to_display = item.product_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_SHAPE:
                if (_m_list_product_shape_not_archived != null) {
                    foreach (var item in _m_list_product_shape_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_shape_id,
                            search_item_to_display = item.product_shape_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_CATEGORY:
                if (_m_list_product_category_not_archived != null) {
                    foreach (var item in _m_list_product_category_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_category_id,
                            search_item_to_display = item.product_category_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_TYPE:
                if (_m_list_product_type_not_archived != null) {
                    foreach (var item in _m_list_product_type_not_archived) {
                        list_search_item.Add(new SearchItem {
                            search_id = item.product_type_id,
                            search_item_to_display = item.product_type_name
                        });
                    }
                }
                break;
            case EDatabaseTableName.PRODUCT_LOT:
                if (_m_list_product_lot_not_archived != null) {
                    foreach (var item in _m_list_product_lot_not_archived) {
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
        var years = _m_receipt_model.getExistingYear();
        
        if (years == null) {
            return new ObservableCollection<SearchItem>();
        }

        var list_search_year = new ObservableCollection<SearchItem> {
            new SearchItem {
                search_year = string.Empty,
                search_item_to_display = string.Empty
            }
        };
                
        foreach (string year in years) {
            list_search_year.Add(new SearchItem {
                search_year = year,
                search_item_to_display = string.Empty
            });
        }

        return list_search_year;
    }

    /// <summary>
    /// creates a filtered version of a receipt product item list based on a specified filter function
    /// </summary>
    private ObservableCollection<ReceiptProductItem> FilterReceiptProducts<T>(
        ObservableCollection<ReceiptProductItem> products, 
        Func<ReceiptProductItem, bool> filterFunc)
    {
        var filtered = new ObservableCollection<ReceiptProductItem>();
        
        foreach (var product in products) {
            if (filterFunc(product)) {
                filtered.Add(new ReceiptProductItem {
                    receipt_product_id = product.receipt_product_id,
                    receipt_product_quantity = product.receipt_product_quantity,
                    receipt_product_unity_price = product.receipt_product_unity_price,
                    fk_receipt_id = product.fk_receipt_id,
                    product_item = product.product_item,
                    product_lot_item = product.product_lot_item
                });
            }
        }
        
        return filtered;
    }

    /// <summary>
    /// Calculates the total price for filtered products
    /// </summary>
    private decimal CalculateTotalPrice(ObservableCollection<ReceiptProductItem> products, Func<ReceiptProductItem, bool> filterFunc)
    {
        decimal total = 0;
        
        foreach (var product in products) {
            if (filterFunc(product)) {
                total += product.receipt_product_quantity * product.receipt_product_unity_price;
            }
        }
        
        return total;
    }

    /// <summary>
    /// creates a filtered ReceiptProfitDetailedItem based on original item and filtered products
    /// </summary>
    private ReceiptProfitDetailedItem createFilteredProfitItem(
        ReceiptProfitDetailedItem original, 
        ObservableCollection<ReceiptProductItem> filteredProducts,
        decimal totalPrice)
    {
        return new ReceiptProfitDetailedItem {
            receipt = new ReceiptItem {
                receipt_id = original.receipt.receipt_id,
                receipt_number = original.receipt.receipt_number,
                receipt_date_created = original.receipt.receipt_date_created,
                receipt_total_price = totalPrice
            },
            entity = original.entity,
            client = original.client,
            receipt_client = original.receipt_client,
            receipt_products = filteredProducts
        };
    }

    /// <summary>
    /// creates a filtered ReceiptFeeDetailedItem based on original item and filtered products
    /// </summary>
    private ReceiptFeeDetailedItem createFilteredFeeItem(
        ReceiptFeeDetailedItem original, 
        ObservableCollection<ReceiptProductItem> filteredProducts,
        decimal totalPrice)
    {
        return new ReceiptFeeDetailedItem {
            receipt = new ReceiptItem {
                receipt_id = original.receipt.receipt_id,
                receipt_number = original.receipt.receipt_number,
                receipt_date_created = original.receipt.receipt_date_created,
                receipt_total_price = totalPrice
            },
            entity = original.entity,
            supplier = original.supplier,
            receipt_supplier = original.receipt_supplier,
            receipt_products = filteredProducts
        };
    }

    /// <summary>
    /// Filters profit and fee items based on the specified search criteria.
    /// </summary>
    private ResumeItem filterBy(SearchItem? search) {
        var filtered_data = new ResumeItem();

        if (_m_list_profit_items == null || _m_list_fee_items == null) {
            return filtered_data;
        }
        
        if (search == null) {
            filtered_data.profit_items = _m_list_profit_items;
            filtered_data.fee_items = _m_list_fee_items;
            return filtered_data;
        }
        
        if (search.search_id == 0 && search.search_table != EDatabaseTableName.NONE) {
            return filtered_data;
        }

        switch (search.search_table) {
            case EDatabaseTableName.NONE:
                filtered_data.profit_items = _m_list_profit_items;
                filtered_data.fee_items = _m_list_fee_items;
                break;
            case EDatabaseTableName.ENTITY:
                filtered_data.profit_items = filterEntityItems(_m_list_profit_items, search.search_id);
                filtered_data.fee_items = filterEntityItems(_m_list_fee_items, search.search_id);
                break;
            case EDatabaseTableName.PRODUCT:
                filtered_data.profit_items = filterProductItems(_m_list_profit_items, item => item.product_id == search.search_id);
                filtered_data.fee_items = filterProductItems(_m_list_fee_items, item => item.product_id == search.search_id);
                break;
            case EDatabaseTableName.PRODUCT_TYPE:
                filtered_data.profit_items = filterProductItems(_m_list_profit_items, item => item.fk_product_type_id == search.search_id);
                filtered_data.fee_items = filterProductItems(_m_list_fee_items, item => item.fk_product_type_id == search.search_id);
                break;
            case EDatabaseTableName.PRODUCT_CATEGORY:
                filtered_data.profit_items = filterProductItems(_m_list_profit_items, item => item.fk_product_category_id == search.search_id);
                filtered_data.fee_items = filterProductItems(_m_list_fee_items, item => item.fk_product_category_id == search.search_id);
                break;
            case EDatabaseTableName.PRODUCT_SHAPE:
                filtered_data.profit_items = filterProductItems(_m_list_profit_items, item => item.fk_product_shape_id == search.search_id);
                filtered_data.fee_items = filterProductItems(_m_list_fee_items, item => item.fk_product_shape_id == search.search_id);
                break;
            case EDatabaseTableName.PRODUCT_LOT:
                filtered_data.profit_items = filterLotItems(_m_list_profit_items, item => item.product_lot_id == search.search_id);
                filtered_data.fee_items = filterLotItems(_m_list_fee_items, item => item.product_lot_id == search.search_id);
                break;
            case EDatabaseTableName.BEEHIVE:
                filtered_data.profit_items = filterLotItems(_m_list_profit_items, item => item.fk_beehive_id == search.search_id);
                filtered_data.fee_items = filterLotItems(_m_list_fee_items, item => item.fk_beehive_id == search.search_id);
                break;
        }

        return filtered_data;
    }

    // Helper methods for filtering different types of items
    private ObservableCollection<ReceiptProfitDetailedItem> filterEntityItems(
        ObservableCollection<ReceiptProfitDetailedItem> items, long entityId)
    {
        var result = new ObservableCollection<ReceiptProfitDetailedItem>();
        
        foreach (var item in items) {
            if (item.entity.entity_id == entityId) {
                result.Add(item);
            }
        }
        
        return result;
    }

    private ObservableCollection<ReceiptFeeDetailedItem> filterEntityItems(
        ObservableCollection<ReceiptFeeDetailedItem> items, long entityId)
    {
        var result = new ObservableCollection<ReceiptFeeDetailedItem>();
        
        foreach (var item in items) {
            if (item.entity.entity_id == entityId) {
                result.Add(item);
            }
        }
        
        return result;
    }

    private ObservableCollection<ReceiptProfitDetailedItem> filterProductItems(
        ObservableCollection<ReceiptProfitDetailedItem> items, 
        Func<ProductItem, bool> productFilter)
    {
        var result = new ObservableCollection<ReceiptProfitDetailedItem>();
        
        foreach (var profit in items) {
            bool hasMatchingProducts = false;
            decimal totalPrice = 0;
            var filteredProducts = new ObservableCollection<ReceiptProductItem>();
            
            foreach (var product in profit.receipt_products) {
                if (productFilter(product.product_item)) {
                    hasMatchingProducts = true;
                    totalPrice += product.receipt_product_quantity * product.receipt_product_unity_price;
                    filteredProducts.Add(new ReceiptProductItem {
                        receipt_product_id = product.receipt_product_id,
                        receipt_product_quantity = product.receipt_product_quantity,
                        receipt_product_unity_price = product.receipt_product_unity_price,
                        fk_receipt_id = product.fk_receipt_id,
                        product_item = product.product_item,
                        product_lot_item = product.product_lot_item
                    });
                }
            }
            
            if (hasMatchingProducts) {
                result.Add(createFilteredProfitItem(profit, filteredProducts, totalPrice));
            }
        }
        
        return result;
    }

    private ObservableCollection<ReceiptFeeDetailedItem> filterProductItems(
        ObservableCollection<ReceiptFeeDetailedItem> items, 
        Func<ProductItem, bool> productFilter)
    {
        var result = new ObservableCollection<ReceiptFeeDetailedItem>();
        
        foreach (var fee in items) {
            bool hasMatchingProducts = false;
            decimal totalPrice = 0;
            var filteredProducts = new ObservableCollection<ReceiptProductItem>();
            
            foreach (var product in fee.receipt_products) {
                if (productFilter(product.product_item)) {
                    hasMatchingProducts = true;
                    totalPrice += product.receipt_product_quantity * product.receipt_product_unity_price;
                    filteredProducts.Add(new ReceiptProductItem {
                        receipt_product_id = product.receipt_product_id,
                        receipt_product_quantity = product.receipt_product_quantity,
                        receipt_product_unity_price = product.receipt_product_unity_price,
                        fk_receipt_id = product.fk_receipt_id,
                        product_item = product.product_item,
                        product_lot_item = product.product_lot_item
                    });
                }
            }
            
            if (hasMatchingProducts) {
                result.Add(createFilteredFeeItem(fee, filteredProducts, totalPrice));
            }
        }
        
        return result;
    }

    private ObservableCollection<ReceiptProfitDetailedItem> filterLotItems(
        ObservableCollection<ReceiptProfitDetailedItem> items, 
        Func<ProductLotItem, bool> lotFilter)
    {
        var result = new ObservableCollection<ReceiptProfitDetailedItem>();
        
        foreach (var profit in items) {
            bool hasMatchingProducts = false;
            decimal totalPrice = 0;
            var filteredProducts = new ObservableCollection<ReceiptProductItem>();
            
            foreach (var product in profit.receipt_products) {
                if (lotFilter(product.product_lot_item)) {
                    hasMatchingProducts = true;
                    totalPrice += product.receipt_product_quantity * product.receipt_product_unity_price;
                    filteredProducts.Add(new ReceiptProductItem {
                        receipt_product_id = product.receipt_product_id,
                        receipt_product_quantity = product.receipt_product_quantity,
                        receipt_product_unity_price = product.receipt_product_unity_price,
                        fk_receipt_id = product.fk_receipt_id,
                        product_item = product.product_item,
                        product_lot_item = product.product_lot_item
                    });
                }
            }
            
            if (hasMatchingProducts) {
                result.Add(createFilteredProfitItem(profit, filteredProducts, totalPrice));
            }
        }
        
        return result;
    }

    private ObservableCollection<ReceiptFeeDetailedItem> filterLotItems(
        ObservableCollection<ReceiptFeeDetailedItem> items, 
        Func<ProductLotItem, bool> lotFilter)
    {
        var result = new ObservableCollection<ReceiptFeeDetailedItem>();
        
        foreach (var fee in items) {
            bool hasMatchingProducts = false;
            decimal totalPrice = 0;
            var filteredProducts = new ObservableCollection<ReceiptProductItem>();
            
            foreach (var product in fee.receipt_products) {
                if (lotFilter(product.product_lot_item)) {
                    hasMatchingProducts = true;
                    totalPrice += product.receipt_product_quantity * product.receipt_product_unity_price;
                    filteredProducts.Add(new ReceiptProductItem {
                        receipt_product_id = product.receipt_product_id,
                        receipt_product_quantity = product.receipt_product_quantity,
                        receipt_product_unity_price = product.receipt_product_unity_price,
                        fk_receipt_id = product.fk_receipt_id,
                        product_item = product.product_item,
                        product_lot_item = product.product_lot_item
                    });
                }
            }
            
            if (hasMatchingProducts) {
                result.Add(createFilteredFeeItem(fee, filteredProducts, totalPrice));
            }
        }
        
        return result;
    }

    /// <summary>
    /// Sorts the profit and fee items within a <see cref="ResumeItem"/> by year and date in descending order.
    /// </summary>
    private ResumeItem sortByYear(ResumeItem data_to_sort, SearchItem? search) {
        if (data_to_sort.profit_items.Count == 0 && data_to_sort.fee_items.Count == 0) {
            return new ResumeItem();
        }

        var sorted_items = new ResumeItem();
        string yearFilter = search?.search_year ?? string.Empty;
        
        // Pre-parse dates to avoid repetitive parsing
        Dictionary<string, DateTime> dateCache = new Dictionary<string, DateTime>();

        if (data_to_sort.profit_items.Count > 0) {
            var profitList = new List<ReceiptProfitDetailedItem>(data_to_sort.profit_items.Count);
            
            foreach (var item in data_to_sort.profit_items) {
                // Only add items matching year filter if filter is set
                if (yearFilter != string.Empty) {
                    string itemYear = SFormatData.getYearFromDate(item.receipt.receipt_date_created);
                    if (itemYear != SFormatData.getYearFromDate(yearFilter)) {
                        continue;
                    }
                }
                profitList.Add(item);
            }

            // Sort by date
            profitList.Sort((a, b) => {
                if (!dateCache.TryGetValue(a.receipt.receipt_date_created, out DateTime dateA)) {
                    dateA = DateTime.ParseExact(a.receipt.receipt_date_created,
                        globals.SGlobals.g_EU_DATE_FORMAT,
                        System.Globalization.CultureInfo.InvariantCulture);
                    dateCache[a.receipt.receipt_date_created] = dateA;
                }
                
                if (!dateCache.TryGetValue(b.receipt.receipt_date_created, out DateTime dateB)) {
                    dateB = DateTime.ParseExact(b.receipt.receipt_date_created,
                        globals.SGlobals.g_EU_DATE_FORMAT,
                        System.Globalization.CultureInfo.InvariantCulture);
                    dateCache[b.receipt.receipt_date_created] = dateB;
                }
                
                return dateB.CompareTo(dateA); // Descending order
            });
            
            sorted_items.profit_items = new ObservableCollection<ReceiptProfitDetailedItem>(profitList);
        }

        if (data_to_sort.fee_items.Count > 0) {
            var feeList = new List<ReceiptFeeDetailedItem>(data_to_sort.fee_items.Count);
            
            foreach (var item in data_to_sort.fee_items) {
                // Only add items matching year filter if filter is set
                if (yearFilter != string.Empty) {
                    string itemYear = SFormatData.getYearFromDate(item.receipt.receipt_date_created);
                    if (itemYear != SFormatData.getYearFromDate(yearFilter)) {
                        continue;
                    }
                }
                feeList.Add(item);
            }

            // Sort by date
            feeList.Sort((a, b) => {
                if (!dateCache.TryGetValue(a.receipt.receipt_date_created, out DateTime dateA)) {
                    dateA = DateTime.ParseExact(a.receipt.receipt_date_created,
                        globals.SGlobals.g_EU_DATE_FORMAT,
                        System.Globalization.CultureInfo.InvariantCulture);
                    dateCache[a.receipt.receipt_date_created] = dateA;
                }
                
                if (!dateCache.TryGetValue(b.receipt.receipt_date_created, out DateTime dateB)) {
                    dateB = DateTime.ParseExact(b.receipt.receipt_date_created,
                        globals.SGlobals.g_EU_DATE_FORMAT,
                        System.Globalization.CultureInfo.InvariantCulture);
                    dateCache[b.receipt.receipt_date_created] = dateB;
                }
                
                return dateB.CompareTo(dateA); // Descending order
            });
            
            sorted_items.fee_items = new ObservableCollection<ReceiptFeeDetailedItem>(feeList);
        }

        return sorted_items;
    }

    /// <summary>
    /// Populates various collections with data retrieved from their respective models, filtering out archived items
    /// where applicable.
    /// </summary>
    private void populateListOfItems() {
        var tasks = new List<Task>(9);

        // Entity
        tasks.Add(Task.Run(() => {
            _m_list_entity_all = _m_entity_model.getTable();
            if (_m_list_entity_all != null) {
                _m_list_entity_not_archived = new(_m_list_entity_all.Where(item => item.entity_archive == string.Empty));
            }
        }));

        // Product 
        tasks.Add(Task.Run(() => {
            _m_list_product_all = _m_product_model.getTable();
            if (_m_list_product_all != null) {
                _m_list_product_not_archived = new(_m_list_product_all.Where(item => item.product_archive == string.Empty));
            }
        }));

        // Product Shape
        tasks.Add(Task.Run(() => {
            _m_list_product_shape_all = _m_product_shape_model.getTable();
            if (_m_list_product_shape_all != null) {
                _m_list_product_shape_not_archived = new(_m_list_product_shape_all.Where(item => 
                    item.product_shape_archive == string.Empty));
            }
        }));

        // Product Category
        tasks.Add(Task.Run(() => {
            _m_list_product_category_all = _m_product_category_model.getTable();
            if (_m_list_product_category_all != null) {
                _m_list_product_category_not_archived = new(_m_list_product_category_all.Where(item => 
                    item.product_category_archive == string.Empty));
            }
        }));

        // Product Type
        tasks.Add(Task.Run(() => {
            _m_list_product_type_all = _m_product_type_model.getTable();
            if (_m_list_product_type_all != null) {
                _m_list_product_type_not_archived = new(_m_list_product_type_all.Where(item => 
                    item.product_type_archive == string.Empty));
            }
        }));

        // Product Lot
        tasks.Add(Task.Run(() => {
            _m_list_product_lot_all = _m_product_lot_model.getTable();
            if (_m_list_product_lot_all != null) {
                _m_list_product_lot_not_archived = new(_m_list_product_lot_all.Where(item => 
                    item.product_lot_archive == string.Empty));
            }
        }));

        // Beehive
        tasks.Add(Task.Run(() => {
            _m_list_beehive_all = _m_beehive_model.getTable();
            if (_m_list_beehive_all != null) {
                _m_list_beehive_not_archived = new(_m_list_beehive_all.Where(item => 
                    item.beehive_archive == string.Empty));
            }
        }));

        // Profit
        tasks.Add(Task.Run(() => {
            _m_list_profit_items = _m_profit_model.getTable();
        }));

        // Fee
        tasks.Add(Task.Run(() => {
            _m_list_fee_items = _m_fee_model.getTable();
        }));

        // Wait for all tasks to complete
        Task.WaitAll(tasks.ToArray());
    }
}
