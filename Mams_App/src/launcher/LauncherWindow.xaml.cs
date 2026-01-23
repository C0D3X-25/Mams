using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.launcher;

/// <summary>
/// Launcher window that displays startup progress while initializing the application.
/// </summary>
public partial class LauncherWindow : Window
{
    private readonly LauncherWindowController _controller;

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
        DataContext = _controller;

        _controller.InitializationCompleted += (s, e) => InitializationCompleted?.Invoke(this, EventArgs.Empty);
        _controller.InitializationFailed += (s, e) => InitializationFailed?.Invoke(this, EventArgs.Empty);

        // Clean up when window is closing
        Closing += (s, e) => _controller.Dispose();
    }

    /// <summary>
    /// Starts the initialization process.
    /// </summary>
    public async Task StartInitializationAsync()
    {
        await _controller.StartInitializationAsync();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
}
