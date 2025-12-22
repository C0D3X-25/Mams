using System.Windows;
using Mams.src.services;

namespace Mams;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Prevent the MainWindow from showing automatically
        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        // Run the async initialization on the UI thread
        Dispatcher.InvokeAsync(async () =>
        {
            try
            {
                // Ensure MariaDB Portable is installed, running, and database is ready
                if (!await SMariaDbPortableService.ensureMariaDbReadyAsync())
                {
                    // MariaDB setup failed, exit the application
                    Shutdown(1);
                    return;
                }

                // MariaDB is ready, now show the main window
                ShutdownMode = ShutdownMode.OnMainWindowClose;
                
                var mainWindow = new Mams.src.views.MainWindow();
                MainWindow = mainWindow;
                mainWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Failed to start application:\n\n{ex.Message}",
                    "Startup Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(1);
            }
        });
    }

    protected override void OnExit(ExitEventArgs e)
    {
        // Stop MariaDB when the application exits
        SMariaDbPortableService.stopMariaDb();
        base.OnExit(e);
    }
}

