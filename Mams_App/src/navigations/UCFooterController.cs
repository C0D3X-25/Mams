using Mams.src.configurations;
using Mams.src.views.globalView;

namespace Mams.src.navigations;

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
