using Mams_App.src.configurations;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Mams_App.src.localizations;

/// <summary>
/// JSON-based localization service that loads strings from JSON files.
/// </summary>
public class LocalizationService : ILocalizationService
{
    private const string DefaultCulture = "fr";
    private const string LocalizationFolder = "resources/localization";

    private Dictionary<string, string> _strings = [];
    private string _currentCulture = DefaultCulture;

    public string CurrentCulture => _currentCulture;

    public event EventHandler? CultureChanged;

    public LocalizationService()
    {
        LoadStrings(DefaultCulture);
    }

    public LocalizationService(string cultureCode)
    {
        LoadStrings(cultureCode);
    }

    public string GetString(string key)
    {
        if (_strings.TryGetValue(key, out var value))
        {
            return value;
        }

        // Return the key itself if not found (helps identify missing translations)
        return $"[{key}]";
    }

    public string GetString(string key, params object[] args)
    {
        var template = GetString(key);
        try
        {
            return string.Format(template, args);
        }
        catch (FormatException)
        {
            return template;
        }
    }

    public void SetCulture(string cultureCode)
    {
        if (_currentCulture == cultureCode)
            return;

        LoadStrings(cultureCode);
        CultureChanged?.Invoke(this, EventArgs.Empty);
    }

    private void LoadStrings(string cultureCode)
    {
        _currentCulture = cultureCode;
        _strings = [];

        var filePath = GetLocalizationFilePath(cultureCode);

        if (!File.Exists(filePath))
        {
            // Fallback to default culture if requested culture file doesn't exist
            if (cultureCode != DefaultCulture)
            {
                filePath = GetLocalizationFilePath(DefaultCulture);
            }
        }

        if (File.Exists(filePath))
        {
            try
            {
                var json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    ReadCommentHandling = JsonCommentHandling.Skip
                };

                _strings = JsonSerializer.Deserialize<Dictionary<string, string>>(json, options) ?? [];
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading localization file: {ex.Message}");
            }
        }
    }

    private static string GetLocalizationFilePath(string cultureCode)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        return Path.Combine(baseDirectory, LocalizationFolder, $"{cultureCode}.json");
    }

    /// <summary>
    /// Gets the currency symbol based on the configured culture from settings.
    /// </summary>
    /// <returns>The currency symbol (e.g., "CHF", "€", "$").</returns>
    public string GetCurrencySymbol()
    {
        try
        {
            var config = SAppConfigService.loadConfig();
            var cultureCode = config.m_localization.m_culture;
            var cultureInfo = new CultureInfo(cultureCode);
            return cultureInfo.NumberFormat.CurrencySymbol;
        }
        catch
        {
            // Fallback to CHF if culture is invalid
            return "CHF";
        }
    }
}
