using Mams.src.commands;
using Mams.src.entities;
using Mams.src.views.pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Mams.src.navigations;

public class UCHeaderController {

    public ICommand m_navigate_homepage_command { get; set; }

    public UCHeaderController() {

        m_navigate_homepage_command = new RelayCommand(navigateToHomepage);
    }

    private void navigateToHomepage(object? obj) {
        PageNavigationController.navigateToHomePage();
    }
}
