using Mams.src.commands;
using Mams.src.controllers;
using Mams.src.databaseOperations;
using Mams.src.fees;
using Mams.src.navigations;
using Mams.src.profits;
using Mams.src.views.globalView;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Media;

namespace Mams.src.resumes;

public class ResumeController : ABaseController {

    public string m_page_background_color { get; set; } = SGlobalView.m_page_frame_color;
    public string m_body_background_color { get; set; } = SGlobalView.m_page_body_color;
    public string m_page_button_color_1 { get; set; } = SGlobalView.m_header_button_background_color;
    public string m_page_button_text_color { get; set; } = SGlobalView.m_header_button_text_color;

    public ICommand m_clear_search_command { get; }
    public ICommand m_double_click_command_profit { get; }
    public ICommand m_double_click_command_fee { get; }

    private readonly ResumeModel _m_resume_model = new();

    private ObservableCollection<DatabaseTablesNameItem>? _m_list_table;
    public ObservableCollection<DatabaseTablesNameItem>? m_list_table {
        get => _m_list_table;
        set {
            if (_m_list_table != value) {
                _m_list_table = value;
                onPropertyChanged();
            }
        }
    }

    private DatabaseTablesNameItem? _m_selected_table;
    public DatabaseTablesNameItem? m_selected_table {
        get => _m_selected_table;
        set {
            if (_m_selected_table != value) {
                _m_selected_table = value;
                updateListSearchItems();
                updateDisplayedProfitsAndFeesLists();
                onPropertyChanged();
            }
        }
    }

    private ObservableCollection<SearchItem>? _m_list_search_item;
    public ObservableCollection<SearchItem>? m_list_search_item {
        get => _m_list_search_item;
        set {
            if (_m_list_search_item != value) {
                _m_list_search_item = value;
                onPropertyChanged();
            }
        }
    }

    private SearchItem? _m_selected_search_item;
    public SearchItem? m_selected_search_item {
        get => _m_selected_search_item;
        set {
            if (_m_selected_search_item != value) {
                _m_selected_search_item = value;
                updateDisplayedProfitsAndFeesLists();
                onPropertyChanged();
            }
        }
    }

    private ObservableCollection<SearchItem>? _m_list_year;
    public ObservableCollection<SearchItem>? m_list_year {
        get => _m_list_year;
        set {
            if (_m_list_year != value) {
                _m_list_year = value;
                onPropertyChanged();
            }
        }
    }

    private SearchItem? _m_selected_year;
    public SearchItem? m_selected_year {
        get => _m_selected_year;
        set {
            if (_m_selected_year != value) {
                _m_selected_year = value;
                updateDisplayedProfitsAndFeesLists();
                onPropertyChanged();
            }
        }
    }

    private ObservableCollection<ReceiptProfitDetailedItem>? _m_list_profit_item;
    public ObservableCollection<ReceiptProfitDetailedItem>? m_list_profit_item {
        get => _m_list_profit_item;
        set {
            if (_m_list_profit_item != value) {
                _m_list_profit_item = value;
                onPropertyChanged();
            }
        }
    }

    private ObservableCollection<ReceiptFeeDetailedItem>? _m_list_fee_item;
    public ObservableCollection<ReceiptFeeDetailedItem>? m_list_fee_item {
        get => _m_list_fee_item;
        set {
            if (_m_list_fee_item != value) {
                _m_list_fee_item = value;
                onPropertyChanged();
            }
        }
    }

