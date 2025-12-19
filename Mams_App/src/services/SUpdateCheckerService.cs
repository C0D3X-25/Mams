using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Windows;

namespace Mams.src.services;

/// <summary>
/// Service to check for application updates from GitHub Releases.
/// </summary>
public static class SUpdateCheckerService
{
    private const string GITHUB_OWNER = "C0D3X-25";
    private const string GITHUB_REPO = "Mams";
    private const string GITHUB_API_URL = $"https://api.github.com/repos/{GITHUB_OWNER}/{GITHUB_REPO}/releases/latest";
    private const string GITHUB_RELEASES_URL = $"https://github.com/{GITHUB_OWNER}/{GITHUB_REPO}/releases/latest";

    private static readonly HttpClient s_httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    static SUpdateCheckerService()
    {
        // GitHub API requires a User-Agent header
        s_httpClient.DefaultRequestHeaders.Add("User-Agent", "Mams-App");
    }

    /// <summary>
    /// Checks for updates asynchronously and shows a dialog if a new version is available.
    /// </summary>
    public static async Task checkForUpdatesAsync(bool showNoUpdateMessage = false)
    {
        try
        {
            var latestRelease = await getLatestReleaseAsync();
            if (latestRelease == null)
            {
                if (showNoUpdateMessage)
                {
                    MessageBox.Show(
                        "Unable to check for updates. Please try again later.",
                        "Update Check",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                return;
            }

            var currentVersion = getCurrentVersion();
            var latestVersion = parseVersion(latestRelease.TagName);

            if (latestVersion > currentVersion)
            {
                var result = MessageBox.Show(
                    $"A new version ({latestRelease.TagName}) is available!\n\n" +
                    $"Current version: v{currentVersion}\n\n" +
                    $"Would you like to download the update?",
                    "Update Available",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    openReleasePage();
                }
            }
            else if (showNoUpdateMessage)
            {
                MessageBox.Show(
                    $"You are using the latest version (v{currentVersion}).",
                    "No Update Available",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Update check failed: {ex.Message}");
            if (showNoUpdateMessage)
            {
                MessageBox.Show(
                    $"Failed to check for updates: {ex.Message}",
                    "Update Check Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// Gets the latest release information from GitHub.
    /// </summary>
    private static async Task<GitHubRelease?> getLatestReleaseAsync()
    {
        try
        {
            var response = await s_httpClient.GetAsync(GITHUB_API_URL);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<GitHubRelease>();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the current application version.
    /// </summary>
    private static Version getCurrentVersion()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var version = assembly.GetName().Version;
        return version ?? new Version(1, 0, 0);
    }

    /// <summary>
    /// Parses a version string (e.g., "v1.2.3") to a Version object.
    /// </summary>
    private static Version parseVersion(string? tagName)
    {
        if (string.IsNullOrEmpty(tagName))
        {
            return new Version(0, 0, 0);
        }

        // Remove 'v' prefix if present
        var versionString = tagName.TrimStart('v', 'V');

        if (Version.TryParse(versionString, out var version))
        {
            return version;
        }

        return new Version(0, 0, 0);
    }

    /// <summary>
    /// Opens the GitHub releases page in the default browser.
    /// </summary>
    private static void openReleasePage()
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = GITHUB_RELEASES_URL,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to open release page: {ex.Message}");
        }
    }

    /// <summary>
    /// Represents a GitHub release response.
    /// </summary>
    private class GitHubRelease
    {
        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime? PublishedAt { get; set; }
    }
}
