using Mams_App.src.commands;
using Mams_App.src.resumes;
using Mams_App.src.searches;
using Mams_App.src.settings;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.navigations;

public class UCHeaderController : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _m_is_home_page_active;
    private bool _m_is_search_page_active;

    public bool m_is_home_page_active
    {
        get => _m_is_home_page_active;
        set { _m_is_home_page_active = value; OnPropertyChanged(); }
    }

    public bool m_is_search_page_active
    {
        get => _m_is_search_page_active;
        set { _m_is_search_page_active = value; OnPropertyChanged(); }
    }

    /// <summary>
    /// Command to navigate to the homepage.
    /// </summary>
    public ICommand m_navigate_homepage_command { get; set; }

    /// <summary>
    /// Command to navigate to the global search page.
    /// </summary>
    public ICommand m_navigate_searchpage_command { get; set; }

    /// <summary>
    /// Command to open the settings window.
    /// </summary>
    public ICommand m_open_settings_command { get; set; }

    public UCHeaderController()
    {
        m_navigate_homepage_command = new RelayCommand(navigateToHomepage);
        m_navigate_searchpage_command = new RelayCommand(navigateToSearchPage);
        m_open_settings_command = new RelayCommand(openSettings);

        SPageNavigationController.PageChanged += OnPageChanged;
    }

    private void OnPageChanged(Type? pageType)
    {
        m_is_home_page_active = pageType == typeof(ResumePage);
        m_is_search_page_active = pageType == typeof(ListSearchPage);
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

    /// <summary>
    /// Opens the settings window.
    /// </summary>
    /// <param name="obj">An optional parameter that can be used to pass additional context. This parameter is currently unused.</param>
    private void openSettings(object? obj)
    {
        var mainWindow = Application.Current.MainWindow;
        SettingsWindow.ShowSettings(mainWindow);
    }
}
