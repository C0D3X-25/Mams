using Mams_App.src.commands;
using System.Windows.Input;

namespace Mams_App.src.navigations;

public class UCHeaderController
{

    public ICommand m_navigate_homepage_command { get; set; }

    public UCHeaderController()
    {
        m_navigate_homepage_command = new RelayCommand(navigateToHomepage);
    }

    /// <summary>
    /// Navigates to the application's homepage.
    /// </summary>
    /// <param name="obj">An optional parameter that can be used to pass additional context for navigation. This parameter is currently
    /// unused.</param>
    private void navigateToHomepage(object? obj)
    {
        SPageNavigationController.navigateToHomePage();
    }
}
