namespace Mams_App.src.launcher;

/// <summary>
/// Event arguments for user confirmation requests.
/// </summary>
public class UserConfirmationRequestedEventArgs(string message, TaskCompletionSource<bool> responseSource) : EventArgs
{
    public string Message { get; } = message;
    public TaskCompletionSource<bool> ResponseSource { get; } = responseSource;
}
