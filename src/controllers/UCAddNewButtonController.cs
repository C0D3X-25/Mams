using Mams.src.commands;
using System.Windows.Input;

namespace Mams.src.controllers;

public class UCAddNewButtonController {

    private readonly PageNavigationController _m_page_navigation;

    public ICommand m_navigate_command { get; set; }

    public UCAddNewButtonController(PageNavigationController page_navigation) {

        _m_page_navigation = page_navigation;

        m_navigate_command = new RelayCommand(navigateTo);
    }

    private void navigateTo(object? obj) {
        throw new NotImplementedException();
    }
}
