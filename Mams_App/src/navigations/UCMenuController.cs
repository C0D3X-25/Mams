using Mams_App.src.beehives;
using Mams_App.src.commands;
using Mams_App.src.entities;
using Mams_App.src.fees;
using Mams_App.src.products;
using Mams_App.src.productsCategories;
using Mams_App.src.productsLots;
using Mams_App.src.productsShapes;
using Mams_App.src.productsTypes;
using Mams_App.src.profits;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Mams_App.src.navigations;

public class UCMenuController : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _m_is_profit_page_active;
    private bool _m_is_fee_page_active;
    private bool _m_is_entity_page_active;
    private bool _m_is_product_page_active;
    private bool _m_is_lot_page_active;
    private bool _m_is_beehive_page_active;
    private bool _m_is_product_category_page_active;
    private bool _m_is_product_type_page_active;
    private bool _m_is_product_shape_page_active;

    public bool m_is_profit_page_active
    {
        get => _m_is_profit_page_active;
        set { _m_is_profit_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_fee_page_active
    {
        get => _m_is_fee_page_active;
        set { _m_is_fee_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_entity_page_active
    {
        get => _m_is_entity_page_active;
        set { _m_is_entity_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_product_page_active
    {
        get => _m_is_product_page_active;
        set { _m_is_product_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_lot_page_active
    {
        get => _m_is_lot_page_active;
        set { _m_is_lot_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_beehive_page_active
    {
        get => _m_is_beehive_page_active;
        set { _m_is_beehive_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_product_category_page_active
    {
        get => _m_is_product_category_page_active;
        set { _m_is_product_category_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_product_type_page_active
    {
        get => _m_is_product_type_page_active;
        set { _m_is_product_type_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_product_shape_page_active
    {
        get => _m_is_product_shape_page_active;
        set { _m_is_product_shape_page_active = value; OnPropertyChanged(); }
    }

    public ICommand m_navigate_list_fee_command { get; set; }
    public ICommand m_navigate_list_profit_command { get; set; }
    public ICommand m_navigate_list_entity_command { get; set; }
    public ICommand m_navigate_list_product_command { get; set; }
    public ICommand m_navigate_list_lot_command { get; set; }
    public ICommand m_navigate_list_beehive_command { get; set; }
    public ICommand m_navigate_list_product_category_command { get; set; }
    public ICommand m_navigate_list_product_type_command { get; set; }
    public ICommand m_navigate_list_product_shape_command { get; set; }

    public UCMenuController()
    {
        m_navigate_list_fee_command = new RelayCommand(navigateToListFee);
        m_navigate_list_profit_command = new RelayCommand(navigateToListProfit);
        m_navigate_list_entity_command = new RelayCommand(navigateToListEntity);
        m_navigate_list_product_command = new RelayCommand(navigateToListProduct);
        m_navigate_list_lot_command = new RelayCommand(navigateToListLot);
        m_navigate_list_beehive_command = new RelayCommand(navigateToListBeehive);
        m_navigate_list_product_category_command = new RelayCommand(navigateToListProductCategory);
        m_navigate_list_product_type_command = new RelayCommand(navigateToListProductType);
        m_navigate_list_product_shape_command = new RelayCommand(navigateToListProductShape);

        SPageNavigationController.PageChanged += OnPageChanged;
    }

    private void OnPageChanged(Type? pageType)
    {
        m_is_profit_page_active = pageType == typeof(ListProfitPage);
        m_is_fee_page_active = pageType == typeof(ListFeePage);
        m_is_entity_page_active = pageType == typeof(ListEntityPage);
        m_is_product_page_active = pageType == typeof(ListProductPage);
        m_is_lot_page_active = pageType == typeof(ListProductLotPage);
        m_is_beehive_page_active = pageType == typeof(ListBeehivePage);
        m_is_product_category_page_active = pageType == typeof(ListProductCategoryPage);
        m_is_product_type_page_active = pageType == typeof(ListProductTypePage);
        m_is_product_shape_page_active = pageType == typeof(ListProductShapePage);
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void navigateToListFee(object? obj)
    {
        SPageNavigationController.navigateTo(new ListFeePage(), true);
    }

    private void navigateToListProfit(object? obj)
    {
        SPageNavigationController.navigateTo(new ListProfitPage(), true);
    }

    private void navigateToListEntity(object? obj)
    {
        SPageNavigationController.navigateTo(new ListEntityPage(), true);
    }
    private void navigateToListProduct(object? obj)
    {
        SPageNavigationController.navigateTo(new ListProductPage(), true);
    }

    private void navigateToListLot(object? obj)
    {
        SPageNavigationController.navigateTo(new ListProductLotPage(), true);
    }

    private void navigateToListBeehive(object? obj)
    {
        SPageNavigationController.navigateTo(new ListBeehivePage(), true);
    }

    private void navigateToListProductCategory(object? obj)
    {
        SPageNavigationController.navigateTo(new ListProductCategoryPage(), true);
    }

    private void navigateToListProductType(object? obj)
    {
        SPageNavigationController.navigateTo(new ListProductTypePage(), true);
    }

    private void navigateToListProductShape(object? obj)
    {
        SPageNavigationController.navigateTo(new ListProductShapePage(), true);
    }
}
