using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.launcher;

/// <summary>
/// Launcher window that displays startup progress while initializing the application.
/// </summary>
public partial class LauncherWindow : Window
{
    private readonly LauncherWindowController _controller;
    private TaskCompletionSource<bool>? _userResponseTcs;

    /// <summary>
    /// Event raised when initialization is complete and successful.
    /// </summary>
    public event EventHandler? InitializationCompleted;

    /// <summary>
    /// Event raised when initialization fails.
    /// </summary>
    public event EventHandler? InitializationFailed;

    public LauncherWindow()
    {
        InitializeComponent();

        _controller = new LauncherWindowController();
        VersionText.Text = $"v{_controller.Version}";

        // Wire up controller events to UI updates
        _controller.StatusChanged += OnStatusChanged;
        _controller.ProgressChanged += OnProgressChanged;
        _controller.ProgressBarVisibilityChanged += OnProgressBarVisibilityChanged;
        _controller.InitializationCompleted += OnControllerInitializationCompleted;
        _controller.InitializationFailed += OnControllerInitializationFailed;

        // Wire up UI callbacks to controller
        _controller.PromptUserAsync = ShowPromptAsync;
        _controller.HidePrompt = HidePrompt;
    }

    /// <summary>
    /// Starts the initialization process.
    /// </summary>
    public async Task startInitializationAsync()
    {
        await _controller.StartInitializationAsync();
    }

    /// <summary>
    /// Enables the Start App button when initialization is complete.
    /// </summary>
    public void EnableStartButton()
    {
        Dispatcher.Invoke(() => StartAppButton.IsEnabled = true);
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }

    private void OnStatusChanged(string status)
    {
        Dispatcher.Invoke(() => StatusText.Text = status);
    }

    private void OnProgressChanged(string status, double? percentage)
    {
        Dispatcher.Invoke(() =>
        {
            StatusText.Text = status;
            if (percentage.HasValue)
            {
                ProgressBar.IsIndeterminate = false;
                ProgressBar.Value = percentage.Value;
            }
            else
            {
                ProgressBar.IsIndeterminate = true;
            }
        });
    }

    private void OnProgressBarVisibilityChanged(bool show)
    {
        Dispatcher.Invoke(() =>
        {
            ProgressBar.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            ProgressBar.IsIndeterminate = true;
        });
    }

    private void OnControllerInitializationCompleted(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            EnableStartButton();
            if (StartWhenReadyCheckBox.IsChecked == true)
            {
                InitializationCompleted?.Invoke(this, EventArgs.Empty);
            }
        });
    }

    private void OnControllerInitializationFailed(object? sender, EventArgs e)
    {
        InitializationFailed?.Invoke(this, EventArgs.Empty);
    }

    private async Task<bool> ShowPromptAsync(string message, string yesText, string noText)
    {
        _userResponseTcs = new TaskCompletionSource<bool>();

        Dispatcher.Invoke(() =>
        {
            StatusText.Text = message;
            YesButton.Content = yesText;
            NoButton.Content = noText;
            ActionButtonsPanel.Visibility = Visibility.Visible;
            ProgressBar.Visibility = Visibility.Collapsed;
        });

        return await _userResponseTcs.Task;
    }

    private void HidePrompt()
    {
        Dispatcher.Invoke(() =>
        {
            ActionButtonsPanel.Visibility = Visibility.Collapsed;
            ProgressBar.Visibility = Visibility.Visible;
        });
    }

    private void YesButton_Click(object sender, RoutedEventArgs e)
    {
        _userResponseTcs?.TrySetResult(true);
    }

    private void NoButton_Click(object sender, RoutedEventArgs e)
    {
        _userResponseTcs?.TrySetResult(false);
    }

    private void CloseAppButton_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void StartAppButton_Click(object sender, RoutedEventArgs e)
    {
        InitializationCompleted?.Invoke(this, EventArgs.Empty);
    }
}
