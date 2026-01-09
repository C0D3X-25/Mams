using Mams_App.src.configurations;
using Mams_App.src.databaseOperations;
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
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Load configuration and set culture
        var config = SAppConfigService.loadConfig();
        setCultureFromConfig(config);

        // Prevent the MainWindow from showing automatically
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Start initialization
        initializeApplicationAsync();
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
    /// Initializes the application asynchronously.
    /// </summary>
    private async void initializeApplicationAsync()
    {
        try
        {
            // Ensure MariaDB Portable is installed, running, and database is ready
            bool mariaDbReady = await SMariaDbPortableService.ensureMariaDbReadyAsync();

            if (!mariaDbReady)
            {
                // MariaDB setup failed, exit the application
                Dispatcher.Invoke(() => Shutdown(1));
                return;
            }

            // Perform startup backup
            performBackup("Startup");

            // MariaDB is ready, now show the main window on the UI thread
            Dispatcher.Invoke(() =>
            {
                ShutdownMode = ShutdownMode.OnMainWindowClose;

                var mainWindow = new Mams_App.src.views.MainWindow();
                MainWindow = mainWindow;
                mainWindow.Show();
            });
        }
        catch (Exception ex)
        {
            Dispatcher.Invoke(() =>
            {
                MessageBox.Show(
                    $"Failed to start application:\n\n{ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(1);
            });
        }
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

