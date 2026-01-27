using Mams_App.src.configurations;
using Mams_App.src.localizations;
using Mams_App.src.services;
using System.Diagnostics;

namespace Mams_App.src.launcher;

/// <summary>
/// Manages the step-by-step initialization process for the launcher.
/// Separates the business logic from the UI controller.
/// </summary>
public class LauncherStepManager : IDisposable
{
    private ELauncherStep _currentStep = ELauncherStep.CheckUpdates;
    private CancellationTokenSource? _cancellationTokenSource;
    private bool _disposed;

    /// <summary>
    /// Event raised when the current step changes.
    /// </summary>
    public event EventHandler<StepChangedEventArgs>? StepChanged;

    /// <summary>
    /// Event raised when progress is updated.
    /// </summary>
    public event EventHandler<ProgressChangedEventArgs>? ProgressUpdated;

    /// <summary>
    /// Event raised when user confirmation is needed.
    /// </summary>
    public event EventHandler<UserConfirmationRequestedEventArgs>? UserConfirmationRequested;

    /// <summary>
    /// Event raised when initialization completes successfully.
    /// </summary>
    public event EventHandler? InitializationCompleted;

    /// <summary>
    /// Event raised when initialization fails.
    /// </summary>
    public event EventHandler? InitializationFailed;

    /// <summary>
    /// Gets the current step in the initialization process.
    /// </summary>
    public ELauncherStep CurrentStep => _currentStep;

    /// <summary>
    /// Gets the view type corresponding to the current step.
    /// </summary>
    public ELauncherStepView CurrentViewType => GetViewTypeForStep(_currentStep);

    /// <summary>
    /// Starts the initialization process asynchronously.
    /// </summary>
    public async Task StartAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = _cancellationTokenSource.Token;

