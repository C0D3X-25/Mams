using Mams_App.src.configurations;
using Mams_App.src.localizations;
using Mams_App.src.services;
using System.Diagnostics;

namespace Mams_App.src.launcher;

/// <summary>
/// Controller for the launcher window that handles the application initialization process.
/// </summary>
public class LauncherWindowController
{
    /// <summary>
    /// Event raised when initialization is complete and successful.
    /// </summary>
    public event EventHandler? InitializationCompleted;

    /// <summary>
    /// Event raised when initialization fails.
    /// </summary>
    public event EventHandler? InitializationFailed;

    /// <summary>
    /// Event raised when the status message should be updated.
    /// </summary>
    public event Action<string>? StatusChanged;

    /// <summary>
    /// Event raised when progress should be updated (status, percentage or null for indeterminate).
    /// </summary>
    public event Action<string, double?>? ProgressChanged;

    /// <summary>
    /// Event raised when the progress bar visibility should change.
    /// </summary>
    public event Action<bool>? ProgressBarVisibilityChanged;

    /// <summary>
    /// Delegate for prompting the user with Yes/No options.
    /// </summary>
    public Func<string, string, string, Task<bool>>? PromptUserAsync { get; set; }

    /// <summary>
    /// Delegate for hiding the prompt UI.
    /// </summary>
    public Action? HidePrompt { get; set; }

    /// <summary>
    /// Gets the current application version.
    /// </summary>
    public string Version => SVersionService.GetVersion();

    /// <summary>
    /// Starts the initialization process.
    /// </summary>
    public async Task StartInitializationAsync()
    {
        try
        {
            Debug.WriteLine("[Launcher] Starting initialization...");

            // Show progress bar at the start
            ShowProgressBar(true);

            // Step 1: Check MariaDB installation
            Debug.WriteLine("[Launcher] Step 1: Checking MariaDB installation...");
            UpdateStatus(Loc.Get("Launcher.CheckingMariaDb") ?? "Checking database installation...");
            await Task.Delay(100);

            if (!SMariaDbPortableService.isInstalled())
            {
                Debug.WriteLine("[Launcher] MariaDB not installed, prompting user...");
                var installMessage = Loc.Get("Launcher.MariaDbSetupRequired.Message") ??
                    "The database server is not installed.\n\n" +
                    "This application requires MariaDB to store data.\n" +
                    "MariaDB Portable will be downloaded and installed automatically.\n\n" +
                    "Download size: ~100 MB\n\n" +
                    "Would you like to proceed with the installation?";

                var userWantsInstall = await PromptUserForConfirmationAsync(
                    installMessage,
                    Loc.Get("Common.Yes") ?? "Yes",
                    Loc.Get("Common.No") ?? "No");

                if (!userWantsInstall)
                {
                    Debug.WriteLine("[Launcher] User declined MariaDB installation");
                    OnInitializationFailed();
                    return;
                }

                Debug.WriteLine("[Launcher] User accepted, starting MariaDB download...");
                HidePrompt?.Invoke();
                ShowProgressBar(true);
                UpdateStatus(Loc.Get("Launcher.InstallingMariaDb") ?? "Installing database...");

                if (!await SMariaDbPortableService.downloadAndInstallMariaDbAsync(OnProgressChanged))
                {
                    Debug.WriteLine("[Launcher] MariaDB download/install failed");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.MariaDbInstallFailed") ??
                        "Failed to download MariaDB.\nPlease check your internet connection and try again.");
                    return;
                }
                Debug.WriteLine("[Launcher] MariaDB installed successfully");
            }
            else
            {
                Debug.WriteLine("[Launcher] MariaDB already installed");
            }

            // Step 2: Initialize data directory if needed
            Debug.WriteLine("[Launcher] Step 2: Checking data directory...");
            if (!SMariaDbPortableService.isDataInitialized())
            {
                Debug.WriteLine("[Launcher] Initializing data directory...");
                UpdateStatus(Loc.Get("Launcher.InitializingDatabase") ?? "Initializing database...");
                ShowProgressBar(true);
                await Task.Delay(100);

                if (!SMariaDbPortableService.initializeDataDirectory())
                {
                    Debug.WriteLine("[Launcher] Data directory initialization failed");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.MariaDbInitFailed") ??
                        "Failed to initialize the database.\nPlease try restarting the application.");
                    return;
                }
                Debug.WriteLine("[Launcher] Data directory initialized");
            }
            else
            {
                Debug.WriteLine("[Launcher] Data directory already initialized");
            }

            // Step 3: Start MariaDB if not running
            Debug.WriteLine("[Launcher] Step 3: Checking if MariaDB is running...");
            if (!SMariaDbPortableService.isRunning())
            {
                Debug.WriteLine("[Launcher] Starting MariaDB server...");
                UpdateStatus(Loc.Get("Launcher.StartingMariaDb") ?? "Starting database server...");
                ShowProgressBar(true);
                await Task.Delay(100);

                if (!SMariaDbPortableService.startMariaDb())
                {
                    Debug.WriteLine("[Launcher] Failed to start MariaDB");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.MariaDbStartFailed") ??
                        "Failed to start the database server.\nPlease try restarting the application.");
                    return;
                }

