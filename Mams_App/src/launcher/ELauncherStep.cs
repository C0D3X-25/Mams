namespace Mams_App.src.launcher;

/// <summary>
/// Represents the different steps in the launcher initialization process.
/// </summary>
public enum ELauncherStep
{
    CheckUpdates,
    PromptUpdate,
    DownloadUpdate,
    VerifyResources,
    CheckMariaDbInstallation,
    InstallMariaDb,
    InitializeDataDirectory,
    StartMariaDb,
    CreateDatabase,
    Finalize,
    Ready,
    Failed
}
