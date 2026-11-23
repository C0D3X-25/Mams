using Mams_App.src.configurations;
using Mams_App.src.localizations;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace Mams_App.src.services;

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
    /// Contains information about an available update.
    /// </summary>
    public class UpdateInfo
    {
        public bool IsUpdateAvailable { get; set; }
        public string? LatestVersion { get; set; }
        public string? DownloadUrl { get; set; }
        public string? AssetName { get; set; }
        internal GitHubRelease? Release { get; set; }
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
                        Loc.Get("Update.CheckFailed.Message") ??
                        "Unable to check for updates.\n\n" +
                        "This may be because:\n" +
                        "• No internet connection\n" +
                        "• No releases published yet\n" +
                        "• Repository is private (requires authentication)",
                        Loc.Get("Update.CheckFailed.Title") ?? "Update Check",
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
                    Loc.Get("Update.Available.Message", latestRelease.TagName ?? "", currentVersion) ??
                    $"A new version ({latestRelease.TagName}) is available!\n\n" +
                    $"Current version: v{currentVersion}\n\n" +
                    $"Would you like to download and install the update?\n\n" +
                    $"The application will restart after the update.",
                    Loc.Get("Update.Available.Title") ?? "Update Available",
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
                        Loc.Get("Update.NoUpdate.Message", currentVersion) ??
                        $"You are using the latest version (v{currentVersion}).",
                        Loc.Get("Update.NoUpdate.Title") ?? "No Update Available",
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
                    Loc.Get("Update.CheckError.Message", ex.Message) ??
                    $"Failed to check for updates: {ex.Message}",
                    Loc.Get("Update.Error.Title") ?? "Update Check Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }

    private static CancellationTokenSource? s_downloadCancellationTokenSource;

    /// <summary>
    /// Cancels any ongoing download operation.
    /// </summary>
    public static void cancelDownload()
    {
        s_downloadCancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Checks for updates and returns information about any available update.
    /// Does not show any UI - caller is responsible for prompting user.
    /// </summary>
    /// <returns>UpdateInfo with IsUpdateAvailable=true if an update exists, null if check failed.</returns>
    public static async Task<UpdateInfo?> checkForUpdateInfoAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            Debug.WriteLine($"[UpdateChecker] Checking for updates at: {GITHUB_API_URL}");
            Debug.WriteLine($"[UpdateChecker] Current version: {getCurrentVersion()}");

            var latestRelease = await getLatestReleaseAsync(cancellationToken);
            if (latestRelease == null)
            {
                Debug.WriteLine("[UpdateChecker] No release found or API request failed");
                return null;
            }

            Debug.WriteLine($"[UpdateChecker] Latest release found: {latestRelease.TagName}");

            var currentVersion = getCurrentVersion();
            var latestVersion = parseVersion(latestRelease.TagName);

            Debug.WriteLine($"[UpdateChecker] Comparing: current={currentVersion} vs latest={latestVersion}");

            var zipAsset = latestRelease.Assets?.FirstOrDefault(a =>
                a.Name?.Contains("Portable", StringComparison.OrdinalIgnoreCase) == true &&
                a.Name?.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) == true);

            return new UpdateInfo
            {
                IsUpdateAvailable = latestVersion > currentVersion,
                LatestVersion = latestRelease.TagName?.TrimStart('v', 'V'),
                DownloadUrl = zipAsset?.DownloadUrl,
                AssetName = zipAsset?.Name,
                Release = latestRelease
            };
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[UpdateChecker] Update check cancelled");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateChecker] Update check failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Downloads and installs the update with progress reporting.
    /// </summary>
    /// <param name="updateInfo">Update information from checkForUpdateInfoAsync</param>
    /// <param name="progressCallback">Callback for progress updates (status message, percentage 0-100 or null for indeterminate)</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
    public static async Task downloadAndInstallUpdateAsync(UpdateInfo updateInfo, Action<string, double?>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        if (updateInfo.Release == null || string.IsNullOrEmpty(updateInfo.DownloadUrl))
        {
            Debug.WriteLine("[UpdateChecker] No valid update info provided");
            return;
        }

        s_downloadCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var linkedToken = s_downloadCancellationTokenSource.Token;

        try
        {
            var release = updateInfo.Release;

            Debug.WriteLine($"[UpdateChecker] Found asset: {updateInfo.AssetName} at {updateInfo.DownloadUrl}");

            // Create temp directory for update
            var tempDir = Path.Combine(Path.GetTempPath(), UPDATE_FOLDER_NAME);
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }
            Directory.CreateDirectory(tempDir);

            var zipPath = Path.Combine(tempDir, "update.zip");
            var extractPath = Path.Combine(tempDir, "extracted");

            // Download the ZIP file with progress
            Debug.WriteLine($"[UpdateChecker] Downloading from: {updateInfo.DownloadUrl}");
            progressCallback?.Invoke(Loc.Get("Launcher.DownloadingUpdate") ?? "Downloading update...", null);

            using var response = await s_httpClient.GetAsync(updateInfo.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, linkedToken);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            var canReportProgress = totalBytes > 0;

            await using var contentStream = await response.Content.ReadAsStreamAsync(linkedToken);
            await using var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true);

            var buffer = new byte[65536];
            long downloadedBytes = 0;
            long lastReportedBytes = 0;
            const long progressReportInterval = 102400;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, linkedToken)) > 0)
            {
                linkedToken.ThrowIfCancellationRequested();
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), linkedToken);
                downloadedBytes += bytesRead;

                if (downloadedBytes - lastReportedBytes >= progressReportInterval)
                {
                    lastReportedBytes = downloadedBytes;

                    if (canReportProgress)
                    {
                        var percentage = (double)downloadedBytes / totalBytes * 100;
                        var statusText = $"{Loc.Get("Launcher.DownloadingUpdate") ?? "Downloading update..."}\n{percentage:F1}% ({formatBytes(downloadedBytes)} / {formatBytes(totalBytes)})";
                        progressCallback?.Invoke(statusText, percentage);
                    }
                    else
                    {
                        progressCallback?.Invoke($"{Loc.Get("Launcher.DownloadingUpdate") ?? "Downloading update..."}\n{formatBytes(downloadedBytes)}", null);
                    }

                    await Task.Yield();
                }
            }

            Debug.WriteLine($"[UpdateChecker] Download complete: {downloadedBytes} bytes");

            linkedToken.ThrowIfCancellationRequested();

            // Extract phase
            progressCallback?.Invoke(Loc.Get("Launcher.ExtractingUpdate") ?? "Extracting update...", null);
            await Task.Delay(100, linkedToken);

            // Close file stream before extracting
            await fileStream.DisposeAsync();

            // Extract the ZIP - run on background thread to keep UI responsive
            Debug.WriteLine($"[UpdateChecker] Extracting to: {extractPath}");
            await Task.Run(() => ZipFile.ExtractToDirectory(zipPath, extractPath, true), linkedToken);

            linkedToken.ThrowIfCancellationRequested();

            // Get the new version from the release tag
            var newVersion = release.TagName?.TrimStart('v', 'V') ?? "1.0.0";

            // Create the updater script
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var scriptPath = Path.Combine(tempDir, UPDATER_SCRIPT_NAME);
            var scriptContent = generateUpdaterScript(extractPath, appDir, newVersion);
            await File.WriteAllTextAsync(scriptPath, scriptContent, linkedToken);

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

            // Close the application to allow update - the Launcher will be shown first after restart
            Application.Current.Shutdown();
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[UpdateChecker] Update download cancelled");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateChecker] Update download failed: {ex.Message}");
            throw;
        }
        finally
        {
            s_downloadCancellationTokenSource?.Dispose();
            s_downloadCancellationTokenSource = null;
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
                    Loc.Get("Update.PackageNotFound.Message") ??
                    "Could not find the update package. Please download manually from GitHub.",
                    Loc.Get("Update.Error.Title") ?? "Update Error",
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

            // Create progress window with progress bar
            var progressWindow = new Window
            {
                Title = Loc.Get("Update.Downloading.Title") ?? "Downloading Update...",
                Width = 450,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow
            };

            var stackPanel = new StackPanel
            {
                Margin = new Thickness(20),
                VerticalAlignment = VerticalAlignment.Center
            };

            var statusText = new TextBlock
            {
                Text = Loc.Get("Update.Connecting") ?? "Connecting to server...",
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var progressBar = new ProgressBar
            {
                Height = 25,
                Minimum = 0,
                Maximum = 100,
                Value = 0
            };

            var progressText = new TextBlock
            {
                Text = "0%",
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 5, 0, 0),
                Foreground = System.Windows.Media.Brushes.Gray
            };

            stackPanel.Children.Add(statusText);
            stackPanel.Children.Add(progressBar);
            stackPanel.Children.Add(progressText);
            progressWindow.Content = stackPanel;
            progressWindow.Show();

            try
            {
                // Download the ZIP file with progress
                Debug.WriteLine($"[UpdateChecker] Downloading from: {zipAsset.DownloadUrl}");
                statusText.Text = Loc.Get("Update.Downloading", zipAsset.Name ?? "update") ?? $"Downloading {zipAsset.Name}...";

                using var response = await s_httpClient.GetAsync(zipAsset.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                var canReportProgress = totalBytes > 0;

                await using var contentStream = await response.Content.ReadAsStreamAsync();
                await using var fileStream = new FileStream(zipPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true);

                // Use larger buffer for better download performance
                var buffer = new byte[65536]; // 64KB buffer instead of 8KB
                long downloadedBytes = 0;
                long lastReportedBytes = 0;
                const long progressReportInterval = 102400; // Update UI every 100KB
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                    downloadedBytes += bytesRead;

                    // Only update UI periodically to avoid slowing down the download
                    if (downloadedBytes - lastReportedBytes >= progressReportInterval)
                    {
                        lastReportedBytes = downloadedBytes;

                        if (canReportProgress)
                        {
                            var percentage = (double)downloadedBytes / totalBytes * 100;
                            progressBar.Value = percentage;
                            progressText.Text = $"{percentage:F1}% ({formatBytes(downloadedBytes)} / {formatBytes(totalBytes)})";
                        }
                        else
                        {
                            progressText.Text = $"Downloaded: {formatBytes(downloadedBytes)}";
                            progressBar.IsIndeterminate = true;
                        }

                        // Yield to allow UI to update, but don't add artificial delay
                        await Task.Yield();
                    }
                }

                // Final progress update
                if (canReportProgress)
                {
                    progressBar.Value = 100;
                    progressText.Text = $"100% ({formatBytes(downloadedBytes)} / {formatBytes(totalBytes)})";
                }

                Debug.WriteLine($"[UpdateChecker] Download complete: {downloadedBytes} bytes");

                // Extract phase
                statusText.Text = Loc.Get("Update.Extracting") ?? "Extracting update...";
                progressBar.IsIndeterminate = true;
                progressText.Text = Loc.Get("Update.PleaseWait") ?? "Please wait...";
                await Task.Delay(100); // Allow UI to update
            }
            finally
            {
                progressWindow.Close();
            }

            // Extract the ZIP - run on background thread to keep UI responsive
            Debug.WriteLine($"[UpdateChecker] Extracting to: {extractPath}");
            await Task.Run(() => ZipFile.ExtractToDirectory(zipPath, extractPath, true));

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

            // Close the application to allow update - the Launcher will be shown first after restart
            Application.Current.Shutdown();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateChecker] Update download failed: {ex.Message}");
            MessageBox.Show(
                Loc.Get("Update.DownloadFailed.Message", ex.Message) ??
                $"Failed to download update: {ex.Message}\n\nPlease download manually from GitHub.",
                Loc.Get("Update.Error.Title") ?? "Update Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            openReleasePage();
        }
    }

    /// <summary>
    /// Formats bytes into a human-readable string.
    /// </summary>
    private static string formatBytes(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:F2} {sizes[order]}";
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

