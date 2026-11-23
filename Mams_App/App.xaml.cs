using Mams_App.src.configurations;
using Mams_App.src.databaseOperations;
using Mams_App.src.launcher;
using Mams_App.src.mainWindow;
using Mams_App.src.services;
using MySqlConnector;
using System.Globalization;
using System.Windows;

namespace Mams_App;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private LauncherWindow? _launcherWindow;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Clear version service cache to ensure fresh data after an update
        SVersionService.ClearCache();

        // Load configuration and set culture
        var config = SAppConfigService.loadConfig();
        setCultureFromConfig(config);

        // Prevent automatic shutdown when launcher closes
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Show launcher window and start initialization
        showLauncherAndInitialize();
    }

    /// <summary>
    /// Sets the application culture from the configuration.
    /// </summary>
    /// <param name="config">The application configuration.</param>
    private static void setCultureFromConfig(AppConfigItem config)
    {
        try
        {
            var cultureName = config.m_localization.m_culture;
            if (string.IsNullOrEmpty(cultureName))
            {
                cultureName = "fr-CH"; // Default to Swiss French
            }

            var culture = new CultureInfo(cultureName);
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
        catch
        {
            // If culture is invalid, fall back to Swiss French
            var swissCulture = new CultureInfo("fr-CH");
            CultureInfo.DefaultThreadCurrentCulture = swissCulture;
            CultureInfo.DefaultThreadCurrentUICulture = swissCulture;
            Thread.CurrentThread.CurrentCulture = swissCulture;
            Thread.CurrentThread.CurrentUICulture = swissCulture;
        }
    }

    /// <summary>
    /// Shows the launcher window and starts the initialization process.
    /// </summary>
    private void showLauncherAndInitialize()
    {
        _launcherWindow = new LauncherWindow();

        _launcherWindow.InitializationCompleted += (s, e) =>
        {
            Dispatcher.Invoke(() =>
            {
                // Perform startup backup
                performBackup("Startup");

                // Show main window
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                var mainWindow = new MainWindow();
                MainWindow = mainWindow;
                mainWindow.Show();

                // Close launcher
                _launcherWindow?.Close();
            });
        };

        _launcherWindow.InitializationFailed += (s, e) =>
        {
            Dispatcher.Invoke(() =>
            {
                _launcherWindow?.Close();
                Shutdown(1);
            });
        };

        // Subscribe to ContentRendered before showing the window
        // ContentRendered fires after the window is fully rendered and ready
        _launcherWindow.ContentRendered += async (s, e) =>
        {
            try
            {
                await _launcherWindow.StartInitializationAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[App] Initialization error: {ex.Message}");
                _launcherWindow?.Close();
                Shutdown(1);
            }
        };

        _launcherWindow.Show();
    }

    /// <summary>
    /// Performs a backup of the database.
    /// </summary>
    /// <param name="context">Context for logging (e.g., "Startup" or "Shutdown")</param>
    private static void performBackup(string context)
    {
        // Skip backup if MariaDB is not running
        if (!SMariaDbPortableService.isRunning())
        {
            System.Diagnostics.Debug.WriteLine($"[App] {context} backup skipped - MariaDB not running");
            return;
        }

        try
        {
            using var connection = new MySqlConnection(SMariaDbPortableService.ConnectionString);
            connection.Open();
            SDatabaseBackup.createBackup(connection);
            System.Diagnostics.Debug.WriteLine($"[App] {context} backup completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[App] {context} backup failed: {ex.Message}");
            // Don't show error to user - backup failure shouldn't prevent app from starting/closing
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Perform shutdown backup before stopping MariaDB
        performBackup("Shutdown");

        // Stop MariaDB when the application exits
        SMariaDbPortableService.stopMariaDb();
        base.OnExit(e);
    }
}

