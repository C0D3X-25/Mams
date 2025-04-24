using Mams.src.clients;
using Mams.src.commands;
using Mams.src.fees;
using Mams.src.products;
using Mams.src.productsLots;
using Mams.src.profits;
using System.Windows.Input;

namespace Mams.src.navigations;

public class UCMenuController {

    private readonly PageNavigationController _m_page_navigation;
    public ICommand m_navigate_fee_command { get; set; }
    public ICommand m_navigate_profit_command { get; set; }
    public ICommand m_navigate_list_client_command { get; set; }
    public ICommand m_navigate_list_lot_command { get; set; }
    public ICommand m_navigate_list_product_command { get; set; }

    public UCMenuController(PageNavigationController page_navigation) {

        _m_page_navigation = page_navigation;

        m_navigate_fee_command = new RelayCommand(navigateToFee);
        m_navigate_profit_command = new RelayCommand(navigateToProfit);
        m_navigate_list_client_command = new RelayCommand(navigateToListClient);
        m_navigate_list_lot_command = new RelayCommand(navigateToListLot);
        m_navigate_list_product_command = new RelayCommand(navigateToListProduct);
    }

    private void navigateToFee(object? obj) {
        _m_page_navigation.navigateTo(new SaveFeePage());
    }

    private void navigateToProfit(object? obj) {
        _m_page_navigation.navigateTo(new SaveProfitPage());
    }

    private void navigateToListClient(object? obj) {
        _m_page_navigation.navigateTo(new ListClientPage());
    }

    private void navigateToListLot(object? obj) {
        _m_page_navigation.navigateTo(new ListProductLotPage());
    }

    private void navigateToListProduct(object? obj) {
        _m_page_navigation.navigateTo(new ListProductPage());
    }
}
