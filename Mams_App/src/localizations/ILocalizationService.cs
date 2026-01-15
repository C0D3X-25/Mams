namespace Mams_App.src.localizations;

/// <summary>
/// Interface for localization service providing access to localized strings.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets the current culture code (e.g., "fr", "en").
    /// </summary>
    string CurrentCulture { get; }

    /// <summary>
    /// Gets a localized string by its key.
    /// </summary>
    /// <param name="key">The key of the string to retrieve.</param>
    /// <returns>The localized string, or the key if not found.</returns>
    string GetString(string key);

    /// <summary>
    /// Gets a localized string by its key with format arguments.
    /// </summary>
    /// <param name="key">The key of the string to retrieve.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>The formatted localized string.</returns>
    string GetString(string key, params object[] args);

    /// <summary>
    /// Changes the current culture and reloads strings.
    /// </summary>
    /// <param name="cultureCode">The culture code to switch to (e.g., "fr", "en").</param>
    void SetCulture(string cultureCode);

    /// <summary>
    /// Event raised when the culture changes.
    /// </summary>
    event EventHandler? CultureChanged;

    /// <summary>
    /// Gets the currency symbol based on the configured culture.
    /// </summary>
    /// <returns>The currency symbol (e.g., "CHF", "€", "$").</returns>
    string GetCurrencySymbol();
}
