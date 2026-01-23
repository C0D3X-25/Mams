namespace Mams_App.src.launcher.Views;

/// <summary>
/// Defines the different view types available in the launcher window.
/// </summary>
public enum ELauncherViewType
{
    /// <summary>
    /// View for MariaDB installation process.
    /// </summary>
    MariaDbInstallation,

    /// <summary>
    /// View for main application starting information.
    /// </summary>
    AppStarting,

    /// <summary>
    /// View for new update available notification.
    /// </summary>
    NewUpdate,

    /// <summary>
    /// View when the application is ready to start.
    /// </summary>
    AppReady
}
