using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Windows;
using Mams.src.configurations;

namespace Mams.src.services;

/// <summary>
/// Service to check for application updates from GitHub Releases and apply them.
/// </summary>
public static class SUpdateCheckerService
{
    private const string GITHUB_OWNER = "C0D3X-25";
    private const string GITHUB_REPO = "Mams";
    private const string GITHUB_API_URL = $"https://api.github.com/repos/{GITHUB_OWNER}/{GITHUB_REPO}/releases/latest";
    private const string UPDATE_FOLDER_NAME = "Mams_Update";
    private const string UPDATER_SCRIPT_NAME = "update.ps1";

    private static readonly HttpClient s_httpClient = new()
    {
        Timeout = TimeSpan.FromMinutes(5) // Longer timeout for downloads
    };

    static SUpdateCheckerService()
    {
        // GitHub API requires a User-Agent header
        s_httpClient.DefaultRequestHeaders.Add("User-Agent", "Mams-App");
    }

    /// <summary>
    /// Checks for updates asynchronously and offers to download and install if available.
    /// </summary>
    public static async Task checkForUpdatesAsync(bool showNoUpdateMessage = false)
    {
        try
        {
            Debug.WriteLine($"[UpdateChecker] Checking for updates at: {GITHUB_API_URL}");
            Debug.WriteLine($"[UpdateChecker] Current version: {getCurrentVersion()}");
            
            var latestRelease = await getLatestReleaseAsync();
            if (latestRelease == null)
            {
                Debug.WriteLine("[UpdateChecker] No release found or API request failed (private repo?)");
                if (showNoUpdateMessage)
                {
                    MessageBox.Show(
                        "Unable to check for updates.\n\n" +
                        "This may be because:\n" +
                        "• No internet connection\n" +
                        "• No releases published yet\n" +
                        "• Repository is private (requires authentication)",
                        "Update Check",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
                return;
            }

            Debug.WriteLine($"[UpdateChecker] Latest release found: {latestRelease.TagName}");
            
            var currentVersion = getCurrentVersion();
            var latestVersion = parseVersion(latestRelease.TagName);

            Debug.WriteLine($"[UpdateChecker] Comparing: current={currentVersion} vs latest={latestVersion}");

            if (latestVersion > currentVersion)
            {
                Debug.WriteLine("[UpdateChecker] Update available!");
                var result = MessageBox.Show(
                    $"A new version ({latestRelease.TagName}) is available!\n\n" +
                    $"Current version: v{currentVersion}\n\n" +
                    $"Would you like to download and install the update?\n\n" +
                    $"The application will restart after the update.",
                    "Update Available",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    await downloadAndInstallUpdateAsync(latestRelease);
                }
            }
            else
            {
                Debug.WriteLine("[UpdateChecker] Already up to date");
                if (showNoUpdateMessage)
                {
                    MessageBox.Show(
                        $"You are using the latest version (v{currentVersion}).",
                        "No Update Available",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateChecker] Update check failed: {ex.Message}");
            Debug.WriteLine($"[UpdateChecker] Stack trace: {ex.StackTrace}");
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
    /// Downloads and installs the update.
    /// </summary>
    private static async Task downloadAndInstallUpdateAsync(GitHubRelease release)
    {
        try
        {
            // Find the portable ZIP asset
            Debug.WriteLine($"[UpdateChecker] Looking for Portable ZIP in {release.Assets?.Count ?? 0} assets");
            
            var zipAsset = release.Assets?.FirstOrDefault(a => 
                a.Name?.Contains("Portable", StringComparison.OrdinalIgnoreCase) == true &&
                a.Name?.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == true);

            if (zipAsset?.DownloadUrl == null)
            {
                Debug.WriteLine("[UpdateChecker] No Portable ZIP found in release assets");
                MessageBox.Show(
                    "Could not find the update package. Please download manually from GitHub.",
                    "Update Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                openReleasePage();
                return;
            }

            Debug.WriteLine($"[UpdateChecker] Found asset: {zipAsset.Name} at {zipAsset.DownloadUrl}");

            // Create temp directory for update
            var tempDir = Path.Combine(Path.GetTempPath(), UPDATE_FOLDER_NAME);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);

            var zipPath = Path.Combine(tempDir, "update.zip");
            var extractPath = Path.Combine(tempDir, "extracted");

            // Show download progress
            var progressWindow = new Window
            {
                Title = "Downloading Update...",
                Width = 400,
                Height = 100,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow
            };

            var progressText = new System.Windows.Controls.TextBlock
            {
                Text = "Downloading update, please wait...",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 14
            };

            progressWindow.Content = progressText;
            progressWindow.Show();

            try
            {
                // Download the ZIP file
                Debug.WriteLine($"[UpdateChecker] Downloading from: {zipAsset.DownloadUrl}");
                using var response = await s_httpClient.GetAsync(zipAsset.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                await using var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await response.Content.CopyToAsync(fileStream);
                Debug.WriteLine($"[UpdateChecker] Download complete: {new FileInfo(zipPath).Length} bytes");
            }
            finally
            {
                progressWindow.Close();
            }

            // Extract the ZIP
            Debug.WriteLine($"[UpdateChecker] Extracting to: {extractPath}");
            ZipFile.ExtractToDirectory(zipPath, extractPath, true);

            // Get the new version from the release tag
            var newVersion = release.TagName?.TrimStart('v', 'V') ?? "1.0.0";

            // Create the updater script
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var scriptPath = Path.Combine(tempDir, UPDATER_SCRIPT_NAME);
            var scriptContent = generateUpdaterScript(extractPath, appDir, newVersion);
            await File.WriteAllTextAsync(scriptPath, scriptContent);

            Debug.WriteLine($"[UpdateChecker] Launching updater script: {scriptPath}");

            // Launch the updater script
            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-ExecutionPolicy Bypass -File \"{scriptPath}\"",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(startInfo);

            // Close the application to allow update
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateChecker] Update download failed: {ex.Message}");
            MessageBox.Show(
                $"Failed to download update: {ex.Message}\n\nPlease download manually from GitHub.",
                "Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            openReleasePage();
        }
    }

    /// <summary>
    /// Generates the PowerShell updater script content.
    /// </summary>
    private static string generateUpdaterScript(string sourcePath, string targetPath, string newVersion)
    {
        return $@"
# Mams Auto-Updater Script
$ErrorActionPreference = 'Stop'

$sourcePath = '{sourcePath.Replace("'", "''")}'

$targetPath = '{targetPath.Replace("'", "''")}'

$newVersion = '{newVersion}'
$appExe = Join-Path $targetPath 'Mams_App.exe'
$configPath = Join-Path $targetPath 'ressources\app_config.json'

# Wait for the application to close
Write-Host 'Waiting for application to close...'
Start-Sleep -Seconds 3

# Wait for the process to fully exit
$maxWait = 30
$waited = 0
while ($waited -lt $maxWait) {{
    $process = Get-Process -Name 'Mams_App' -ErrorAction SilentlyContinue
    if ($null -eq $process) {{
        break
    }}
    Start-Sleep -Seconds 1
    $waited++
}}

# Copy all files from source to target (overwrite)
Write-Host 'Copying update files...'
$files = Get-ChildItem -Path $sourcePath -Recurse -File
foreach ($file in $files) {{
    $relativePath = $file.FullName.Substring($sourcePath.Length + 1)
    $destPath = Join-Path $targetPath $relativePath
    $destDir = Split-Path $destPath -Parent
    
    if (-not (Test-Path $destDir)) {{
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }}
    
    # Skip the config file to preserve user settings
    if ($relativePath -ne 'ressources\app_config.json') {{
        Copy-Item -Path $file.FullName -Destination $destPath -Force
    }}
}}

# Update version in config file
Write-Host 'Updating version in config...'
if (Test-Path $configPath) {{
    $config = Get-Content $configPath -Raw | ConvertFrom-Json
    $config.version = $newVersion
    $config | ConvertTo-Json -Depth 10 | Set-Content $configPath -Encoding UTF8
}} else {{
    # Create config with just the version if it doesn't exist
    $config = @{{
        version = $newVersion
        window = @{{
            left = 100
            top = 100
            width = 1224
            height = 800
            is_maximized = $false
        }}
        localization = @{{
            language = 'en'
        }}
    }}
    
    $configDir = Split-Path $configPath -Parent
    if (-not (Test-Path $configDir)) {{
        New-Item -ItemType Directory -Path $configDir -Force | Out-Null
    }}
    
    $config | ConvertTo-Json -Depth 10 | Set-Content $configPath -Encoding UTF8
}}

# Start the updated application
Write-Host 'Starting updated application...'
Start-Process -FilePath $appExe

# Clean up temp files (with delay to ensure app has started)
Start-Sleep -Seconds 5
$tempDir = Split-Path $sourcePath -Parent
if (Test-Path $tempDir) {{
    Remove-Item -Path $tempDir -Recurse -Force -ErrorAction SilentlyContinue
}}

Write-Host 'Update complete!'
";
    }

    /// <summary>
    /// Gets the latest release information from GitHub.
    /// </summary>
    private static async Task<GitHubRelease?> getLatestReleaseAsync()
    {
        try
        {
            var response = await s_httpClient.GetAsync(GITHUB_API_URL);
            
            Debug.WriteLine($"[UpdateChecker] GitHub API response: {response.StatusCode}");
            
            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine($"[UpdateChecker] GitHub API error: {content}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<GitHubRelease>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateChecker] API request failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets the current application version from the config file.
    /// </summary>
    private static Version getCurrentVersion()
    {
        var config = SAppConfigService.loadConfig();
        return parseVersion(config.m_version);
    }

    /// <summary>
    /// Parses a version string (e.g., "v1.2.3" or "1.2.3") to a Version object.
    /// </summary>
    private static Version parseVersion(string? versionString)
    {
        if (string.IsNullOrEmpty(versionString))
        {
            return new Version(1, 0, 0);
        }

        // Remove 'v' prefix if present
        var cleanVersion = versionString.TrimStart('v', 'V');

        if (Version.TryParse(cleanVersion, out var version))
        {
            return version;
        }

        return new Version(1, 0, 0);
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
                FileName = $"https://github.com/{GITHUB_OWNER}/{GITHUB_REPO}/releases/latest",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateChecker] Failed to open release page: {ex.Message}");
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

        [JsonPropertyName("assets")]
        public List<GitHubAsset>? Assets { get; set; }
    }

    /// <summary>
    /// Represents a GitHub release asset.
    /// </summary>
    private class GitHubAsset
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string? DownloadUrl { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }
    }
}