                Debug.WriteLine("[Launcher] Waiting for MariaDB to be ready...");
                if (!await SMariaDbPortableService.waitForMariaDbReadyAsync(30))
                {
                    Debug.WriteLine("[Launcher] MariaDB not responding");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.MariaDbConnectionFailed") ??
                        "Database server started but is not responding.\nPlease try restarting the application.");
                    return;
                }
                Debug.WriteLine("[Launcher] MariaDB is ready");
            }
            else
            {
                Debug.WriteLine("[Launcher] MariaDB already running");
            }

            // Step 4: Create database if needed
            Debug.WriteLine("[Launcher] Step 4: Checking application database...");
            if (!SMariaDbPortableService.isDatabaseCreated())
            {
                Debug.WriteLine("[Launcher] Creating application database...");
                UpdateStatus(Loc.Get("Launcher.CreatingDatabase") ?? "Creating database...");
                ShowProgressBar(true);
                await Task.Delay(100);

                if (!await SMariaDbPortableService.initializeDatabaseAsync())
                {
                    Debug.WriteLine("[Launcher] Database creation failed");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.DatabaseCreationFailed") ??
                        "Failed to create the application database.\nPlease try restarting the application.");
                    return;
                }
                Debug.WriteLine("[Launcher] Database created");
            }
            else
            {
                Debug.WriteLine("[Launcher] Database already exists");
            }

            // Step 5: Check for updates
            Debug.WriteLine("[Launcher] Step 5: Checking for updates...");
            UpdateStatus(Loc.Get("Launcher.CheckingUpdates") ?? "Checking for updates...");
            ShowProgressBar(true);
            await Task.Delay(100);

            var updateInfo = await SUpdateCheckerService.checkForUpdateInfoAsync();
            if (updateInfo != null && updateInfo.IsUpdateAvailable)
            {
                Debug.WriteLine($"[Launcher] Update available: {updateInfo.LatestVersion}");
                var currentVersion = SVersionService.GetVersion();
                var updateMessage = Loc.Get("Launcher.UpdateAvailable.Message", updateInfo.LatestVersion ?? "", currentVersion) ??
                    $"A new version ({updateInfo.LatestVersion}) is available!\n\n" +
                    $"Current version: v{currentVersion}\n\n" +
                    "Would you like to download and install the update?\n" +
                    "The application will restart after the update.";

                var userWantsUpdate = await PromptUserForConfirmationAsync(
                    updateMessage,
                    Loc.Get("Common.Yes") ?? "Yes",
                    Loc.Get("Common.No") ?? "No");

                if (userWantsUpdate)
                {
                    Debug.WriteLine("[Launcher] User accepted update, downloading...");
                    HidePrompt?.Invoke();
                    UpdateStatus(Loc.Get("Launcher.DownloadingUpdate") ?? "Downloading update...");
                    await SUpdateCheckerService.downloadAndInstallUpdateAsync(updateInfo, OnProgressChanged);
                    return; // App will restart
                }

                Debug.WriteLine("[Launcher] User declined update");
                HidePrompt?.Invoke();
            }
            else
            {
                Debug.WriteLine("[Launcher] No updates available");
            }

            // Step 6: Finalizing
            Debug.WriteLine("[Launcher] Step 6: Finalizing...");
            UpdateStatus(Loc.Get("Launcher.Starting") ?? "Starting application...");
            ShowProgressBar(true);
            await Task.Delay(300);

            Debug.WriteLine("[Launcher] Initialization completed successfully");
            OnInitializationCompleted();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Launcher] Initialization failed with exception: {ex}");
            UpdateStatus($"{Loc.Get("Launcher.Error.Message") ?? "Failed to start application:"}\n\n{ex.Message}");
            ShowProgressBar(false);
            await Task.Delay(3000);
            OnInitializationFailed();
        }
    }

    private async Task<bool> PromptUserForConfirmationAsync(string message, string yesText, string noText)
    {
        if (PromptUserAsync != null)
        {
            return await PromptUserAsync(message, yesText, noText);
        }
        return false;
    }

    private async Task ShowErrorAndFailAsync(string errorMessage)
    {
        UpdateStatus(errorMessage);
        ShowProgressBar(false);
        await Task.Delay(3000);
        OnInitializationFailed();
    }

    private void UpdateStatus(string status)
    {
        StatusChanged?.Invoke(status);
    }

    private void ShowProgressBar(bool show)
    {
        ProgressBarVisibilityChanged?.Invoke(show);
    }

    private void OnProgressChanged(string status, double? percentage)
    {
        ProgressChanged?.Invoke(status, percentage);
    }

    private void OnInitializationCompleted()
    {
        InitializationCompleted?.Invoke(this, EventArgs.Empty);
    }

    private void OnInitializationFailed()
    {
        InitializationFailed?.Invoke(this, EventArgs.Empty);
    }
}
