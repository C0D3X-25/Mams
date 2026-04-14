using Mams_App.src.beehives;
using Mams_App.src.commands;
using Mams_App.src.controllers;
using Mams_App.src.databaseOperations;
using Mams_App.src.filters;
using Mams_App.src.helpers;
using Mams_App.src.invoices;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.products;
using Mams_App.src.doseUnits;
using Mams_App.src.regions;
using Mams_App.src.treatmentStocks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.treatments;

/// <summary>
/// Controller for the treatment list page that displays and manages treatments with filtering.
/// </summary>
public class ListTreatmentController : ABaseController
{
    public ICommand m_add_command { get; set; }
    public ICommand m_modify_command { get; set; }
    public ICommand m_delete_command { get; set; }
    public ICommand m_double_click_command { get; set; }
    public ICommand m_sort_command { get; set; }
    public ICommand m_generate_pdf_command { get; set; }
    public ICommand m_clear_search_command { get; set; }

    public string m_delete_button_text => Loc.Get("Common.Delete");

    private readonly TreatmentModel _m_treatment_model = new();
    private readonly BeehiveModel _m_beehive_model = new();
    private readonly ProductModel _m_product_model = new();
    private readonly RegionModel _m_region_model = new();
    private readonly DoseUnitModel _m_dose_unit_model = new();
    private readonly TreatmentStockModel _m_treatment_stock_model = new();

    // Filter tables
    private ObservableCollection<DatabaseTablesNameItem> _m_list_table;
    public ObservableCollection<DatabaseTablesNameItem> m_list_table
    {
        get => _m_list_table;
        set
        {
            _m_list_table = value;
            onPropertyChanged();
        }
    }

    private DatabaseTablesNameItem? _m_selected_table;
    public DatabaseTablesNameItem? m_selected_table
    {
        get => _m_selected_table;
        set
        {
            if (_m_selected_table != value)
            {
                _m_selected_table = value;
                updateListFilterItems();
                updateDisplayedItems();
                onPropertyChanged();
                onPropertyChanged(nameof(m_is_filter_item_enabled));
            }
        }
    }

    public bool m_is_filter_item_enabled
    {
        get
        {
            if (_m_list_table == null || _m_list_table.Count == 0)
                return false;
            return _m_selected_table != _m_list_table[0];
        }
    }

    private ObservableCollection<FilterItem>? _m_list_filter_item;
    public ObservableCollection<FilterItem>? m_list_filter_item
    {
        get => _m_list_filter_item;
        set
        {
            _m_list_filter_item = value;
            onPropertyChanged();
        }
    }

    private FilterItem? _m_selected_filter_item;
    public FilterItem? m_selected_filter_item
    {
        get => _m_selected_filter_item;
        set
        {
            if (_m_selected_filter_item != value)
            {
                _m_selected_filter_item = value;
                updateDisplayedItems();
                onPropertyChanged();
            }
        }
    }

    private ObservableCollection<FilterItem>? _m_list_year;
    public ObservableCollection<FilterItem>? m_list_year
    {
        get => _m_list_year;
        set
        {
            _m_list_year = value;
            onPropertyChanged();
        }
    }

    private FilterItem? _m_selected_year;
    public FilterItem? m_selected_year
    {
        get => _m_selected_year;
        set
        {
            if (_m_selected_year != value)
            {
                _m_selected_year = value;
                updateDisplayedItems();
                onPropertyChanged();
            }
        }
    }

    private ObservableCollection<TreatmentItem>? _m_list_items;
    public ObservableCollection<TreatmentItem>? m_list_items
    {
        get => _m_list_items;
        set
        {
            _m_list_items = value;
            onPropertyChanged();
        }
    }

    private TreatmentItem? _m_selected_item;
    public TreatmentItem? m_selected_item
    {
        get => _m_selected_item;
        set
        {
            _m_selected_item = value;
            onPropertyChanged();
        }
    }

