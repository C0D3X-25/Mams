using Mams.src.commands;
using Mams.src.fees;
using Mams.src.products;
using Mams.src.productsLots;
using Mams.src.productsCategories;
using Mams.src.beehives;
using Mams.src.productsShapes;
using Mams.src.productsTypes;
using Mams.src.profits;
using System.Windows.Input;
using Mams.src.entities;

namespace Mams.src.navigations;

public class UCMenuController {

    public ICommand m_navigate_list_fee_command { get; set; }
    public ICommand m_navigate_list_profit_command { get; set; }
    public ICommand m_navigate_list_entity_command { get; set; }
    public ICommand m_navigate_list_product_command { get; set; }
    public ICommand m_navigate_list_lot_command { get; set; }
    public ICommand m_navigate_list_beehive_command { get; set; }
    public ICommand m_navigate_list_product_category_command { get; set; }
    public ICommand m_navigate_list_product_type_command { get; set; }
    public ICommand m_navigate_list_product_shape_command { get; set; }

    public UCMenuController() {
        m_navigate_list_fee_command = new RelayCommand(navigateToListFee);
        m_navigate_list_profit_command = new RelayCommand(navigateToListProfit);
        m_navigate_list_entity_command = new RelayCommand(navigateToListEntity);
        m_navigate_list_product_command = new RelayCommand(navigateToListProduct);
        m_navigate_list_lot_command = new RelayCommand(navigateToListLot);
        m_navigate_list_beehive_command = new RelayCommand(navigateToListBeehive);
        m_navigate_list_product_category_command = new RelayCommand(navigateToListProductCategory);
        m_navigate_list_product_type_command = new RelayCommand(navigateToListProductType);
        m_navigate_list_product_shape_command = new RelayCommand(navigateToListProductShape);
    }

    private void navigateToListFee(object? obj) {
        SPageNavigationController.navigateTo(new ListFeePage());
    }

    private void navigateToListProfit(object? obj) {
        SPageNavigationController.navigateTo(new ListProfitPage());
    }

    private void navigateToListEntity(object? obj) {
        SPageNavigationController.navigateTo(new ListEntityPage());
    }
    private void navigateToListProduct(object? obj) {
        SPageNavigationController.navigateTo(new ListProductPage());
    }

    private void navigateToListLot(object? obj) {
        SPageNavigationController.navigateTo(new ListProductLotPage());
    }

    private void navigateToListBeehive(object? obj) {
        SPageNavigationController.navigateTo(new ListBeehivePage());
    }

    private void navigateToListProductCategory(object? obj) {
        SPageNavigationController.navigateTo(new ListProductCategoryPage());
    }

    private void navigateToListProductType(object? obj) {
        SPageNavigationController.navigateTo(new ListProductTypePage());
    }

    private void navigateToListProductShape(object? obj) {
        SPageNavigationController.navigateTo(new ListProductShapePage());
    }
}
