using Mams_App.src.configurations;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Text.Json;

namespace Mams_App.src.integrities;

/// <summary>
/// Service for verifying and repairing application resources.
/// Provides both quick startup checks (existence only) and full verification (with checksums).
/// </summary>
public static class SIntegrityService
{
    private const string MANIFEST_FILE = "resources/manifest.json";
    private const string GITHUB_OWNER = "C0D3X-25";
    private const string GITHUB_REPO = "Mams";
    private const string GITHUB_PROJECT_PATH = "Mams_App";

    private static readonly HttpClient s_httpClient = new()
    {
        Timeout = TimeSpan.FromMinutes(5)
    };

    static SIntegrityService()
    {
        s_httpClient.DefaultRequestHeaders.Add("User-Agent", "Mams-App");
    }

    #region Public Methods

    /// <summary>
    /// Performs a quick startup check to verify essential resources exist.
    /// Only checks required files from the manifest (existence only, no checksums).
    /// If resources are missing, attempts to download and repair them.
    /// </summary>
    /// <param name="progressCallback">Optional callback for progress updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the integrity check and repair operation.</returns>
    public static async Task<IntegrityCheckResult> verifyAndRepairAsync(
        Action<string, double?>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        return await verifyAndRepairAsync(
            requiredOnly: true,
            verifyChecksums: false,
            progressCallback,
            cancellationToken);
    }

    /// <summary>
    /// Performs a full integrity check on all files in the manifest.
    /// Used by the Verify/Repair feature in settings.
    /// </summary>
    /// <param name="requiredOnly">If true, only checks required files. If false, checks all files.</param>
    /// <param name="progressCallback">Optional callback for progress updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the integrity check and repair operation.</returns>
    public static async Task<IntegrityCheckResult> verifyAndRepairAsync(
        bool requiredOnly,
        Action<string, double?>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        return await verifyAndRepairAsync(
            requiredOnly,
            verifyChecksums: !requiredOnly, // Full check includes checksums
            progressCallback,
            cancellationToken);
    }

