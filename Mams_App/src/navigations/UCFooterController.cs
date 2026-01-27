using Mams_App.src.configurations;

namespace Mams_App.src.navigations;

/// <summary>
/// Controller for the footer user control, displaying application version information.
/// </summary>
public class UCFooterController
{
    /// <summary>
    /// Gets the current application version from the version.json file.
    /// </summary>
    public string m_version { get; set; }

    /// <summary>
    /// Initializes a new instance of the UCFooterController class.
    /// </summary>
    public UCFooterController()
    {
        m_version = $"v{SVersionService.GetVersion()}";
    }
}
