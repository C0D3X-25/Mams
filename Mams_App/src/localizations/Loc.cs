using Mams_App.src.configurations;

namespace Mams_App.src.localizations;

/// <summary>
/// Static accessor for localization strings.
/// Provides easy access to localized text throughout the application.
/// </summary>
public static class Loc
{
    private static ILocalizationService? _service;

    /// <summary>
    /// Gets the localization service instance.
    /// Automatically initializes from settings.json if not already initialized.
    /// </summary>
    public static ILocalizationService Service
    {
        get
        {
            if (_service == null)
            {
                var config = SAppConfigService.loadConfig();
                _service = new LocalizationService(config.m_localization.m_language);
            }
            return _service;
        }
    }

    /// <summary>
    /// Initializes the localization system from settings.json.
    /// Call this at application startup.
    /// </summary>
    public static void Initialize()
    {
        var config = SAppConfigService.loadConfig();
        _service = new LocalizationService(config.m_localization.m_language);
    }

    /// <summary>
    /// Initializes the localization system with a specific language.
    /// </summary>
    /// <param name="language">The language code (e.g., "fr", "en").</param>
    public static void Initialize(string language)
    {
        _service = new LocalizationService(language);
    }

    /// <summary>
    /// Gets a localized string by key.
    /// Shorthand for Loc.Service.GetString(key).
    /// </summary>
    /// <param name="key">The string key.</param>
    /// <returns>The localized string.</returns>
    public static string Get(string key) => Service.GetString(key);

    /// <summary>
    /// Gets a localized string with format arguments.
    /// </summary>
    /// <param name="key">The string key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    public static string Get(string key, params object[] args) => Service.GetString(key, args);

    /// <summary>
    /// Gets the currency symbol based on the configured culture.
    /// Use this instead of Loc.Get("Unit.CHF") for dynamic currency display.
    /// </summary>
    public static string Currency => Service.GetCurrencySymbol();

    /// <summary>
    /// Changes the current culture.
    /// </summary>
    /// <param name="cultureCode">The culture code to switch to.</param>
    public static void SetCulture(string cultureCode) => Service.SetCulture(cultureCode);
}
