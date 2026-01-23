namespace Mams_App.src.launcher;

/// <summary>
/// Represents the different steps in the launcher initialization process.
/// </summary>
public enum ELauncherStep
{
    CheckMariaDbInstallation,
    InstallMariaDb,
    InitializeDataDirectory,
    StartMariaDb,
    CreateDatabase,
    CheckUpdates,
    PromptUpdate,
    DownloadUpdate,
    Finalize,
    Ready,
    Failed
}
