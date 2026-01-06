using Mams_App.src.configurations;
using Mams_App.src.views.globalView;

namespace Mams_App.src.navigations;

public class UCFooterController {
    public string m_background_color { get; set; } = SGlobalView.m_footer_background_color;
    
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