    private decimal _m_total_profit;
    public decimal m_total_profit {
        get => _m_total_profit;
        set {
            if (_m_total_profit != value) {
                _m_total_profit = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_profit_ui));
            }
        }
    }
    public string m_total_profit_ui => $"{_m_total_profit:F2} CHF";

    private decimal _m_total_fee;
    public decimal m_total_fee {
        get => _m_total_fee;
        set {
            if (_m_total_fee != value) {
                _m_total_fee = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_fee_ui));
            }
        }
    }
    public string m_total_fee_ui => $"{_m_total_fee:F2} CHF";

    private decimal _m_total;
    public decimal m_total {
        get => _m_total;
        set {
            if (_m_total != value) {
                _m_total = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_ui));
            }
        }
    }
    public string m_total_ui => $"{_m_total:F2} CHF";

    private decimal _m_total_weight;
    public decimal m_total_weight {
        get => _m_total_weight;
        set {
            if (_m_total_weight != value) {
                _m_total_weight = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_weight_ui));
            }
        }
    }
    public string m_total_weight_ui => $"{_m_total_weight:F2} kg";

    private int _m_total_quantity;
    public int m_total_quantity {
        get => _m_total_quantity;
        set {
            if (_m_total_quantity != value) {
                _m_total_quantity = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_total_quantity_ui));
            }
        }
    }
    public string m_total_quantity_ui => $"{_m_total_quantity}";

    private decimal _m_average_price;
    public decimal m_average_price {
        get => _m_average_price;
        set {
            if (_m_average_price != value) {
                _m_average_price = value;
                onPropertyChanged();
                onPropertyChanged(nameof(m_average_price_ui));
            }
        }
    }
    public string m_average_price_ui => $"{m_average_price:F2} CHF/u";

    private SolidColorBrush _m_total_fee_color = Brushes.Black;
    public SolidColorBrush m_total_fee_color {
        get => _m_total_fee_color;
        set {
            if (_m_total_fee_color != value) {
                _m_total_fee_color = value;
                onPropertyChanged();
            }
        }
    }

    private SolidColorBrush _m_total_color = Brushes.Black;
    public SolidColorBrush m_total_color {
        get => _m_total_color;
        set {
            if (_m_total_color != value) {
                _m_total_color = value;
                onPropertyChanged();
            }
        }
    }

    private ReceiptProfitDetailedItem? _m_selected_profit_item;
    public ReceiptProfitDetailedItem? m_selected_profit_item {
        get => _m_selected_profit_item;
        set {
            if (_m_selected_profit_item != value) {
                _m_selected_profit_item = value;
                onPropertyChanged();
            }
        }
    }

    private ReceiptFeeDetailedItem? _m_selected_fee_item;
    public ReceiptFeeDetailedItem? m_selected_fee_item {
        get => _m_selected_fee_item;
        set {
            if (_m_selected_fee_item != value) {
                _m_selected_fee_item = value;
                onPropertyChanged();
            }
        }
    }

    public ResumeController() {
        m_clear_search_command = new RelayCommand(clearSelectedItems);
        m_double_click_command_profit = new RelayCommand(navigateToProfitDetails, isProfitItemSelected);
        m_double_click_command_fee = new RelayCommand(navigateToFeeDetails, isFeeItemSelected);

        populateListTable();
        updateDisplayedProfitsAndFeesLists();
        updateListSearchItems();
    }

    public void navigateToProfitDetails(object? obj) {
        if (_m_selected_profit_item != null) {
            SPageNavigationController.navigateTo(new SaveProfitPage(_m_selected_profit_item.receipt.receipt_id));
        }
    }

    public void navigateToFeeDetails(object? obj) {
        if (_m_selected_fee_item != null) {
            SPageNavigationController.navigateTo(new SaveFeePage(_m_selected_fee_item.receipt.receipt_id));
        }
    }

    private void clearSelectedItems(object? obj) {
        if (m_list_table?.Count > 0) {
            m_selected_table = m_list_table[0];
        }
        
        if (m_list_search_item?.Count > 0) {
            m_selected_search_item = m_list_search_item[0];
        }
        
        if (m_list_year?.Count > 0) {
            m_selected_year = m_list_year[0];
        }
        
        // Force update display
        updateDisplayedProfitsAndFeesLists();
    }

    private bool isProfitItemSelected(object? arg) => m_selected_profit_item != null;

    private bool isFeeItemSelected(object? arg) => m_selected_fee_item != null;

    private void updateDisplayedProfitsAndFeesLists() {
        var search_item = _m_selected_search_item ?? new SearchItem();
        var table_item = _m_selected_table ?? new DatabaseTablesNameItem();
        var year_item = _m_selected_year ?? new SearchItem();

        search_item.search_table = table_item.m_name_in_database;
        search_item.search_year = year_item.search_year;

        var resume_item = _m_resume_model.getFilteredResume(search_item);

        m_list_profit_item = resume_item.profit_items;
        m_list_fee_item = resume_item.fee_items;

        updateDisplayedTotalTransactions();
        updateDisplayedDetailTransactions();
    }

    private void updateDisplayedTotalTransactions() {
        decimal total_profit = 0.00M;
        decimal total_fee = 0.00M;

        if (m_list_profit_item != null) {
            foreach (var item in m_list_profit_item) {
                total_profit += item.receipt.receipt_total_price;
            }
        }

        if (m_list_fee_item != null) {
            foreach (var item in m_list_fee_item) {
                total_fee -= item.receipt.receipt_total_price;
            }
        }

        m_total_profit = total_profit;
        m_total_fee = total_fee;
        m_total_fee_color = total_fee < 0 ? Brushes.Red : Brushes.Black;

        // Calculate total (profit + fee)
        decimal total = total_profit + total_fee;
        m_total = total;
        m_total_color = total < 0 ? Brushes.Red : Brushes.Black;
    }

    private void updateDisplayedDetailTransactions() {
        // Reset values for invalid search criteria
        if (_m_selected_search_item == null || _m_selected_search_item.search_id == 0) {
            m_total_weight = 0.00M;
            m_total_quantity = 0;
            m_average_price = 0.00M;
            return;
        }

        decimal total_weight = 0.00M;
        int total_quantity = 0;

        if (m_list_profit_item != null) {
            foreach (var profit_item in m_list_profit_item) {
                foreach (var receipt_product in profit_item.receipt_products) {
                    int quantity = receipt_product.receipt_product_quantity;
                    total_quantity += quantity;
                    total_weight += receipt_product.product_item.product_weight * quantity;
                }
            }
        }

        m_total_weight = total_weight / 1000;
        m_total_quantity = total_quantity;
        m_average_price = total_quantity > 0 ? m_total_profit / total_quantity : 0.00M;
    }

    private void populateListTable() {
        m_list_table = _m_resume_model.m_search_tables;
    }

    private void updateListSearchItems() {
        if (_m_selected_table != null) {
            m_list_search_item = _m_resume_model.getListSearchItems(_m_selected_table);
            m_list_year = _m_resume_model.getListYears();
        }
    }
}