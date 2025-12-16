using Mams.src.navigations;
using Mams.src.configurations;
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
        var windowConfig = config.m_window;
        
        // Validate that the window position is within screen bounds
        var screenWidth = SystemParameters.VirtualScreenWidth;
        var screenHeight = SystemParameters.VirtualScreenHeight;
        
        if (windowConfig.m_left >= 0 && windowConfig.m_left < screenWidth)
        {
            Left = windowConfig.m_left;
        }
        
        if (windowConfig.m_top >= 0 && windowConfig.m_top < screenHeight)
        {
            Top = windowConfig.m_top;
        }
        
        if (windowConfig.m_width >= MinWidth && windowConfig.m_width <= screenWidth)
        {
            Width = windowConfig.m_width;
        }
        
        if (windowConfig.m_height >= MinHeight && windowConfig.m_height <= screenHeight)
        {
            Height = windowConfig.m_height;
        }
        
        if (windowConfig.m_is_maximized)
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
        var windowConfig = config.m_window;
        
        // Save the restore bounds if maximized, otherwise save current bounds
        if (WindowState == WindowState.Maximized)
        {
            windowConfig.m_left = RestoreBounds.Left;
            windowConfig.m_top = RestoreBounds.Top;
            windowConfig.m_width = RestoreBounds.Width;
            windowConfig.m_height = RestoreBounds.Height;
            windowConfig.m_is_maximized = true;
        }
        else
        {
            windowConfig.m_left = Left;
            windowConfig.m_top = Top;
            windowConfig.m_width = Width;
            windowConfig.m_height = Height;
            windowConfig.m_is_maximized = false;
        }
        
        SAppConfigService.saveConfig(config);
    }
}
