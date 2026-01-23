using Mams_App.src.beehives;
using Mams_App.src.databaseOperations;
using Mams_App.src.entities;
using Mams_App.src.fees;
using Mams_App.src.filters;
using Mams_App.src.helpers;
using Mams_App.src.localizations;
using Mams_App.src.products;
using Mams_App.src.productsCategories;
using Mams_App.src.productsLots;
using Mams_App.src.productsShapes;
using Mams_App.src.productsTypes;
using Mams_App.src.profits;
using Mams_App.src.receipts;
using System.Collections.ObjectModel;

namespace Mams_App.src.resumes;

/// <summary>
/// This class is mainly used to manage the filter of the page Resume.
/// Uses direct MySQL queries for filtering and sorting for optimal performance.
/// </summary>
public class ResumeModel
{

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

    // Thread-safe lazy-loaded dropdown data using Lazy<T>
    // Each lazy loader calls the model's getActive* method which filters at database level
    private readonly Lazy<ObservableCollection<EntityItem>> _m_lazy_entity_not_archived;
    private readonly Lazy<ObservableCollection<ProductItem>> _m_lazy_product_not_archived;
    private readonly Lazy<ObservableCollection<ProductShapeItem>> _m_lazy_product_shape_not_archived;
    private readonly Lazy<ObservableCollection<ProductCategoryItem>> _m_lazy_product_category_not_archived;
    private readonly Lazy<ObservableCollection<ProductTypeItem>> _m_lazy_product_type_not_archived;
    private readonly Lazy<ObservableCollection<ProductLotItem>> _m_lazy_product_lot_not_archived;
    private readonly Lazy<ObservableCollection<BeehiveItem>> _m_lazy_beehive_not_archived;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResumeModel"/> class with thread-safe lazy loading for dropdown data.
    /// </summary>
    public ResumeModel()
    {
        // Initialize lazy loaders with thread-safe mode
        // Each loader fetches non-archived items from the database on first access using optimized queries
        _m_lazy_entity_not_archived = new Lazy<ObservableCollection<EntityItem>>(
            () => _m_entity_model.getActiveEntities(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        _m_lazy_product_not_archived = new Lazy<ObservableCollection<ProductItem>>(
            () => _m_product_model.getActiveProducts(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        _m_lazy_product_shape_not_archived = new Lazy<ObservableCollection<ProductShapeItem>>(
            () => _m_product_shape_model.getActiveProductShapes(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        _m_lazy_product_category_not_archived = new Lazy<ObservableCollection<ProductCategoryItem>>(
            () => _m_product_category_model.getActiveProductCategories(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        _m_lazy_product_type_not_archived = new Lazy<ObservableCollection<ProductTypeItem>>(
            () => _m_product_type_model.getActiveProductTypes(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        _m_lazy_product_lot_not_archived = new Lazy<ObservableCollection<ProductLotItem>>(
            () => _m_product_lot_model.getActiveProductLots(),
            LazyThreadSafetyMode.ExecutionAndPublication);

        _m_lazy_beehive_not_archived = new Lazy<ObservableCollection<BeehiveItem>>(
            () => _m_beehive_model.getActiveBeehives(),
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <summary>
    /// Represents a predefined collection of database table names and their corresponding display names.
    /// </summary>
    public readonly ObservableCollection<DatabaseTablesNameItem> m_filter_tables = [
        new(){ m_name_in_database = EDatabaseTableName.NONE, m_name_to_display = string.Empty }, // Filter all
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_CATEGORY, m_name_to_display = Loc.Get("Search.Category") },
        new(){ m_name_in_database = EDatabaseTableName.ENTITY, m_name_to_display = Loc.Get("Search.ClientSupplier") },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_SHAPE, m_name_to_display = Loc.Get("Search.Shape") },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_LOT, m_name_to_display = Loc.Get("Search.Lot") },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT, m_name_to_display = Loc.Get("Search.Product") },
        new(){ m_name_in_database = EDatabaseTableName.BEEHIVE, m_name_to_display = Loc.Get("Search.Beehive") },
        new(){ m_name_in_database = EDatabaseTableName.PRODUCT_TYPE, m_name_to_display = Loc.Get("Search.Type") }
    ];

    /// <summary>
    /// Retrieves a filtered and sorted resume item based on the specified filter criteria.
    /// Filtering and sorting are performed directly in SQL for optimal performance.
    /// </summary>
    /// <param name="filter">The filter criteria used to filter and sort the resume item. Can be null.</param>
    /// <returns>A <see cref="ResumeItem"/> object that matches the specified filter criteria, sorted by date descending.</returns>
    public ResumeItem getFilteredResume(FilterItem? filter)
    {
        var resume_item = new ResumeItem();

        // Extract filter parameters
        var filterTable = filter?.filter_table ?? EDatabaseTableName.NONE;
        var filterId = filter?.filter_id ?? 0;
        var yearFilter = extractYear(filter?.filter_year);

        // Return empty if filter is selected but no ID provided (except for NONE which means no filter)
        if (filterId == 0 && filterTable != EDatabaseTableName.NONE)
        {
            return resume_item;
        }

        // Query profits with filtering and sorting at database level
        var profitResult = _m_profit_model.getFilteredItems(filterTable, filterId, yearFilter);
        if (profitResult.is_success && profitResult.returned_items != null)
        {
            resume_item.profit_items = profitResult.returned_items;
        }

        // Query fees with filtering and sorting at database level
        var feeResult = _m_fee_model.getFilteredItems(filterTable, filterId, yearFilter);
        if (feeResult.is_success && feeResult.returned_items != null)
        {
            resume_item.fee_items = feeResult.returned_items;
        }

        return resume_item;
    }

    /// <summary>
    /// Extracts the year from a date string in EU format (dd.MM.yyyy).
    /// </summary>
    /// <param name="dateString">The date string to extract year from.</param>
    /// <returns>The year as a string, or empty string if invalid.</returns>
    private static string extractYear(string? dateString)
    {
        if (string.IsNullOrEmpty(dateString))
        {
            return string.Empty;
        }
        return SFormatData.getYearFromDate(dateString);
    }

    /// <summary>
    /// Retrieves a collection of filter items based on the specified database table.
    /// </summary>
    /// <param name="selected_table">The database table from which to retrieve filter items.</param>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="FilterItem"/> objects.</returns>
    public ObservableCollection<FilterItem> getListFilterItems(DatabaseTablesNameItem selected_table)
    {

        ObservableCollection<FilterItem> list_filter_item = [];

        switch (selected_table.m_name_in_database)
        {
            case EDatabaseTableName.ENTITY:
                foreach (var item in _m_lazy_entity_not_archived.Value)
                {
                    list_filter_item.Add(new FilterItem
                    {
                        filter_id = item.entity_id,
                        filter_item_to_display = item.entity_name
                    });
                }
                break;
            case EDatabaseTableName.BEEHIVE:
                foreach (var item in _m_lazy_beehive_not_archived.Value)
                {
                    list_filter_item.Add(new FilterItem
                    {
                        filter_id = item.beehive_id,
                        filter_item_to_display = item.beehive_name
                    });
                }
                break;
            case EDatabaseTableName.PRODUCT:
                foreach (var item in _m_lazy_product_not_archived.Value)
                {
                    list_filter_item.Add(new FilterItem
                    {
                        filter_id = item.product_id,
                        filter_item_to_display = item.product_name
                    });
                }
                break;
            case EDatabaseTableName.PRODUCT_SHAPE:
                foreach (var item in _m_lazy_product_shape_not_archived.Value)
                {
                    list_filter_item.Add(new FilterItem
                    {
                        filter_id = item.product_shape_id,
                        filter_item_to_display = item.product_shape_name
                    });
                }
                break;
            case EDatabaseTableName.PRODUCT_CATEGORY:
                foreach (var item in _m_lazy_product_category_not_archived.Value)
                {
                    list_filter_item.Add(new FilterItem
                    {
                        filter_id = item.product_category_id,
                        filter_item_to_display = item.product_category_name
                    });
                }
                break;
            case EDatabaseTableName.PRODUCT_TYPE:
                foreach (var item in _m_lazy_product_type_not_archived.Value)
                {
                    list_filter_item.Add(new FilterItem
                    {
                        filter_id = item.product_type_id,
                        filter_item_to_display = item.product_type_name
                    });
                }
                break;
            case EDatabaseTableName.PRODUCT_LOT:
                foreach (var item in _m_lazy_product_lot_not_archived.Value)
                {
                    list_filter_item.Add(new FilterItem
                    {
                        filter_id = item.product_lot_id,
                        filter_item_to_display = item.product_lot_name
                    });
                }
                break;
        }

        return list_filter_item;
    }

    /// <summary>
    /// Retrieves a collection of filter items representing available years.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> of <see cref="FilterItem"/> objects.</returns>
    public ObservableCollection<FilterItem> getListYears()
    {
        var years = _m_receipt_model.getExistingYear();

        if (years == null)
        {
            return [];
        }

        var list_filter_year = new ObservableCollection<FilterItem> {
            new() {
                filter_year = string.Empty,
                filter_item_to_display = string.Empty
            }
        };

        foreach (string year in years)
        {
            list_filter_year.Add(new FilterItem
            {
                filter_year = year,
                filter_item_to_display = string.Empty
            });
        }

        return list_filter_year;
    }

    /// <summary>
    /// Calculates total transactions values from profit and fee items.
    /// </summary>
    /// <param name="profit_items">Collection of profit items to calculate from.</param>
    /// <param name="fee_items">Collection of fee items to calculate from.</param>
    /// <returns>Tuple containing (total_profit, total_fee, total).</returns>
    public (decimal total_profit, decimal total_fee, decimal total) calculateTotalTransactions(
        ObservableCollection<ReceiptProfitDetailedItem>? profit_items,
        ObservableCollection<ReceiptFeeDetailedItem>? fee_items)
    {
        decimal total_profit = 0.00M;
        decimal total_fee = 0.00M;

        if (profit_items != null)
        {
            foreach (var item in profit_items)
            {
                total_profit += item.receipt.receipt_total_price;
            }
        }

        if (fee_items != null)
        {
            foreach (var item in fee_items)
            {
                total_fee -= item.receipt.receipt_total_price;
            }
        }

        decimal total = total_profit + total_fee;
        return (total_profit, total_fee, total);
    }

    /// <summary>
    /// Calculates detailed transaction metrics from profit items.
    /// </summary>
    /// <param name="profit_items">Collection of profit items to calculate from.</param>
    /// <param name="filter_item">Current filter item for validation.</param>
    /// <param name="total_profit">Total profit value for average calculation.</param>
    /// <param name="includeFreeItems">If true, includes items with price of 0 (gifts/free) in average calculations.</param>
    /// <returns>Tuple containing (total_weight_kg, total_quantity, average_price_per_unit, average_price_per_weight).</returns>
    public (decimal total_weight_kg, int total_quantity, decimal average_price_per_unit, decimal average_price_per_weight) calculateDetailTransactions(
        ObservableCollection<ReceiptProfitDetailedItem>? profit_items,
        FilterItem? filter_item,
        decimal total_profit,
        bool includeFreeItems = false)
    {
        // Reset values for invalid filter criteria
        if (filter_item == null || filter_item.filter_id == 0)
        {
            return (0.00M, 0, 0.00M, 0.00M);
        }

        decimal total_weight_g = 0.00M;
        int total_quantity = 0;
        decimal counted_weight_g = 0.00M;
        int counted_quantity = 0;

        if (profit_items != null)
        {
            foreach (var profit_item in profit_items)
            {
                foreach (var receipt_product in profit_item.receipt_products)
                {
                    if (includeFreeItems || receipt_product.receipt_product_unity_price > 0)
                    {
                        counted_quantity += receipt_product.receipt_product_quantity;
                        counted_weight_g += receipt_product.product_item.product_weight * receipt_product.receipt_product_quantity;
                    }
                    total_quantity += receipt_product.receipt_product_quantity;
                    total_weight_g += receipt_product.product_item.product_weight * receipt_product.receipt_product_quantity;
                }
            }
        }

        decimal average_price_per_unit = counted_quantity > 0 ? total_profit / counted_quantity : 0.00M;
        decimal average_price_per_weight = counted_weight_g > 0 ? total_profit / (counted_weight_g / 1000) : 0.00M;

        return (total_weight_g / 1000, total_quantity, average_price_per_unit, average_price_per_weight);
    }
}
