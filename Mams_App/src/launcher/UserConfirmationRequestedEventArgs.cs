namespace Mams_App.src.launcher;

/// <summary>
/// Event arguments for user confirmation requests.
/// </summary>
public class UserConfirmationRequestedEventArgs : EventArgs
{
    public string Message { get; }
    public string YesText { get; }
    public string NoText { get; }
    public TaskCompletionSource<bool> ResponseSource { get; }

    public UserConfirmationRequestedEventArgs(string message, string yesText, string noText, TaskCompletionSource<bool> responseSource)
    {
        Message = message;
        YesText = yesText;
        NoText = noText;
        ResponseSource = responseSource;
    }
}
