using Mams_App.src.commands;
using Mams_App.src.views.globalView;
using System.Windows.Input;

namespace Mams_App.src.navigations;

public class UCHeaderController {

    public ICommand m_navigate_homepage_command { get; set; }
    public string m_page_top_color { get; set; } = SGlobalView.m_header_top_color;
    public string m_page_bot_color { get; set; } = SGlobalView.m_header_bot_color;
    public string m_page_title_color { get; set; } = SGlobalView.m_header_title_color;
    public string m_button_background_color { get; set; } = SGlobalView.m_menu_button_background_color;
    public string m_button_text_color { get; set; } = SGlobalView.m_header_button_text_color;
    public string m_button_over_color { get; set; } = SGlobalView.m_header_button_over_color;

    public UCHeaderController() {
        m_navigate_homepage_command = new RelayCommand(navigateToHomepage);
    }

    /// <summary>
    /// Navigates to the application's homepage.
    /// </summary>
    /// <param name="obj">An optional parameter that can be used to pass additional context for navigation. This parameter is currently
    /// unused.</param>
    private void navigateToHomepage(object? obj) {
        SPageNavigationController.navigateToHomePage();
    }
}