    private string _m_sorted_column = string.Empty;
    public string m_sorted_column
    {
        get { return _m_sorted_column; }
        set
        {
            _m_sorted_column = value;
            onPropertyChanged();
        }
    }

    private ListSortDirection _m_sort_direction = ListSortDirection.Ascending;
    public ListSortDirection m_sort_direction
    {
        get { return _m_sort_direction; }
        set
        {
            _m_sort_direction = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Initializes a new instance of the ListTreatmentController class.
    /// </summary>
    public ListTreatmentController()
    {
        _m_list_table =
        [
            new(){ m_name_in_database = EDatabaseTableName.NONE, m_name_to_display = string.Empty },
            new(){ m_name_in_database = EDatabaseTableName.BEEHIVE, m_name_to_display = Loc.Get("Search.Beehive") },
            new(){ m_name_in_database = EDatabaseTableName.PRODUCT, m_name_to_display = Loc.Get("Search.Product") },
            new(){ m_name_in_database = EDatabaseTableName.REGION, m_name_to_display = Loc.Get("Search.Region") },
            new(){ m_name_in_database = EDatabaseTableName.DOSE_UNIT, m_name_to_display = Loc.Get("Search.DoseUnit") },
            new(){ m_name_in_database = EDatabaseTableName.TREATMENT_STOCK, m_name_to_display = Loc.Get("Search.TreatmentStock") }
        ];

        _m_selected_table = _m_list_table[0];

        loadYearFilter();
        updateDisplayedItems();

        m_add_command = new RelayCommand(navigateToSavePage);
        m_modify_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_delete_command = new RelayCommand(deleteItem, isItemSelected);
        m_double_click_command = new RelayCommand(navigateToModifyPage, isItemSelected);
        m_sort_command = new RelayCommand(sortByColumn);
        m_generate_pdf_command = new RelayCommand(generatePdf);
        m_clear_search_command = new RelayCommand(clearSelectedItems);
    }

    /// <summary>
    /// Sorts the list by the specified column.
    /// </summary>
    private void sortByColumn(object? parameter)
    {
        var result = SortHelper.sortByColumn(parameter, m_list_items, _m_sorted_column, _m_sort_direction);
        if (result.HasValue)
        {
            m_sorted_column = result.Value.ColumnName;
            m_sort_direction = result.Value.Direction;
        }
    }

    /// <summary>
    /// Loads the year filter from distinct treatment years.
    /// </summary>
    private void loadYearFilter()
    {
        var years = _m_treatment_model.getDistinctYears();
        _m_list_year = [new FilterItem { filter_year = string.Empty }];
        foreach (var year in years)
        {
            _m_list_year.Add(new FilterItem { filter_year = year });
        }
        _m_selected_year = _m_list_year[0];
    }

    /// <summary>
    /// Updates the displayed treatment list based on current filter selections.
    /// </summary>
    private void updateDisplayedItems()
    {
        var filterItem = _m_selected_filter_item ?? new FilterItem();
        var tableItem = _m_selected_table ?? new DatabaseTablesNameItem();
        var yearItem = _m_selected_year ?? new FilterItem();

        var filterTable = tableItem.m_name_in_database;
        var filterId = filterItem.filter_id;
        var yearFilter = string.IsNullOrEmpty(yearItem.filter_year) ? string.Empty : SFormatData.getYearFromDate(yearItem.filter_year);

        if (filterId == 0 && filterTable != EDatabaseTableName.NONE)
        {
            m_list_items = [];
            return;
        }

        var result = _m_treatment_model.getFilteredItems(filterTable, filterId, yearFilter);
        if (result.is_success && result.returned_items != null)
        {
            m_list_items = result.returned_items;
        }
        else
        {
            m_list_items = [];
        }
    }

    /// <summary>
    /// Updates the list of filter items based on the selected table.
    /// </summary>
    private void updateListFilterItems()
    {
        if (_m_selected_table == null) return;

        ObservableCollection<FilterItem> list = [];

        switch (_m_selected_table.m_name_in_database)
        {
            case EDatabaseTableName.BEEHIVE:
                foreach (var item in _m_beehive_model.getActiveBeehives())
                {
                    list.Add(new FilterItem
                    {
                        filter_id = item.beehive_id,
                        filter_item_to_display = item.beehive_name
                    });
                }
                break;
            case EDatabaseTableName.PRODUCT:
                foreach (var item in _m_product_model.getActiveProducts())
                {
                    list.Add(new FilterItem
                    {
                        filter_id = item.product_id,
                        filter_item_to_display = item.product_name
                    });
                }
                break;
            case EDatabaseTableName.REGION:
                foreach (var item in _m_region_model.getActiveRegions())
                {
                    list.Add(new FilterItem
                    {
                        filter_id = item.region_id,
                        filter_item_to_display = item.region_name
                    });
                }
                break;
            case EDatabaseTableName.DOSE_UNIT:
                foreach (var item in _m_dose_unit_model.getActiveDoseUnits())
                {
                    list.Add(new FilterItem
                    {
                        filter_id = item.dose_unit_id,
                        filter_item_to_display = item.dose_unit_name
                    });
                }
                break;
            case EDatabaseTableName.TREATMENT_STOCK:
                var stockResult = _m_treatment_stock_model.getAllItems();
                if (stockResult.is_success && stockResult.returned_items != null)
                {
                    foreach (var item in stockResult.returned_items)
                    {
                        list.Add(new FilterItem
                        {
                            filter_id = item.treatment_stock_id,
                            filter_item_to_display = item.display_name
                        });
                    }
                }
                break;
            default:
                break;
        }

        m_list_filter_item = list;
        if (list.Count > 0)
        {
            m_selected_filter_item = list[0];
        }
    }

    /// <summary>
    /// Clears all selected filter items and resets to default selections.
    /// </summary>
    private void clearSelectedItems(object? obj)
    {
        if (_m_list_table?.Count > 0)
        {
            m_selected_table = _m_list_table[0];
        }

        if (_m_list_filter_item?.Count > 0)
        {
            m_selected_filter_item = _m_list_filter_item[0];
        }

        if (_m_list_year?.Count > 0)
        {
            m_selected_year = _m_list_year[0];
        }

        updateDisplayedItems();
    }

    private bool isItemSelected(object? arg) => _m_selected_item != null;

    /// <summary>
    /// Navigates to the save page for creating a new treatment.
    /// </summary>
    private void navigateToSavePage(object? obj)
    {
        SPageNavigationController.navigateTo(new SaveTreatmentPage());
    }

    /// <summary>
    /// Navigates to the save page for modifying the selected treatment.
    /// </summary>
    public void navigateToModifyPage(object? obj)
    {
        if (_m_selected_item != null)
        {
            SPageNavigationController.navigateTo(new SaveTreatmentPage(_m_selected_item.treatment_id));
        }
    }

    /// <summary>
    /// Deletes the selected treatment (hard delete).
    /// </summary>
    private void deleteItem(object? obj)
    {
        if (_m_selected_item != null)
        {
            int stockId = _m_selected_item.fk_treatment_stock_id;
            var result = _m_treatment_model.deleteItem(_m_selected_item.treatment_id.ToString());

            if (!result.is_success)
            {
                MessageBox.Show(Loc.Get("Message.DeleteErrorOccurred"),
                    Loc.Get("Common.Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                _m_treatment_stock_model.updateUsageDates(stockId);
            }

            updateDisplayedItems();
        }
    }

    /// <summary>
    /// Generates a PDF of the current filtered treatment list.
    /// </summary>
    private void generatePdf(object? obj)
    {
        if (m_list_items != null && m_list_items.Count > 0)
        {
            SInvoicePdfService.generateAndOpenTreatmentPdf(m_list_items);
        }
    }
}
