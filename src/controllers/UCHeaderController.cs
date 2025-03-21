using Mams.src.commands;
using Mams.src.views.pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mams.src.controllers;

public class UCHeaderController {

    private readonly PageNavigationController _m_page_navigation;
    public ICommand m_navigate_homepage_command { get; set; }

    public UCHeaderController(PageNavigationController pageNavigation) {

        _m_page_navigation = pageNavigation;
        m_navigate_homepage_command = new RelayCommand(navigateToHomepage);
    }

    private void navigateToHomepage(object? obj) {
        _m_page_navigation.navigateToHomePage();
    }
}
