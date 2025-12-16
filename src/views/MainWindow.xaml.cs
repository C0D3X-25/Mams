using Mams.src.navigations;
using Mams.src.resources;
using Mams.src.search;
using System.Windows;

namespace Mams.src.views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {

    public MainWindow() {
        InitializeComponent();
        SPageNavigationController.initialize(MainFrame);
        SSearchModel.initialize();
        
        loadWindowSettings();
        
        Closing += MainWindow_Closing;
    }

    /// <summary>
    /// Loads window position and size from the configuration file.
    /// </summary>
    private void loadWindowSettings()
    {
        var config = SAppConfigService.loadConfig();
        
        // Validate that the window position is within screen bounds
        var screenWidth = SystemParameters.VirtualScreenWidth;
        var screenHeight = SystemParameters.VirtualScreenHeight;
        
        if (config.WindowLeft >= 0 && config.WindowLeft < screenWidth)
        {
            Left = config.WindowLeft;
        }
        
        if (config.WindowTop >= 0 && config.WindowTop < screenHeight)
        {
            Top = config.WindowTop;
        }
        
        if (config.WindowWidth >= MinWidth && config.WindowWidth <= screenWidth)
        {
            Width = config.WindowWidth;
        }
        
        if (config.WindowHeight >= MinHeight && config.WindowHeight <= screenHeight)
        {
            Height = config.WindowHeight;
        }
        
        if (config.IsMaximized)
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
        
        // Save the restore bounds if maximized, otherwise save current bounds
        if (WindowState == WindowState.Maximized)
        {
            config.WindowLeft = RestoreBounds.Left;
            config.WindowTop = RestoreBounds.Top;
            config.WindowWidth = RestoreBounds.Width;
            config.WindowHeight = RestoreBounds.Height;
            config.IsMaximized = true;
        }
        else
        {
            config.WindowLeft = Left;
            config.WindowTop = Top;
            config.WindowWidth = Width;
            config.WindowHeight = Height;
            config.IsMaximized = false;
        }
        
        SAppConfigService.saveConfig(config);
    }
}
