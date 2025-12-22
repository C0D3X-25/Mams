using MySqlConnector;
using System.Diagnostics;
using System.IO;

namespace Mams.src.databaseOperations;

public static class SDatabaseBackup 
{
    private static readonly string destination_path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Mams", "Backups");

    /// <summary>
    /// Create a backup (a dump) of the current database.
    /// </summary>
    /// <remarks>Create a backup with timestamp including hours and minutes.
    /// The file name is backup_dd_MM_yyyy_HH_mm.sql</remarks>
    /// <param name="connection">An open connection of type <see cref="MySqlConnection"/></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void createBackup(MySqlConnection connection)
    {
        if (connection == null || connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException("Database connection is not established or is closed.");
        }

        try
        {
            // Ensure the backup directory exists
            ensureBackupDirectoryExists();

            var backupPath = getTimestampedBackupPath();

            using (MySqlCommand cmd = new MySqlCommand())
            {
                using (MySqlBackup mb = new MySqlBackup(cmd))
                {
                    cmd.Connection = connection;
                    mb.ExportToFile(backupPath);
                }
            }

            Debug.WriteLine($"[SDatabaseBackup] Backup created successfully: {backupPath}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SDatabaseBackup] Failed to create backup: {ex.Message}");
            // Don't throw - backup failure shouldn't prevent app from starting/closing
        }
    }

    /// <summary>
    /// Not implemented yet.
    /// </summary>
    /// <param name="connection"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void restoreBackup(MySqlConnection connection)
    {
        if (connection == null || connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException("Database connection is not established or is closed.");
        }

        using (MySqlCommand cmd = new MySqlCommand()) 
        {
            using (MySqlBackup mb = new MySqlBackup(cmd))
            {
                cmd.Connection = connection;
                // TODO: get the correct file
                //mb.ImportFromFile(getTodayDateBackupString()); 
            }
        }
    }

    /// <summary>
    /// Ensures the backup directory exists, creating it if necessary.
    /// </summary>
    private static void ensureBackupDirectoryExists()
    {
        if (!Directory.Exists(destination_path))
        {
            Directory.CreateDirectory(destination_path);
            Debug.WriteLine($"[SDatabaseBackup] Created backup directory: {destination_path}");
        }
    }

    /// <summary>
    /// Builds the path for the backup file with full timestamp (date + time).
    /// </summary>
    /// <returns>Full path to the backup file with format backup_dd_MM_yyyy_HH_mm.sql</returns>
    private static string getTimestampedBackupPath()
    {
        string timestamp = DateTime.Now.ToString("dd_MM_yyyy_HH_mm");
        string destination_file = $"backup_{timestamp}.sql";

        return Path.Combine(destination_path, destination_file);
    }

    /// <summary>
    /// Gets the backup directory path.
    /// </summary>
    public static string BackupDirectoryPath => destination_path;
}
