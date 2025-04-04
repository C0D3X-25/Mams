using Mams.src.commands;
using Mams.src.views.pages;
using System.Windows;
using System.Windows.Input;

namespace Mams.src.controllers;


public class z_UCAddNewButtonController : DependencyObject  {

    public readonly PageNavigationController _m_page_navigation;

    public ICommand m_navigate_command { get; set; }

    public z_UCAddNewButtonController(PageNavigationController page_navigation) {

        _m_page_navigation = page_navigation;

        m_navigate_command = new RelayCommand(navigateTo);
        // Todo: Need to pass CommandParameter in the xaml to precise the Page to load ?
    }

    private void navigateTo(object? obj) {
        _m_page_navigation.navigateTo(new SaveClientPage()); // need to take parameter to load the right page
    }
}
