using System.IO;
using System.Text;
using System.Text.Json;

namespace Mams_App.src.configurations;

/// <summary>
/// Static service class for managing application configuration.
/// </summary>
public static class SAppConfigService
{
    private static readonly string _configFolderPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "ressources");

    private static readonly string _configFilePath = Path.Combine(
        _configFolderPath, "app_config.json");

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Loads the application configuration from the config file.
    /// Returns default values if the file doesn't exist or is invalid.
    /// </summary>
    /// <returns>The loaded <see cref="AppConfigItem"/> or default values.</returns>
    public static AppConfigItem loadConfig()
    {
        try
        {
            if (!File.Exists(_configFilePath))
            {
                return new AppConfigItem();
            }

            string json = File.ReadAllText(_configFilePath, Encoding.UTF8);
            var config = JsonSerializer.Deserialize<AppConfigItem>(json);

            return config ?? new AppConfigItem();
        }
        catch
        {
            return new AppConfigItem();
        }
    }

    /// <summary>
    /// Saves the application configuration to the config file.
    /// Creates the resources folder if it doesn't exist.
    /// </summary>
    /// <param name="config">The configuration to save.</param>
    public static void saveConfig(AppConfigItem config)
    {
        try
        {
            if (!Directory.Exists(_configFolderPath))
            {
                Directory.CreateDirectory(_configFolderPath);
            }

            string json = JsonSerializer.Serialize(config, _jsonOptions);
            File.WriteAllText(_configFilePath, json, Encoding.UTF8);
        }
        catch
        {
            // Silently fail if unable to save config
        }
    }
}
