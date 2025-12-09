using MySqlConnector;
using System.IO;

namespace Mams.src.databaseOperations;

public static class SDatabaseBackup 
{

    private static string destination_path = "C:\\MySqlBackup\\";

    /// <summary>
    /// Create a backup (a dump) of the current database.
    /// </summary>
    /// <remarks>Create a backup only if the same file name is not present in the folder.
    /// The file name is the backup_dd_MM_yyyy.sql</remarks>
    /// <param name="connection">An open connection of type <see cref="MySqlConnection"/></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static void createBackup(MySqlConnection connection)
    {
        if (connection == null || connection.State != System.Data.ConnectionState.Open)
        {
            throw new InvalidOperationException("Database connection is not established or is closed.");
        }

        if (isBackupDoneToday())
        {
            return;
        }

        using (MySqlCommand cmd = new MySqlCommand())
        {
            using (MySqlBackup mb = new MySqlBackup(cmd))
            {
                cmd.Connection = connection;
                mb.ExportToFile(getTodayDateBackupPath());
            }
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
    /// Builds the path for the backup file based on today's date.
    /// </summary>
    /// <returns></returns>
    private static string getTodayDateBackupPath()
    {
        string today_year = DateTime.Now.Year.ToString();
        string today_month = DateTime.Now.Month.ToString();
        string today_day = DateTime.Now.Day.ToString();
        string destination_file = "backup_" + today_day + "_" + today_month + "_" + today_year + ".sql";

        return destination_path + destination_file;
    }

    /// <summary>
    /// Checks if a backup has already been done today.
    /// </summary>
    /// <returns></returns>
    private static bool isBackupDoneToday() 
    {
        return File.Exists(getTodayDateBackupPath());
    }
}