    /// <summary>
    /// Performs an integrity check with full control over options.
    /// </summary>
    /// <param name="requiredOnly">If true, only checks required files.</param>
    /// <param name="verifyChecksums">If true, verifies file checksums (slower but detects corruption).</param>
    /// <param name="progressCallback">Optional callback for progress updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the integrity check and repair operation.</returns>
    public static async Task<IntegrityCheckResult> verifyAndRepairAsync(
        bool requiredOnly,
        bool verifyChecksums,
        Action<string, double?>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        var result = new IntegrityCheckResult { IsValid = true };

        try
        {
            progressCallback?.Invoke("Loading manifest...", null);
            Debug.WriteLine($"[IntegrityService] Starting integrity check (requiredOnly={requiredOnly}, verifyChecksums={verifyChecksums})...");

            var manifest = loadManifest();
            if (manifest == null || manifest.Files.Count == 0)
            {
                Debug.WriteLine("[IntegrityService] No manifest found or manifest is empty, skipping check");
                return result;
            }

            var filesToCheck = requiredOnly
                ? manifest.Files.Where(f => f.Value.Required).ToList()
                : manifest.Files.ToList();

            result.TotalFilesChecked = filesToCheck.Count;
            Debug.WriteLine($"[IntegrityService] Checking {filesToCheck.Count} files...");

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var filesToRepair = new List<KeyValuePair<string, ManifestFile>>();

            // Check each file
            for (int i = 0; i < filesToCheck.Count; i++)
            {
                var (fileKey, file) = filesToCheck[i];
                var filePath = Path.Combine(baseDirectory, file.Path.Replace('/', Path.DirectorySeparatorChar));
                var progress = (double)(i + 1) / filesToCheck.Count * 50;

                progressCallback?.Invoke($"Checking {Path.GetFileName(file.Path)}...", progress);
                cancellationToken.ThrowIfCancellationRequested();

                var fileStatus = checkFile(filePath, file, verifyChecksums);

                switch (fileStatus)
                {
                    case FileStatus.Missing:
                        Debug.WriteLine($"[IntegrityService] Missing file: {file.Path}");
                        filesToRepair.Add(new KeyValuePair<string, ManifestFile>(fileKey, file));
                        if (file.Required)
                        {
                            result.MissingFiles.Add(file.Path);
                            result.IsValid = false;
                        }
                        else
                        {
                            result.OptionalMissingFiles.Add(file.Path);
                        }
                        break;

                    case FileStatus.Corrupted:
                        Debug.WriteLine($"[IntegrityService] Corrupted file: {file.Path}");
                        if (file.IsAppFile)
                        {
                            result.CorruptedAppFiles.Add(file.Path);
                            result.IsValid = false;
                        }
                        else
                        {
                            filesToRepair.Add(new KeyValuePair<string, ManifestFile>(fileKey, file));
                            result.CorruptedFiles.Add(file.Path);
                            if (file.Required)
                                result.IsValid = false;
                        }
                        break;

                    case FileStatus.Valid:
                        Debug.WriteLine($"[IntegrityService] Valid: {file.Path}");
                        break;
                }
            }

            // If app files are corrupted, we can't repair them while running
            if (result.CorruptedAppFiles.Count > 0)
            {
                result.ErrorMessage = "Application files are corrupted. A reinstall is required.";
                progressCallback?.Invoke(result.ErrorMessage, 100);
                Debug.WriteLine("[IntegrityService] Corrupted app files detected - reinstall required");
                return result;
            }

            // Repair resource files
            if (filesToRepair.Count > 0)
            {
                Debug.WriteLine($"[IntegrityService] Attempting to repair {filesToRepair.Count} file(s)...");
                progressCallback?.Invoke("Downloading missing/corrupted files...", 50);

                var repairableFiles = requiredOnly
                    ? filesToRepair.Where(f => f.Value.Required).ToList()
                    : filesToRepair;

                for (int i = 0; i < repairableFiles.Count; i++)
                {
                    var (fileKey, file) = repairableFiles[i];
                    var progress = 50 + (double)(i + 1) / repairableFiles.Count * 50;

                    progressCallback?.Invoke($"Downloading {Path.GetFileName(file.Path)}...", progress);
                    cancellationToken.ThrowIfCancellationRequested();

                    var success = await downloadFileAsync(file.Path, file.Checksum, cancellationToken);

                    if (success)
                    {
                        result.RepairedFiles.Add(file.Path);
                        result.MissingFiles.Remove(file.Path);
                        result.CorruptedFiles.Remove(file.Path);
                        Debug.WriteLine($"[IntegrityService] Repaired: {file.Path}");
                    }
                    else
                    {
                        result.FailedRepairs.Add(file.Path);
                        Debug.WriteLine($"[IntegrityService] Failed to repair: {file.Path}");
                    }
                }

                // Update validity
                result.IsValid = !result.MissingFiles.Any() && !result.CorruptedFiles.Any();
            }

            if (result.IsValid)
            {
                progressCallback?.Invoke("All resources verified", 100);
                Debug.WriteLine("[IntegrityService] Integrity check completed successfully");
            }
            else
            {
                result.ErrorMessage = $"Failed to repair {result.FailedRepairs.Count} file(s)";
                progressCallback?.Invoke(result.ErrorMessage, 100);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[IntegrityService] Integrity check cancelled");
            throw;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[IntegrityService] Integrity check failed: {ex.Message}");
            result.IsValid = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    /// <summary>
    /// Checks if all required resources are valid (quick check - existence only).
    /// Does not attempt to repair.
    /// </summary>
    /// <returns>True if all required resources exist, false otherwise.</returns>
    public static bool areResourcesValid()
    {
        try
        {
            var manifest = loadManifest();
            if (manifest == null || manifest.Files.Count == 0)
                return true;

            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            foreach (var (_, file) in manifest.Files.Where(f => f.Value.Required))
            {
                var filePath = Path.Combine(baseDirectory, file.Path.Replace('/', Path.DirectorySeparatorChar));
                if (!File.Exists(filePath))
                    return false;
            }

            return true;
        }
        catch
        {
            return true; // Don't block app on check failure
        }
    }

    /// <summary>
    /// Gets the current manifest.
    /// </summary>
    /// <returns>The manifest or null if not found.</returns>
    public static ResourceManifest? getManifest()
    {
        return loadManifest();
    }

    /// <summary>
    /// Generates checksums for all files in the manifest.
    /// Useful for creating/updating the manifest during build.
    /// </summary>
    /// <returns>Dictionary of file paths to their checksums.</returns>
    public static Dictionary<string, string> generateChecksums()
    {
        var checksums = new Dictionary<string, string>();
        var manifest = loadManifest();

        if (manifest == null)
            return checksums;

        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        foreach (var (fileKey, file) in manifest.Files)
        {
            var filePath = Path.Combine(baseDirectory, file.Path.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(filePath))
            {
                checksums[file.Path] = ChecksumHelper.ComputeChecksum(filePath);
            }
        }

        return checksums;
    }

    #endregion

    #region Private Methods

    private enum FileStatus
    {
        Valid,
        Missing,
        Corrupted
    }

    private static FileStatus checkFile(string filePath, ManifestFile file, bool verifyChecksum)
    {
        if (!File.Exists(filePath))
            return FileStatus.Missing;

        if (verifyChecksum && !string.IsNullOrEmpty(file.Checksum))
        {
            if (!ChecksumHelper.VerifyChecksum(filePath, file.Checksum))
                return FileStatus.Corrupted;
        }

        return FileStatus.Valid;
    }

    /// <summary>
    /// Loads the manifest from the resources folder.
    /// </summary>
    private static ResourceManifest? loadManifest()
    {
        try
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var manifestPath = Path.Combine(baseDirectory, MANIFEST_FILE.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(manifestPath))
            {
                Debug.WriteLine($"[IntegrityService] Manifest not found at: {manifestPath}");
                return null;
            }

            var json = File.ReadAllText(manifestPath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            return JsonSerializer.Deserialize<ResourceManifest>(json, options);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[IntegrityService] Failed to load manifest: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Downloads a file from the GitHub repository.
    /// </summary>
    private static async Task<bool> downloadFileAsync(
        string relativePath,
        string? expectedChecksum,
        CancellationToken cancellationToken)
    {
        try
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var localPath = Path.Combine(baseDirectory, relativePath.Replace('/', Path.DirectorySeparatorChar));

            // Create directory if it doesn't exist
            var directory = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Build GitHub raw URL - try version tag first, then main branch
            var currentVersion = SVersionService.GetVersion();
            var githubPath = $"{GITHUB_PROJECT_PATH}/{relativePath}";
            var downloadUrl = $"https://raw.githubusercontent.com/{GITHUB_OWNER}/{GITHUB_REPO}/v{currentVersion}/{githubPath}";

            Debug.WriteLine($"[IntegrityService] Downloading from: {downloadUrl}");

            var response = await s_httpClient.GetAsync(downloadUrl, cancellationToken);

            // If version-specific URL fails, try main branch
            if (!response.IsSuccessStatusCode)
            {
                downloadUrl = $"https://raw.githubusercontent.com/{GITHUB_OWNER}/{GITHUB_REPO}/main/{githubPath}";
                Debug.WriteLine($"[IntegrityService] Trying main branch: {downloadUrl}");
                response = await s_httpClient.GetAsync(downloadUrl, cancellationToken);
            }

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"[IntegrityService] Download failed with status: {response.StatusCode}");
                return false;
            }

            // Download content
            var content = await response.Content.ReadAsByteArrayAsync(cancellationToken);

            // Verify checksum before saving
            if (!string.IsNullOrEmpty(expectedChecksum))
            {
                var downloadedChecksum = ChecksumHelper.ComputeChecksum(content);
                if (!string.Equals(downloadedChecksum, expectedChecksum, StringComparison.OrdinalIgnoreCase))
                {
                    Debug.WriteLine($"[IntegrityService] Checksum mismatch for {relativePath}. Expected: {expectedChecksum}, Got: {downloadedChecksum}");
                    return false;
                }
            }

            // Save file
            await File.WriteAllBytesAsync(localPath, content, cancellationToken);

            Debug.WriteLine($"[IntegrityService] Saved file to: {localPath}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[IntegrityService] Download failed: {ex.Message}");
            return false;
        }
    }

    #endregion
}