$appExe = Join-Path $targetPath 'Mams_App.exe'

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

# Also wait for any mysqld process from our installation to stop
Write-Host 'Waiting for MariaDB to stop...'
$mariaDbPath = Join-Path $targetPath 'mariadb\bin\mysqld.exe'
$maxWait = 15
$waited = 0
while ($waited -lt $maxWait) {{
    $mysqldProcesses = Get-Process -Name 'mysqld' -ErrorAction SilentlyContinue
    $ourProcess = $false
    foreach ($proc in $mysqldProcesses) {{
        try {{
            if ($proc.Path -eq $mariaDbPath) {{
                $ourProcess = $true
                break
            }}
        }} catch {{ }}
    }}
    if (-not $ourProcess) {{
        break
    }}
    Start-Sleep -Seconds 1
    $waited++
}}

# Folders to preserve during update (user data and runtime)
# Note: 'resources' is NOT preserved because localization files need to be updated
# version.json is explicitly updated by this script after copying
$preserveFolders = @(
    'mariadb',
    'logs',
    'backups',
    'runtimes'
)

# Clean up old application files before copying new ones
Write-Host 'Cleaning old application files...'
$existingItems = Get-ChildItem -Path $targetPath -ErrorAction SilentlyContinue
foreach ($item in $existingItems) {{
    $itemName = $item.Name
    
    # Check if this folder should be preserved
    $preserve = $false
    foreach ($folder in $preserveFolders) {{
        if ($itemName -eq $folder) {{
            $preserve = $true
            Write-Host ""Preserving: $itemName""
            break
        }}
    }}
    
    if (-not $preserve) {{
        Write-Host ""Removing: $itemName""
        Remove-Item -Path $item.FullName -Recurse -Force -ErrorAction SilentlyContinue
    }}
}}

