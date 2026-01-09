using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mams_App.src.configurations;

/// <summary>
/// Static service class for reading the application version.
/// Version is stored in the resources folder and is read-only.
/// </summary>
public static class SVersionService
{
    private static readonly string _versionFilePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "ressources", "version.json");

    private static string? _cachedVersion;

    /// <summary>
    /// Gets the application version from the version.json file.
    /// </summary>
    /// <returns>The application version string, or "1.0.0" if not found.</returns>
    public static string GetVersion()
    {
        if (_cachedVersion != null)
        {
            return _cachedVersion;
        }

        try
        {
            if (!File.Exists(_versionFilePath))
            {
                _cachedVersion = "1.0.0";
                return _cachedVersion;
            }

            string json = File.ReadAllText(_versionFilePath, Encoding.UTF8);
            var versionInfo = JsonSerializer.Deserialize<VersionInfo>(json);

            _cachedVersion = versionInfo?.Version ?? "1.0.0";
            return _cachedVersion;
        }
        catch
        {
            _cachedVersion = "1.0.0";
            return _cachedVersion;
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
}
