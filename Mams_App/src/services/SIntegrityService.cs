using Mams_App.src.configurations;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace Mams_App.src.services;

/// <summary>
/// Service for verifying and repairing application resources.
/// Provides both quick startup checks (existence only) and full verification (with checksums).
/// </summary>
public static class SIntegrityService
{
    private const string LOCALIZATION_FOLDER = "resources/localization";
    private const string GITHUB_OWNER = "C0D3X-25";
    private const string GITHUB_REPO = "Mams";

    private static readonly HttpClient s_httpClient = new()
    {
        Timeout = TimeSpan.FromMinutes(2)
    };

    static SIntegrityService()
    {
        s_httpClient.DefaultRequestHeaders.Add("User-Agent", "Mams-App");
    }

    /// <summary>
    /// Result of an integrity check operation.
    /// </summary>
    public class IntegrityCheckResult
    {
        public bool IsValid { get; set; }
        public List<string> MissingFiles { get; set; } = [];
        public List<string> RepairedFiles { get; set; } = [];
        public List<string> FailedRepairs { get; set; } = [];
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Performs a quick startup check to verify essential resources exist.
    /// If resources are missing, attempts to download and repair them.
    /// </summary>
    /// <param name="progressCallback">Optional callback for progress updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result of the integrity check and repair operation.</returns>
    public static async Task<IntegrityCheckResult> verifyAndRepairAsync(
        Action<string, double?>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        var result = new IntegrityCheckResult { IsValid = true };

        try
        {
            progressCallback?.Invoke("Verifying resources...", null);
            Debug.WriteLine("[IntegrityService] Starting quick integrity check...");

            // Check localization folder
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var localizationPath = Path.Combine(baseDirectory, LOCALIZATION_FOLDER);

            if (!Directory.Exists(localizationPath))
            {
                Debug.WriteLine($"[IntegrityService] Localization folder missing: {localizationPath}");
                result.MissingFiles.Add(LOCALIZATION_FOLDER);
                result.IsValid = false;
            }
            else
            {
                // Check if folder has at least one .json file
                var jsonFiles = Directory.GetFiles(localizationPath, "*.json");
                if (jsonFiles.Length == 0)
                {
                    Debug.WriteLine("[IntegrityService] Localization folder is empty");
                    result.MissingFiles.Add($"{LOCALIZATION_FOLDER}/*.json");
                    result.IsValid = false;
                }
                else
                {
                    Debug.WriteLine($"[IntegrityService] Found {jsonFiles.Length} localization file(s)");
                }
            }

            // If resources are missing, attempt to repair
            if (!result.IsValid)
            {
                Debug.WriteLine("[IntegrityService] Missing resources detected, attempting repair...");
                progressCallback?.Invoke("Downloading missing resources...", null);

                var repairSuccess = await downloadLocalizationFilesAsync(progressCallback, cancellationToken);

                if (repairSuccess)
                {
                    result.RepairedFiles.Add(LOCALIZATION_FOLDER);
                    result.IsValid = true;
                    Debug.WriteLine("[IntegrityService] Repair completed successfully");
                }
                else
                {
                    result.FailedRepairs.Add(LOCALIZATION_FOLDER);
                    result.ErrorMessage = "Failed to download localization files";
                    Debug.WriteLine("[IntegrityService] Repair failed");
                }
            }
            else
            {
                Debug.WriteLine("[IntegrityService] All resources verified successfully");
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
    /// Downloads the localization files from the GitHub repository.
    /// </summary>
    private static async Task<bool> downloadLocalizationFilesAsync(
        Action<string, double?>? progressCallback = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var localizationPath = Path.Combine(baseDirectory, LOCALIZATION_FOLDER);

            // Create the directory if it doesn't exist
            Directory.CreateDirectory(localizationPath);

            // Download fr.json from GitHub (raw content)
            var currentVersion = SVersionService.GetVersion();
            var downloadUrl = $"https://raw.githubusercontent.com/{GITHUB_OWNER}/{GITHUB_REPO}/v{currentVersion}/Mams_App/resources/localization/fr.json";

            Debug.WriteLine($"[IntegrityService] Downloading from: {downloadUrl}");
            progressCallback?.Invoke("Downloading localization files...", 25);

            var response = await s_httpClient.GetAsync(downloadUrl, cancellationToken);

            // If version-specific URL fails, try main branch
            if (!response.IsSuccessStatusCode)
            {
                downloadUrl = $"https://raw.githubusercontent.com/{GITHUB_OWNER}/{GITHUB_REPO}/main/Mams_App/resources/localization/fr.json";
                Debug.WriteLine($"[IntegrityService] Trying main branch: {downloadUrl}");
                response = await s_httpClient.GetAsync(downloadUrl, cancellationToken);
            }

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine($"[IntegrityService] Download failed with status: {response.StatusCode}");
                return false;
            }

            progressCallback?.Invoke("Saving localization files...", 75);

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var filePath = Path.Combine(localizationPath, "fr.json");
            await File.WriteAllTextAsync(filePath, content, cancellationToken);

            Debug.WriteLine($"[IntegrityService] Saved localization file to: {filePath}");
            progressCallback?.Invoke("Resources verified", 100);

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[IntegrityService] Download failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Checks if the localization resources are valid (quick check - existence only).
    /// Does not attempt to repair.
    /// </summary>
    /// <returns>True if resources exist, false otherwise.</returns>
    public static bool areResourcesValid()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var localizationPath = Path.Combine(baseDirectory, LOCALIZATION_FOLDER);

        if (!Directory.Exists(localizationPath))
            return false;

        var jsonFiles = Directory.GetFiles(localizationPath, "*.json");
        return jsonFiles.Length > 0;
    }
}
