using MySqlConnector;
using System.Diagnostics;
using System.IO;

namespace Mams_App.src.databaseOperations;

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
    /// Restores the database from the specified backup file.
    /// </summary>
    /// <param name="connection">An open connection of type <see cref="MySqlConnection"/></param>
    /// <param name="backupFilePath">Full path to the backup file to restore</param>
    /// <exception cref="InvalidOperationException">Thrown when connection is not open</exception>
    /// <exception cref="FileNotFoundException">Thrown when backup file doesn't exist</exception>
    public static void restoreBackup(MySqlConnection connection, string backupFilePath)
    {
        if (connection == null || connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException("Database connection is not established or is closed.");
        }

        if (!File.Exists(backupFilePath))
        {
            throw new FileNotFoundException("Backup file not found.", backupFilePath);
        }

        using (MySqlCommand cmd = new MySqlCommand())
        {
            using (MySqlBackup mb = new MySqlBackup(cmd))
            {
                cmd.Connection = connection;
                mb.ImportFromFile(backupFilePath);
            }
        }

        Debug.WriteLine($"[SDatabaseBackup] Backup restored successfully from: {backupFilePath}");
    }

    /// <summary>
    /// Gets a list of all available backup files, ordered by date (newest first).
    /// </summary>
    /// <returns>Array of FileInfo objects representing available backups</returns>
    public static FileInfo[] getAvailableBackups()
    {
        ensureBackupDirectoryExists();

        var directory = new DirectoryInfo(destination_path);
        return directory.GetFiles("backup_*.sql")
                        .OrderByDescending(f => f.LastWriteTime)
                        .ToArray();
    }

    /// <summary>
    /// Gets the most recent backup file path, or null if no backups exist.
    /// </summary>
    /// <returns>Full path to the most recent backup, or null if none exists</returns>
    public static string? getLatestBackupPath()
    {
        var backups = getAvailableBackups();
        return backups.Length > 0 ? backups[0].FullName : null;
    }

    /// <summary>
    /// Not implemented yet.
    /// </summary>
    /// <param name="connection"></param>
    /// <exception cref="InvalidOperationException"></exception>
    [Obsolete("Use restoreBackup(MySqlConnection connection, string backupFilePath) instead")]
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
