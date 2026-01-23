using Mams_App.src.launcher.userControls;
using System.Windows.Controls;

namespace Mams_App.src.launcher;

/// <summary>
/// Factory class responsible for creating launcher views based on the current step.
/// This decouples the view creation logic from both the XAML and the controller.
/// </summary>
public static class LauncherViewFactory
{
    /// <summary>
    /// Creates the appropriate view for the given launcher step.
    /// </summary>
    /// <param name="step">The current launcher step.</param>
    /// <returns>A UserControl instance for the specified step.</returns>
    public static UserControl CreateViewForStep(ELauncherStep step)
    {
        return step switch
        {
            ELauncherStep.InstallMariaDb => new UCMariaDbInstallationView(),
            ELauncherStep.PromptUpdate => new UCCheckForUpdateView(),
            ELauncherStep.DownloadUpdate => new UCDownloadUpdateView(),
            ELauncherStep.Ready => new UCAppReadyView(),
            _ => new UCAppStartingView()
        };
    }
}
