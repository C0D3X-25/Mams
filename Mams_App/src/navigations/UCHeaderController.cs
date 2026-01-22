using Mams_App.src.commands;
using Mams_App.src.searches;
using System.Windows.Input;

namespace Mams_App.src.navigations;

public class UCHeaderController
{
    /// <summary>
    /// Command to navigate to the homepage.
    /// </summary>
    public ICommand m_navigate_homepage_command { get; set; }

    /// <summary>
    /// Command to navigate to the global search page.
    /// </summary>
    public ICommand m_navigate_searchpage_command { get; set; }

    public UCHeaderController()
    {
        m_navigate_homepage_command = new RelayCommand(navigateToHomepage);
        m_navigate_searchpage_command = new RelayCommand(navigateToSearchPage);
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

    /// <summary>
    /// Navigates to the global search page.
    /// </summary>
    /// <param name="obj">An optional parameter that can be used to pass additional context for navigation. This parameter is currently
    /// unused.</param>
    private void navigateToSearchPage(object? obj)
    {
        SPageNavigationController.navigateTo(new ListSearchPage(), true);
    }
}
