using Mams.src.views;
using System.Windows;
using System.Windows.Controls;

namespace Mams.src.settings;

/// <summary>
/// Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        var controller = new SettingsController(this);
        DataContext = controller;
    }

    /// <summary>
    /// Shows the settings window as a modal dialog.
    /// </summary>
    /// <param name="owner">The owner window</param>
    public static void ShowSettings(Window? owner = null)
    {
        var settingsWindow = new SettingsWindow();
        if (owner != null)
        {
            settingsWindow.Owner = owner;
        }
        settingsWindow.ShowDialog();
    }
}
