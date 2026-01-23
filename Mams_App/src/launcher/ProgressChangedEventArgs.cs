namespace Mams_App.src.launcher;

/// <summary>
/// Event arguments for progress updates.
/// </summary>
public class ProgressChangedEventArgs : EventArgs
{
    public string StatusMessage { get; }
    public double? Percentage { get; }

    public ProgressChangedEventArgs(string statusMessage, double? percentage)
    {
        StatusMessage = statusMessage;
        Percentage = percentage;
    }
}
