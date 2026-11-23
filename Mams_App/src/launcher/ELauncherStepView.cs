namespace Mams_App.src.launcher;

/// <summary>
/// Defines the different view types available in the launcher window.
/// </summary>
public enum ELauncherStepView
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
    /// View for prompting the user about an available update.
    /// </summary>
    CheckForUpdate,

    /// <summary>
    /// View for downloading and installing an update.
    /// </summary>
    DownloadUpdate,

    /// <summary>
    /// View when the application is ready to start.
    /// </summary>
    AppReady
}
