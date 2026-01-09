using Mams_App.src.configurations;

namespace Mams_App.src.navigations;

public class UCFooterController
{
    /// <summary>
    /// Gets the current application version from the version.json file.
    /// </summary>
    public string m_version { get; set; }

    public UCFooterController()
    {
        m_version = $"v{SVersionService.GetVersion()}";
    }
}