        try
        {
            Debug.WriteLine("[LauncherStepManager] Starting initialization...");

            // Step 1: Check for updates (do this first, before MariaDB, in case update changes MariaDB installation)
            await ExecuteStepAsync(ELauncherStep.CheckUpdates, cancellationToken);

            var updateInfo = await SUpdateCheckerService.checkForUpdateInfoAsync(cancellationToken);
            if (updateInfo != null && updateInfo.IsUpdateAvailable)
            {
                Debug.WriteLine($"[LauncherStepManager] Update available: {updateInfo.LatestVersion}");
                await ExecuteStepAsync(ELauncherStep.PromptUpdate, cancellationToken);

                var currentVersion = SVersionService.GetVersion();
                var updateMessage = Loc.Get("Launcher.UpdateAvailable.Message", updateInfo.LatestVersion ?? "", currentVersion) ??
                    $"A new version ({updateInfo.LatestVersion}) is available!\n\n" +
                    $"Current version: v{currentVersion}\n\n" +
                    "Would you like to download and install the update?\n" +
                    "The application will restart after the update.";

                var userWantsUpdate = await RequestUserConfirmationAsync(
                    updateMessage,
                    cancellationToken);

                if (userWantsUpdate)
                {
                    Debug.WriteLine("[LauncherStepManager] User accepted update, downloading...");
                    await ExecuteStepAsync(ELauncherStep.DownloadUpdate, cancellationToken);
                    await SUpdateCheckerService.downloadAndInstallUpdateAsync(updateInfo, OnProgressChanged, cancellationToken);
                    return; // App will restart
                }

                Debug.WriteLine("[LauncherStepManager] User declined update");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 2: Verify resources (localization files, etc.)
            await ExecuteStepAsync(ELauncherStep.VerifyResources, cancellationToken);

            var integrityResult = await SIntegrityService.verifyAndRepairAsync(OnProgressChanged, cancellationToken);
            if (!integrityResult.IsValid)
            {
                Debug.WriteLine($"[LauncherStepManager] Resource verification failed: {integrityResult.ErrorMessage}");
                // Don't fail startup for missing localizations - app can still work with fallback keys
                // Just log the issue
            }
            else if (integrityResult.RepairedFiles.Count > 0)
            {
                Debug.WriteLine($"[LauncherStepManager] Resources repaired: {string.Join(", ", integrityResult.RepairedFiles)}");
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 3: Check MariaDB installation
            await ExecuteStepAsync(ELauncherStep.CheckMariaDbInstallation, cancellationToken);

            if (!SMariaDbPortableService.isInstalled())
            {
                // Step 3: Install MariaDB
                await ExecuteStepAsync(ELauncherStep.InstallMariaDb, cancellationToken);

                if (!await SMariaDbPortableService.downloadAndInstallMariaDbAsync(OnProgressChanged, cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("[LauncherStepManager] MariaDB download cancelled");
                        return;
                    }
                    await FailWithErrorAsync(
                        Loc.Get("Launcher.MariaDbInstallFailed") ??
                        "Failed to download MariaDB.\nPlease check your internet connection and try again.",
                        cancellationToken);
                    return;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 4: Initialize data directory
            await ExecuteStepAsync(ELauncherStep.InitializeDataDirectory, cancellationToken);

            if (!SMariaDbPortableService.isDataInitialized())
            {
                if (!await SMariaDbPortableService.initializeDataDirectoryAsync(cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("[LauncherStepManager] Data directory initialization cancelled");
                        return;
                    }
                    await FailWithErrorAsync(
                        Loc.Get("Launcher.MariaDbInitFailed") ??
                        "Failed to initialize the database.\nPlease try restarting the application.",
                        cancellationToken);
                    return;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 5: Start MariaDB
            await ExecuteStepAsync(ELauncherStep.StartMariaDb, cancellationToken);

            if (!await SMariaDbPortableService.isRunningAsync(cancellationToken))
            {
                if (!await SMariaDbPortableService.startMariaDbAsync())
                {
                    await FailWithErrorAsync(
                        Loc.Get("Launcher.MariaDbStartFailed") ??
                        "Failed to start the database server.\nPlease try restarting the application.",
                        cancellationToken);
                    return;
                }

                if (!await SMariaDbPortableService.waitForMariaDbReadyAsync(30, cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("[LauncherStepManager] MariaDB wait cancelled");
                        return;
                    }
                    await FailWithErrorAsync(
                        Loc.Get("Launcher.MariaDbConnectionFailed") ??
                        "Database server started but is not responding.\nPlease try restarting the application.",
                        cancellationToken);
                    return;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 6: Create database
            await ExecuteStepAsync(ELauncherStep.CreateDatabase, cancellationToken);

            if (!SMariaDbPortableService.isDatabaseCreated())
            {
                if (!await SMariaDbPortableService.initializeDatabaseAsync(cancellationToken))
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Debug.WriteLine("[LauncherStepManager] Database creation cancelled");
                        return;
                    }
                    await FailWithErrorAsync(
                        Loc.Get("Launcher.DatabaseCreationFailed") ??
                        "Failed to create the application database.\nPlease try restarting the application.",
                        cancellationToken);
                    return;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Step 7: Finalize
            await ExecuteStepAsync(ELauncherStep.Finalize, cancellationToken);
            await Task.Delay(300, cancellationToken);

            // Step 8: Ready
            await ExecuteStepAsync(ELauncherStep.Ready, cancellationToken);
            Debug.WriteLine("[LauncherStepManager] Initialization completed successfully");
            InitializationCompleted?.Invoke(this, EventArgs.Empty);
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[LauncherStepManager] Initialization was cancelled");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[LauncherStepManager] Initialization failed with exception: {ex}");
            await FailWithErrorAsync(
                $"{Loc.Get("Launcher.Error.Message") ?? "Failed to start application:"}\n\n{ex.Message}",
                cancellationToken);
        }
    }

    /// <summary>
    /// Cancels all ongoing operations.
    /// </summary>
    public void CancelAllOperations()
    {
        Debug.WriteLine("[LauncherStepManager] Cancelling all operations...");
        _cancellationTokenSource?.Cancel();
        SMariaDbPortableService.cancelDownload();
        SUpdateCheckerService.cancelDownload();
    }

    private async Task ExecuteStepAsync(ELauncherStep step, CancellationToken cancellationToken)
    {
        var previousStep = _currentStep;
        _currentStep = step;

        var statusMessage = GetStatusMessageForStep(step);
        var viewType = GetViewTypeForStep(step);

        Debug.WriteLine($"[LauncherStepManager] Executing step: {step}");

        StepChanged?.Invoke(this, new StepChangedEventArgs(previousStep, step, viewType, statusMessage));

        await Task.Delay(100, cancellationToken);
    }

    private async Task<bool> RequestUserConfirmationAsync(string message, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();

        await using var registration = cancellationToken.Register(() => tcs.TrySetCanceled());

        UserConfirmationRequested?.Invoke(this, new UserConfirmationRequestedEventArgs(message, tcs));

        return await tcs.Task;
    }

    private async Task FailWithErrorAsync(string errorMessage, CancellationToken cancellationToken)
    {
        _currentStep = ELauncherStep.Failed;
        ProgressUpdated?.Invoke(this, new ProgressChangedEventArgs(errorMessage, null));

        try
        {
            await Task.Delay(3000, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation during error display
        }

        InitializationFailed?.Invoke(this, EventArgs.Empty);
    }

    private void OnProgressChanged(string status, double? percentage)
    {
        ProgressUpdated?.Invoke(this, new ProgressChangedEventArgs(status, percentage));
    }

    private static ELauncherStepView GetViewTypeForStep(ELauncherStep step)
    {
        return step switch
        {
            ELauncherStep.InstallMariaDb => ELauncherStepView.MariaDbInstallation,
            ELauncherStep.PromptUpdate => ELauncherStepView.CheckForUpdate,
            ELauncherStep.DownloadUpdate => ELauncherStepView.DownloadUpdate,
            ELauncherStep.Ready => ELauncherStepView.AppReady,
            _ => ELauncherStepView.AppStarting
        };
    }

    private static string GetStatusMessageForStep(ELauncherStep step)
    {
        return step switch
        {
            ELauncherStep.CheckMariaDbInstallation => Loc.Get("Launcher.CheckingMariaDb") ?? "Checking database installation...",
            ELauncherStep.InstallMariaDb => Loc.Get("Launcher.InstallingMariaDb") ?? "Installing database...",
            ELauncherStep.InitializeDataDirectory => Loc.Get("Launcher.InitializingDatabase") ?? "Initializing database...",
            ELauncherStep.StartMariaDb => Loc.Get("Launcher.StartingMariaDb") ?? "Starting database server...",
            ELauncherStep.CreateDatabase => Loc.Get("Launcher.CreatingDatabase") ?? "Creating database...",
            ELauncherStep.CheckUpdates => Loc.Get("Launcher.CheckingUpdates") ?? "Checking for updates...",
            ELauncherStep.PromptUpdate => Loc.Get("Launcher.UpdateAvailable") ?? "Update available",
            ELauncherStep.DownloadUpdate => Loc.Get("Launcher.DownloadingUpdate") ?? "Downloading update...",
            ELauncherStep.VerifyResources => Loc.Get("Launcher.VerifyingResources") ?? "Verifying resources...",
            ELauncherStep.Finalize => Loc.Get("Launcher.Starting") ?? "Starting application...",
            ELauncherStep.Ready => Loc.Get("Launcher.Ready") ?? "Application is ready",
            ELauncherStep.Failed => Loc.Get("Launcher.Error") ?? "Error",
            _ => "Initializing..."
        };
    }

    /// <summary>
    /// Disposes the step manager and releases all resources.
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
