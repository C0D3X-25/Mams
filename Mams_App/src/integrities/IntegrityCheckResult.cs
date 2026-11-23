namespace Mams_App.src.integrities;

/// <summary>
/// Result of an integrity check operation.
/// </summary>
public class IntegrityCheckResult
{
    /// <summary>
    /// Indicates if all required files are valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// List of missing required files.
    /// </summary>
    public List<string> MissingFiles { get; set; } = [];

    /// <summary>
    /// List of files with invalid checksums (corrupted).
    /// </summary>
    public List<string> CorruptedFiles { get; set; } = [];

    /// <summary>
    /// List of files that were successfully repaired.
    /// </summary>
    public List<string> RepairedFiles { get; set; } = [];

    /// <summary>
    /// List of files that failed to repair.
    /// </summary>
    public List<string> FailedRepairs { get; set; } = [];

    /// <summary>
    /// List of missing optional files.
    /// </summary>
    public List<string> OptionalMissingFiles { get; set; } = [];

    /// <summary>
    /// List of corrupted application files that require a full reinstall.
    /// </summary>
    public List<string> CorruptedAppFiles { get; set; } = [];

    /// <summary>
    /// Error message if the check failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Total number of files checked.
    /// </summary>
    public int TotalFilesChecked { get; set; }

    /// <summary>
    /// Indicates if a full application reinstall is required.
    /// </summary>
    public bool RequiresReinstall => CorruptedAppFiles.Count > 0;
}
