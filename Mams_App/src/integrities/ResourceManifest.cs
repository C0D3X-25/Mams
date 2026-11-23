using System.Text.Json.Serialization;

namespace Mams_App.src.integrities;

/// <summary>
/// Represents the application resource manifest for integrity verification.
/// </summary>
public class ResourceManifest
{
    [JsonPropertyName("manifestVersion")]
    public string ManifestVersion { get; set; } = "1.0";

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("files")]
    public Dictionary<string, ManifestFile> Files { get; set; } = [];
}
