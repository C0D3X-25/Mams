using Mams.src.commands;
using Mams.src.views;
using Mams.src.views.pages;
using Mams.src.views.userControls;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.controllers;

public class UCMenuController {

    private readonly PageNavigationController _m_page_navigation;
    public ICommand m_navigate_fee_command { get; set; }
    public ICommand m_navigate_profit_command { get; set; }
    public ICommand m_navigate_list_client_command { get; set; }
    public ICommand m_navigate_list_lot_command { get; set; }
    public ICommand m_navigate_list_product_command { get; set; }

    public UCMenuController(PageNavigationController pageNavigation) {

        _m_page_navigation = pageNavigation;

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
        _m_page_navigation.navigateTo(new ListLotPage());
    }

    private void navigateToListProduct(object? obj) {
        _m_page_navigation.navigateTo(new ListProductPage());
    }
}
