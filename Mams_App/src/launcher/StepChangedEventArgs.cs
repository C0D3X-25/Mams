namespace Mams_App.src.launcher;

/// <summary>
/// Event arguments for step change events.
/// </summary>
public class StepChangedEventArgs : EventArgs
{
    public ELauncherStep PreviousStep { get; }
    public ELauncherStep CurrentStep { get; }
    public ELauncherStepView ViewType { get; }
    public string StatusMessage { get; }

    public StepChangedEventArgs(ELauncherStep previousStep, ELauncherStep currentStep, ELauncherStepView viewType, string statusMessage)
    {
        PreviousStep = previousStep;
        CurrentStep = currentStep;
        ViewType = viewType;
        StatusMessage = statusMessage;
    }
}
