using Mams_App.src.commands;
using Mams_App.src.configurations;
using Mams_App.src.controllers;
using Mams_App.src.localizations;
using Mams_App.src.services;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.launcher;

/// <summary>
/// Controller for the launcher window that handles the application initialization process.
/// </summary>
public class LauncherWindowController : ABaseController, IDisposable
{
    private TaskCompletionSource<bool>? _userResponseTcs;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _disposed;

    /// <summary>
    /// Event raised when initialization is complete and successful.
    /// </summary>
    public event EventHandler? InitializationCompleted;

    /// <summary>
    /// Event raised when initialization fails.
    /// </summary>
    public event EventHandler? InitializationFailed;

    #region Bindable Properties

    private string _statusText = "Initializing...";
    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            onPropertyChanged();
        }
    }

    private double _progressValue;
    public double ProgressValue
    {
        get => _progressValue;
        set
        {
            _progressValue = value;
            onPropertyChanged();
        }
    }

    private bool _isProgressIndeterminate = true;
    public bool IsProgressIndeterminate
    {
        get => _isProgressIndeterminate;
        set
        {
            _isProgressIndeterminate = value;
            onPropertyChanged();
        }
    }

    private Visibility _progressBarVisibility = Visibility.Visible;
    public Visibility ProgressBarVisibility
    {
        get => _progressBarVisibility;
        set
        {
            _progressBarVisibility = value;
            onPropertyChanged();
        }
    }

    private Visibility _actionButtonsVisibility = Visibility.Collapsed;
    public Visibility ActionButtonsVisibility
    {
        get => _actionButtonsVisibility;
        set
        {
            _actionButtonsVisibility = value;
            onPropertyChanged();
        }
    }

    private string _yesButtonText = "Yes";
    public string YesButtonText
    {
        get => _yesButtonText;
        set
        {
            _yesButtonText = value;
            onPropertyChanged();
        }
    }

    private string _noButtonText = "No";
    public string NoButtonText
    {
        get => _noButtonText;
        set
        {
            _noButtonText = value;
            onPropertyChanged();
        }
    }

    private bool _isStartButtonEnabled;
    public bool IsStartButtonEnabled
    {
        get => _isStartButtonEnabled;
        set
        {
            _isStartButtonEnabled = value;
            onPropertyChanged();
        }
    }

    private bool _startWhenReady;
    public bool StartWhenReady
    {
        get => _startWhenReady;
        set
        {
            _startWhenReady = value;
            onPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ICommand YesCommand { get; }
    public ICommand NoCommand { get; }
    public ICommand CloseAppCommand { get; }
    public ICommand StartAppCommand { get; }

    #endregion

    /// <summary>
    /// Gets the current application version.
    /// </summary>
    public string Version => $"v{SVersionService.GetVersion()}";

    public LauncherWindowController()
    {
        YesCommand = new RelayCommand(_ => OnYesClicked());
        NoCommand = new RelayCommand(_ => OnNoClicked());
        CloseAppCommand = new RelayCommand(_ => OnCloseAppClicked());
        StartAppCommand = new RelayCommand(_ => OnStartAppClicked(), _ => IsStartButtonEnabled);
    }

    private void OnYesClicked()
    {
        _userResponseTcs?.TrySetResult(true);
    }

    private void OnNoClicked()
    {
        _userResponseTcs?.TrySetResult(false);
    }

    private void OnCloseAppClicked()
    {
        CancelAllOperations();
        Application.Current.Shutdown();
    }

    /// <summary>
    /// Cancels all ongoing operations and cleans up resources.
    /// </summary>
    public void CancelAllOperations()
    {
        Debug.WriteLine("[Launcher] Cancelling all operations...");
        
        // Cancel any pending user response
        _userResponseTcs?.TrySetCanceled();
        
        // Cancel all async operations
        _cancellationTokenSource?.Cancel();
        
        // Cancel any ongoing downloads in services
        SMariaDbPortableService.cancelDownload();
        SUpdateCheckerService.cancelDownload();
    }

    private void OnStartAppClicked()
    {
        InitializationCompleted?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Starts the initialization process.
    /// </summary>
    public async Task StartInitializationAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        try
        {
            Debug.WriteLine("[Launcher] Starting initialization...");

            // Show progress bar at the start
            ShowProgressBar(true);

            // Step 1: Check MariaDB installation
            Debug.WriteLine("[Launcher] Step 1: Checking MariaDB installation...");
            UpdateStatus(Loc.Get("Launcher.CheckingMariaDb") ?? "Checking database installation...");
            await Task.Delay(100, cancellationToken);

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
                    Loc.Get("Common.No") ?? "No",
                    cancellationToken);

                if (!userWantsInstall)
                {
                    Debug.WriteLine("[Launcher] User declined MariaDB installation");
                    OnInitializationFailed();
                    return;
                }

                Debug.WriteLine("[Launcher] User accepted, starting MariaDB download...");
                HidePrompt();
                ShowProgressBar(true);
                UpdateStatus(Loc.Get("Launcher.InstallingMariaDb") ?? "Installing database...");

                if (!await SMariaDbPortableService.downloadAndInstallMariaDbAsync(OnProgressChanged, cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("[Launcher] MariaDB download cancelled");
                        return;
                    }
                    Debug.WriteLine("[Launcher] MariaDB download/install failed");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.MariaDbInstallFailed") ??
                        "Failed to download MariaDB.\nPlease check your internet connection and try again.",
                        cancellationToken);
                    return;
                }
                Debug.WriteLine("[Launcher] MariaDB installed successfully");
            }
            else
            {
                Debug.WriteLine("[Launcher] MariaDB already installed");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 2: Initialize data directory if needed
            Debug.WriteLine("[Launcher] Step 2: Checking data directory...");
            if (!SMariaDbPortableService.isDataInitialized())
            {
                Debug.WriteLine("[Launcher] Initializing data directory...");
                UpdateStatus(Loc.Get("Launcher.InitializingDatabase") ?? "Initializing database...");
                ShowProgressBar(true);
                await Task.Delay(100, cancellationToken);

                if (!await SMariaDbPortableService.initializeDataDirectoryAsync(cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("[Launcher] Data directory initialization cancelled");
                        return;
                    }
                    Debug.WriteLine("[Launcher] Data directory initialization failed");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.MariaDbInitFailed") ??
                        "Failed to initialize the database.\nPlease try restarting the application.",
                        cancellationToken);
                    return;
                }
                Debug.WriteLine("[Launcher] Data directory initialized");
            }
            else
            {
                Debug.WriteLine("[Launcher] Data directory already initialized");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 3: Start MariaDB if not running
            Debug.WriteLine("[Launcher] Step 3: Checking if MariaDB is running...");
            if (!await SMariaDbPortableService.isRunningAsync(cancellationToken))
            {
                Debug.WriteLine("[Launcher] Starting MariaDB server...");
                UpdateStatus(Loc.Get("Launcher.StartingMariaDb") ?? "Starting database server...");
                ShowProgressBar(true);
                await Task.Delay(100, cancellationToken);

                if (!await SMariaDbPortableService.startMariaDbAsync())
                {
                    Debug.WriteLine("[Launcher] Failed to start MariaDB");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.MariaDbStartFailed") ??
                        "Failed to start the database server.\nPlease try restarting the application.",
                        cancellationToken);
                    return;
                }

                Debug.WriteLine("[Launcher] Waiting for MariaDB to be ready...");
                if (!await SMariaDbPortableService.waitForMariaDbReadyAsync(30, cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("[Launcher] MariaDB wait cancelled");
                        return;
                    }
                    Debug.WriteLine("[Launcher] MariaDB not responding");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.MariaDbConnectionFailed") ??
                        "Database server started but is not responding.\nPlease try restarting the application.",
                        cancellationToken);
                    return;
                }
                Debug.WriteLine("[Launcher] MariaDB is ready");
            }
            else
            {
                Debug.WriteLine("[Launcher] MariaDB already running");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 4: Create database if needed
            Debug.WriteLine("[Launcher] Step 4: Checking application database...");
            if (!SMariaDbPortableService.isDatabaseCreated())
            {
                Debug.WriteLine("[Launcher] Creating application database...");
                UpdateStatus(Loc.Get("Launcher.CreatingDatabase") ?? "Creating database...");
                ShowProgressBar(true);
                await Task.Delay(100, cancellationToken);

                if (!await SMariaDbPortableService.initializeDatabaseAsync(cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("[Launcher] Database creation cancelled");
                        return;
                    }
                    Debug.WriteLine("[Launcher] Database creation failed");
                    await ShowErrorAndFailAsync(
                        Loc.Get("Launcher.DatabaseCreationFailed") ??
                        "Failed to create the application database.\nPlease try restarting the application.",
                        cancellationToken);
                    return;
                }
                Debug.WriteLine("[Launcher] Database created");
            }
            else
            {
                Debug.WriteLine("[Launcher] Database already exists");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 5: Check for updates
            Debug.WriteLine("[Launcher] Step 5: Checking for updates...");
            UpdateStatus(Loc.Get("Launcher.CheckingUpdates") ?? "Checking for updates...");
            ShowProgressBar(true);
            await Task.Delay(100, cancellationToken);

            var updateInfo = await SUpdateCheckerService.checkForUpdateInfoAsync(cancellationToken);
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
                    Loc.Get("Common.No") ?? "No",
                    cancellationToken);

                if (userWantsUpdate)
                {
                    Debug.WriteLine("[Launcher] User accepted update, downloading...");
                    HidePrompt();
                    UpdateStatus(Loc.Get("Launcher.DownloadingUpdate") ?? "Downloading update...");
                    await SUpdateCheckerService.downloadAndInstallUpdateAsync(updateInfo, OnProgressChanged, cancellationToken);
                    return; // App will restart
                }

                Debug.WriteLine("[Launcher] User declined update");
                HidePrompt();
            }
            else
            {
                Debug.WriteLine("[Launcher] No updates available");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 6: Finalizing
            Debug.WriteLine("[Launcher] Step 6: Finalizing...");
            UpdateStatus(Loc.Get("Launcher.Starting") ?? "Starting application...");
            ShowProgressBar(true);
            await Task.Delay(300, cancellationToken);

            Debug.WriteLine("[Launcher] Initialization completed successfully");
            EnableStartButton();
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[Launcher] Initialization was cancelled");
            // Don't show error, just exit silently
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[Launcher] Initialization failed with exception: {ex}");
            UpdateStatus($"{Loc.Get("Launcher.Error.Message") ?? "Failed to start application:"}\n\n{ex.Message}");
            ShowProgressBar(false);
            try
            {
                await Task.Delay(3000, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // Ignore cancellation during error display
            }
            OnInitializationFailed();
        }
    }

    private void EnableStartButton()
    {
        IsStartButtonEnabled = true;
        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
        
        if (StartWhenReady)
        {
            InitializationCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    private async Task<bool> PromptUserForConfirmationAsync(string message, string yesText, string noText, CancellationToken cancellationToken = default)
    {
        _userResponseTcs = new TaskCompletionSource<bool>();

        // Register cancellation
        await using var registration = cancellationToken.Register(() => _userResponseTcs.TrySetCanceled());

        StatusText = message;
        YesButtonText = yesText;
        NoButtonText = noText;
        ActionButtonsVisibility = Visibility.Visible;
        ProgressBarVisibility = Visibility.Collapsed;

        return await _userResponseTcs.Task;
    }

    private void HidePrompt()
    {
        ActionButtonsVisibility = Visibility.Collapsed;
        ProgressBarVisibility = Visibility.Visible;
    }

    private async Task ShowErrorAndFailAsync(string errorMessage, CancellationToken cancellationToken = default)
    {
        UpdateStatus(errorMessage);
        ShowProgressBar(false);
        try
        {
            await Task.Delay(3000, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation during error display
        }
        OnInitializationFailed();
    }

    private void UpdateStatus(string status)
    {
        StatusText = status;
    }

    private void ShowProgressBar(bool show)
    {
        ProgressBarVisibility = show ? Visibility.Visible : Visibility.Collapsed;
        IsProgressIndeterminate = true;
    }

    private void OnProgressChanged(string status, double? percentage)
    {
        StatusText = status;
        if (percentage.HasValue)
        {
            IsProgressIndeterminate = false;
            ProgressValue = percentage.Value;
        }
        else
        {
            IsProgressIndeterminate = true;
        }
    }

    private void OnInitializationFailed()
    {
        InitializationFailed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Disposes the controller and releases all resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        
        CancelAllOperations();
        _cancellationTokenSource?.Dispose();
        _disposed = true;
        
        GC.SuppressFinalize(this);
    }
}
