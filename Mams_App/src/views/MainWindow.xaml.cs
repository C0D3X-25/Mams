using Mams_App.src.configurations;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.search;
using Mams_App.src.services;
using Mams_App.src.settings;
using System.Windows;

namespace Mams_App.src.views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{

    public MainWindow()
    {
        InitializeComponent();
        SPageNavigationController.initialize(MainFrame);
        SSearchModel.initialize();

        loadWindowSettings();

        Closing += MainWindow_Closing;
        Loaded += MainWindow_Loaded;
    }

    /// <summary>
    /// Checks for updates when the window is loaded.
    /// </summary>
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Check for updates silently (no message if already up to date)
        await SUpdateCheckerService.checkForUpdatesAsync(showNoUpdateMessage: false);
    }

    #region Menu Event Handlers

    /// <summary>
    /// Opens the Settings window.
    /// </summary>
    private void MenuItem_Settings_Click(object sender, RoutedEventArgs e)
    {
        SettingsWindow.ShowSettings(this);
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Shows the About dialog.
    /// </summary>
    private void MenuItem_About_Click(object sender, RoutedEventArgs e)
    {
        var version = SVersionService.GetVersion();
        var message = $"{Loc.Get("Settings.AppName")}\n\n{Loc.Get("Settings.Version")} {version}\n\n{Loc.Get("Settings.AppDescription")}";
        var title = Loc.Get("Menu.About");
        
        MessageBox.Show(
            message,
            title,
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    #endregion

    /// <summary>
    /// Loads window position and size from the configuration file.
    /// </summary>
    private void loadWindowSettings()
    {
        var config = SAppConfigService.loadConfig();
        var windowConfig = config.m_window;

        // Validate that the window position is within screen bounds
        var screenWidth = SystemParameters.VirtualScreenWidth;
        var screenHeight = SystemParameters.VirtualScreenHeight;

        if (windowConfig.m_left >= 0 && windowConfig.m_left < screenWidth)
        {
            Left = windowConfig.m_left;
        }

        if (windowConfig.m_top >= 0 && windowConfig.m_top < screenHeight)
        {
            Top = windowConfig.m_top;
        }

        if (windowConfig.m_width >= MinWidth && windowConfig.m_width <= screenWidth)
        {
            Width = windowConfig.m_width;
        }

        if (windowConfig.m_height >= MinHeight && windowConfig.m_height <= screenHeight)
        {
            Height = windowConfig.m_height;
        }

        if (windowConfig.m_is_maximized)
        {
            WindowState = WindowState.Maximized;
        }
    }

    /// <summary>
    /// Saves window position and size to the configuration file when closing.
    /// </summary>
    private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        var config = SAppConfigService.loadConfig();
        var windowConfig = config.m_window;

        // Save the restore bounds if maximized, otherwise save current bounds
        if (WindowState == WindowState.Maximized)
        {
            windowConfig.m_left = RestoreBounds.Left;
            windowConfig.m_top = RestoreBounds.Top;
            windowConfig.m_width = RestoreBounds.Width;
            windowConfig.m_height = RestoreBounds.Height;
            windowConfig.m_is_maximized = true;
        }
        else
        {
            windowConfig.m_left = Left;
            windowConfig.m_top = Top;
            windowConfig.m_width = Width;
            windowConfig.m_height = Height;
            windowConfig.m_is_maximized = false;
        }

        SAppConfigService.saveConfig(config);
    }
}
