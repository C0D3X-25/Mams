using Mams_App.src.configurations;

namespace Mams_App.src.navigations;

public class UCFooterController
{
    /// <summary>
    /// Gets the current application version from the config file.
    /// </summary>
    public string m_version { get; set; }

    public UCFooterController()
    {
        var config = SAppConfigService.loadConfig();
        m_version = $"v{config.m_version}";
    }
}