# Copy all files from source to target
Write-Host 'Copying update files...'
$files = Get-ChildItem -Path $sourcePath -Recurse -File
foreach ($file in $files) {{
    $relativePath = $file.FullName.Substring($sourcePath.Length + 1)
    $destPath = Join-Path $targetPath $relativePath
    $destDir = Split-Path $destPath -Parent
    
    # Check if this file is in a preserved folder (skip copying to avoid overwriting user data)
    $skip = $false
    foreach ($folder in $preserveFolders) {{
        if ($relativePath -like ""$folder\*"") {{
            $skip = $true
            Write-Host ""Skipping: $relativePath""
            break
        }}
    }}
    
    if ($skip) {{
        continue
    }}
    
    if (-not (Test-Path $destDir)) {{
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }}
    
    
    Copy-Item -Path $file.FullName -Destination $destPath -Force
}}

# Start the updated application (Launcher will be shown first)
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
    private static async Task<GitHubRelease?> getLatestReleaseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await s_httpClient.GetAsync(GITHUB_API_URL, cancellationToken);

            Debug.WriteLine($"[UpdateChecker] GitHub API response: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                Debug.WriteLine($"[UpdateChecker] GitHub API error: {content}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<GitHubRelease>(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[UpdateChecker] API request cancelled");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[UpdateChecker] API request failed: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Gets the current application version from the version.json file.
    /// </summary>
    private static Version getCurrentVersion()
    {
        return parseVersion(SVersionService.GetVersion());
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
    internal class GitHubRelease
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
    internal class GitHubAsset
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("browser_download_url")]
        public string? DownloadUrl { get; set; }

        [JsonPropertyName("size")]
        public long Size { get; set; }
    }
}
