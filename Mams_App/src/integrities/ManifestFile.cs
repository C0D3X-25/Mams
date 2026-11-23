using System.Text.Json.Serialization;

namespace Mams_App.src.integrities;

/// <summary>
/// Represents a file entry in the manifest.
/// </summary>
public class ManifestFile
{
    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("required")]
    public bool Required { get; set; } = true;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("checksum")]
    public string? Checksum { get; set; }

    [JsonPropertyName("isAppFile")]
    public bool IsAppFile { get; set; } = false;
}
