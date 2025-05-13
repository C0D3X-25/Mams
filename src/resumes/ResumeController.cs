using Mams.src.controllers;
using Mams.src.databaseOperations;
using Mams.src.fees;
using Mams.src.profits;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Mams.src.resumes;

public class ResumeController : ABaseController {

    private readonly ResumeModel _m_resume_model;


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
            updateDisplayedProfitsAndFeesLists();
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
            updateDisplayedProfitsAndFeesLists();
            onPropertyChanged();
        }
    }


    private ObservableCollection<SearchItem>? _m_list_year;
    public ObservableCollection<SearchItem>? m_list_year {
        get { return _m_list_year; }
        set {
            _m_list_year = value;
            onPropertyChanged();
        }
    }


    private SearchItem? _m_selected_year;
    public SearchItem? m_selected_year {
        get { return _m_selected_year; }
        set {
            _m_selected_year = value;
            updateDisplayedProfitsAndFeesLists();
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

    private decimal _m_total_profit = 0.00M;
    public decimal m_total_profit {
        get { return _m_total_profit; }
        set {
            _m_total_profit = value;
            onPropertyChanged();
        }
    }


    private decimal _m_total_fee = 0.00M;
    public decimal m_total_fee {
        get { return _m_total_fee; }
        set {
            _m_total_fee = value;
            onPropertyChanged();
        }
    }


    private decimal _m_total = 0.00M;
    public decimal m_total {
        get { return _m_total; }
        set {
            _m_total = value;
            onPropertyChanged();
        }
    }


    private SolidColorBrush _m_total_fee_color = Brushes.Black;
    public SolidColorBrush m_total_fee_color {
        get { return _m_total_fee_color; }
        set {
            _m_total_fee_color = value;
            onPropertyChanged();
        }
    }


    private SolidColorBrush _m_total_color = Brushes.Black;
    public SolidColorBrush m_total_color {
        get { return _m_total_color; }
        set {
            _m_total_color = value;
            onPropertyChanged();
        }
    }


    public ResumeController() {

        _m_resume_model = new();

        populateListTable();

        updateDisplayedProfitsAndFeesLists();
        updateListSearchItems();
    }


    private void updateDisplayedProfitsAndFeesLists() {

        if (m_selected_search_item == null) {
            m_selected_search_item = new SearchItem();
        }
        if (m_selected_table == null) {
            m_selected_table = new DatabaseTablesNameItem();
        }
        if (m_selected_year == null) {
            m_selected_year = new SearchItem();
        }

        m_selected_search_item.search_table = m_selected_table.m_name_in_database;
        m_selected_search_item.search_year = m_selected_year.search_year;

        m_list_profit_item = _m_resume_model.getFilteredProfits(m_selected_search_item);
        m_list_fee_item = _m_resume_model.getFilteredFees(m_selected_search_item);
        updateDisplayedTotalTransactions();
    }


    private void updateDisplayedTotalTransactions() {

        m_total_profit = 0.00M;
        m_total_fee = 0.00M;

        if (m_list_profit_item != null) {
            foreach (ProfitItem item in m_list_profit_item) {
                m_total_profit += item.profit_price_total;
            }
        }
        if (m_list_fee_item != null) {
            foreach (FeeItem item in m_list_fee_item) {
                // Subtract the fees to display the correct (negative) value
                m_total_fee -= item.fee_price_total;
            }
            if (m_total_fee < 0) {
                m_total_fee_color = Brushes.Red;
            }
            else {
                m_total_fee_color = Brushes.Black;
            }
        }

        // Addition of profits and fees, because fee is negative or 0
        m_total = m_total_profit + m_total_fee;

        if (m_total < 0) {
            m_total_color = Brushes.Red;
        }
        else {
            m_total_color = Brushes.Black;
        }
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