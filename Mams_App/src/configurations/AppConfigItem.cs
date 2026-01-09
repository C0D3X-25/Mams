using System.Text.Json.Serialization;

namespace Mams_App.src.configurations;

/// <summary>
/// Represents the application configuration settings.
/// </summary>
public class AppConfigItem
{
    /// <summary>
    /// Gets or sets the window configuration settings.
    /// </summary>
    [JsonPropertyName("window")]
    public WindowConfigItem m_window { get; set; } = new();

    /// <summary>
    /// Gets or sets the localization configuration settings.
    /// </summary>
    [JsonPropertyName("localization")]
    public LocalizationConfigItem m_localization { get; set; } = new();
}

/// <summary>
/// Represents the window configuration settings.
/// </summary>
public class WindowConfigItem
{
    /// <summary>
    /// Gets or sets the window left position.
    /// </summary>
    [JsonPropertyName("left")]
    public double m_left { get; set; } = 100;

    /// <summary>
    /// Gets or sets the window top position.
    /// </summary>
    [JsonPropertyName("top")]
    public double m_top { get; set; } = 100;

    /// <summary>
    /// Gets or sets the window width.
    /// </summary>
    [JsonPropertyName("width")]
    public double m_width { get; set; } = 1224;

    /// <summary>
    /// Gets or sets the window height.
    /// </summary>
    [JsonPropertyName("height")]
    public double m_height { get; set; } = 800;

    /// <summary>
    /// Gets or sets whether the window is maximized.
    /// </summary>
    [JsonPropertyName("is_maximized")]
    public bool m_is_maximized { get; set; } = false;
}

/// <summary>
/// Represents the localization configuration settings.
/// </summary>
public class LocalizationConfigItem
{
    /// <summary>
    /// Gets or sets the language code (e.g., "en", "fr", "de").
    /// </summary>
    [JsonPropertyName("language")]
    public string m_language { get; set; } = "fr";

    /// <summary>
    /// Gets or sets the culture code for currency and number formatting (e.g., "fr-CH", "en-US", "de-DE").
    /// </summary>
    [JsonPropertyName("culture")]
    public string m_culture { get; set; } = "fr-CH";
}
