using Mams_App.src.commands;
using Mams_App.src.configurations;
using Mams_App.src.controllers;
using Mams_App.src.launcher.userControls;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Mams_App.src.launcher;

/// <summary>
/// Controller for the launcher window that handles the UI binding and delegates
/// the initialization process to the LauncherStepManager.
/// </summary>
public class LauncherWindowController : ABaseController, IDisposable
{
    private readonly LauncherStepManager _stepManager;
    private TaskCompletionSource<bool>? _userResponseTcs;
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

    private UserControl _currentView = new UCAppStartingView();
    /// <summary>
    /// Gets or sets the current view displayed in the launcher.
    /// </summary>
    public UserControl CurrentView
    {
        get => _currentView;
        set
        {
            _currentView = value;
            _currentView.DataContext = this;
            onPropertyChanged();
        }
    }

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
            if (_startWhenReady != value)
            {
                _startWhenReady = value;
                SVersionService.SetStartWhenReady(value);
                onPropertyChanged();

                // If the app is already ready and user checks the checkbox, start the app
                if (value && IsStartButtonEnabled)
                {
                    InitializationCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
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
        // Load saved StartWhenReady preference
        _startWhenReady = SVersionService.GetStartWhenReady();

        // Initialize step manager
        _stepManager = new LauncherStepManager();
        _stepManager.StepChanged += OnStepChanged;
        _stepManager.ProgressUpdated += OnProgressUpdated;
        _stepManager.UserConfirmationRequested += OnUserConfirmationRequested;
        _stepManager.InitializationCompleted += OnStepManagerInitializationCompleted;
        _stepManager.InitializationFailed += OnStepManagerInitializationFailed;

        YesCommand = new RelayCommand(_ => OnYesClicked());
        NoCommand = new RelayCommand(_ => OnNoClicked());
        CloseAppCommand = new RelayCommand(_ => OnCloseAppClicked());
        StartAppCommand = new RelayCommand(_ => OnStartAppClicked(), _ => IsStartButtonEnabled);
    }

    private void OnStepChanged(object? sender, StepChangedEventArgs e)
    {
        CurrentView = LauncherViewFactory.CreateViewForStep(e.CurrentStep);
        StatusText = e.StatusMessage;

        // Show progress bar for most steps, hide for prompts
        if (e.CurrentStep == ELauncherStep.PromptUpdate)
        {
            ProgressBarVisibility = Visibility.Collapsed;
        }
        else if (e.CurrentStep != ELauncherStep.Failed)
        {
            ProgressBarVisibility = Visibility.Visible;
            IsProgressIndeterminate = true;
        }
    }

    private void OnProgressUpdated(object? sender, ProgressChangedEventArgs e)
    {
        StatusText = e.StatusMessage;
        if (e.Percentage.HasValue)
        {
            IsProgressIndeterminate = false;
            ProgressValue = e.Percentage.Value;
        }
        else
        {
            IsProgressIndeterminate = true;
        }
    }

    private void OnUserConfirmationRequested(object? sender, UserConfirmationRequestedEventArgs e)
    {
        _userResponseTcs = e.ResponseSource;
        StatusText = e.Message;
        ActionButtonsVisibility = Visibility.Visible;
        ProgressBarVisibility = Visibility.Collapsed;
    }

    private void OnStepManagerInitializationCompleted(object? sender, EventArgs e)
    {
        EnableStartButton();
    }

    private void OnStepManagerInitializationFailed(object? sender, EventArgs e)
    {
        InitializationFailed?.Invoke(this, EventArgs.Empty);
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

        // Delegate cancellation to the step manager
        _stepManager.CancelAllOperations();
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
        Debug.WriteLine("[Launcher] Starting initialization via StepManager...");

        // Show progress bar at the start
        ShowProgressBar(true);
        CurrentView = LauncherViewFactory.CreateViewForStep(ELauncherStep.CheckMariaDbInstallation);

        await _stepManager.StartAsync();
    }

    private void EnableStartButton()
    {
        IsStartButtonEnabled = true;
        CurrentView = LauncherViewFactory.CreateViewForStep(ELauncherStep.Ready);
        System.Windows.Input.CommandManager.InvalidateRequerySuggested();

        if (StartWhenReady)
        {
            InitializationCompleted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HidePrompt()
    {
        ActionButtonsVisibility = Visibility.Collapsed;
        ProgressBarVisibility = Visibility.Visible;
    }

    private void ShowProgressBar(bool show)
    {
        ProgressBarVisibility = show ? Visibility.Visible : Visibility.Collapsed;
        IsProgressIndeterminate = true;
    }

    /// <summary>
    /// Disposes the controller and releases all resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        CancelAllOperations();

        // Unsubscribe from events
        _stepManager.StepChanged -= OnStepChanged;
        _stepManager.ProgressUpdated -= OnProgressUpdated;
        _stepManager.UserConfirmationRequested -= OnUserConfirmationRequested;
        _stepManager.InitializationCompleted -= OnStepManagerInitializationCompleted;
        _stepManager.InitializationFailed -= OnStepManagerInitializationFailed;

        _stepManager.Dispose();
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}
