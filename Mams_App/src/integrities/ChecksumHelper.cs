using System.IO;
using System.Security.Cryptography;

namespace Mams_App.src.integrities;

/// <summary>
/// Utility class for computing file checksums.
/// </summary>
public static class ChecksumHelper
{
    /// <summary>
    /// Computes the SHA256 checksum of a file.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <returns>The SHA256 checksum as a lowercase hexadecimal string.</returns>
    public static string ComputeChecksum(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Computes the SHA256 checksum of a byte array.
    /// </summary>
    /// <param name="data">The byte array.</param>
    /// <returns>The SHA256 checksum as a lowercase hexadecimal string.</returns>
    public static string ComputeChecksum(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Verifies if a file matches the expected checksum.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="expectedChecksum">The expected checksum.</param>
    /// <returns>True if the checksums match, false otherwise.</returns>
    public static bool VerifyChecksum(string filePath, string expectedChecksum)
    {
        if (string.IsNullOrEmpty(expectedChecksum))
            return true; // No checksum to verify

        var actualChecksum = ComputeChecksum(filePath);
        return string.Equals(actualChecksum, expectedChecksum, StringComparison.OrdinalIgnoreCase);
    }
}
