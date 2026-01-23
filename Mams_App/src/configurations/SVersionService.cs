using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mams_App.src.configurations;

/// <summary>
/// Static service class for reading and writing application settings from version.json.
/// </summary>
public static class SVersionService
{
    private static readonly string _versionFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "resources", "version.json");

    private static VersionInfo? _cachedVersionInfo;

    /// <summary>
    /// Clears the cached version info, forcing the next read to load from disk.
    /// This should be called at app startup to ensure fresh data after an update.
    /// </summary>
    public static void ClearCache()
    {
        _cachedVersionInfo = null;
    }

    /// <summary>
    /// Gets the application version from the version.json file.
    /// </summary>
    /// <returns>The application version string, or "1.0.0" if not found.</returns>
    public static string GetVersion()
    {
        return GetVersionInfo().Version;
    }

    /// <summary>
    /// Gets the StartWhenReady setting from the version.json file.
    /// </summary>
    /// <returns>True if the app should start automatically when ready, false otherwise.</returns>
    public static bool GetStartWhenReady()
    {
        return GetVersionInfo().StartWhenReady;
    }

    /// <summary>
    /// Sets and saves the StartWhenReady setting to the version.json file.
    /// </summary>
    /// <param name="value">The value to set.</param>
    public static void SetStartWhenReady(bool value)
    {
        var versionInfo = GetVersionInfo();
        if (versionInfo.StartWhenReady != value)
        {
            versionInfo.StartWhenReady = value;
            SaveVersionInfo(versionInfo);
        }
    }

    private static VersionInfo GetVersionInfo()
    {
        if (_cachedVersionInfo != null)
        {
            return _cachedVersionInfo;
        }

        try
        {
            if (!File.Exists(_versionFilePath))
            {
                _cachedVersionInfo = new VersionInfo();
                return _cachedVersionInfo;
            }

            string json = File.ReadAllText(_versionFilePath, Encoding.UTF8);
            _cachedVersionInfo = JsonSerializer.Deserialize<VersionInfo>(json) ?? new VersionInfo();
            return _cachedVersionInfo;
        }
        catch
        {
            _cachedVersionInfo = new VersionInfo();
            return _cachedVersionInfo;
        }
    }

    private static void SaveVersionInfo(VersionInfo versionInfo)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(versionInfo, options);
            File.WriteAllText(_versionFilePath, json, Encoding.UTF8);
        }
        catch
        {
            // Silently fail if we can't save the settings
        }
    }
}

/// <summary>
/// Represents the version information from version.json.
/// </summary>
internal class VersionInfo
{
    [JsonPropertyName("version")]
    public string Version { get; set; } = "1.0.0";

    [JsonPropertyName("startWhenReady")]
    public bool StartWhenReady { get; set; } = false;
}
