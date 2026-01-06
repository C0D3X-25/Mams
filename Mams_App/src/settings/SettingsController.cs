using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Mams_App.src.commands;
using Mams_App.src.databaseOperations;
using Mams_App.src.services;
using MySqlConnector;

namespace Mams_App.src.settings;

/// <summary>
/// Controller for the Settings window. Handles backup listing and restoration.
/// </summary>
public class SettingsController : INotifyPropertyChanged
{
    private readonly Window m_window;
    private FileInfo? m_selectedBackup;
    private ObservableCollection<FileInfo> m_backups;

    public event PropertyChangedEventHandler? PropertyChanged;

    #region Properties

    /// <summary>
    /// Gets the backup directory path.
    /// </summary>
    public static string BackupDirectory => SDatabaseBackup.BackupDirectoryPath;

    /// <summary>
    /// Gets the list of available backups.
    /// </summary>
    public ObservableCollection<FileInfo> Backups
    {
        get => m_backups;
        private set
        {
            m_backups = value;
            onPropertyChanged();
            onPropertyChanged(nameof(HasNoBackups));
        }
    }

    /// <summary>
    /// Gets or sets the currently selected backup.
    /// </summary>
    public FileInfo? SelectedBackup
    {
        get => m_selectedBackup;
        set
        {
            m_selectedBackup = value;
            onPropertyChanged();
            onPropertyChanged(nameof(CanRestoreBackup));
        }
    }

    /// <summary>
    /// Gets whether there are no backups available.
    /// </summary>
    public bool HasNoBackups => Backups.Count == 0;

    /// <summary>
    /// Gets whether a backup can be restored (one is selected).
    /// </summary>
    public bool CanRestoreBackup => SelectedBackup != null;

    #endregion

    #region Commands

    public ICommand RestoreBackupCommand { get; }
    public ICommand RefreshBackupsCommand { get; }
    public ICommand OpenBackupFolderCommand { get; }
    public ICommand CloseCommand { get; }

    #endregion

    public SettingsController(Window window)
    {
        m_window = window;
        m_backups = [];

        RestoreBackupCommand = new RelayCommand(restoreBackup, _ => CanRestoreBackup);
        RefreshBackupsCommand = new RelayCommand(_ => refreshBackups());
        OpenBackupFolderCommand = new RelayCommand(_ => openBackupFolder());
        CloseCommand = new RelayCommand(_ => m_window.Close());

        refreshBackups();
    }

    /// <summary>
    /// Refreshes the list of available backups.
    /// </summary>
    private void refreshBackups()
    {
        var backups = SDatabaseBackup.getAvailableBackups();
        Backups = new ObservableCollection<FileInfo>(backups);
        SelectedBackup = null;
    }

    /// <summary>
    /// Opens the backup folder in Windows Explorer.
    /// </summary>
    private static void openBackupFolder()
    {
        try
        {
            // Ensure the directory exists
            if (!Directory.Exists(BackupDirectory))
            {
                Directory.CreateDirectory(BackupDirectory);
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = BackupDirectory,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Could not open backup folder:\n{ex.Message}",
                "Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Restores the selected backup after confirmation.
    /// </summary>
    private void restoreBackup(object? parameter)
    {
        if (SelectedBackup == null)
        {
            return;
        }

        // Confirmation dialog
        var result = MessageBox.Show(
            $"Are you sure you want to restore the backup?\n\n" +
            $"Backup: {SelectedBackup.Name}\n" +
            $"Date: {SelectedBackup.LastWriteTime:dd/MM/yyyy HH:mm}\n\n" +
            "WARNING: This will replace all current data with the backup data.\n" +
            "This action cannot be undone!",
            "Confirm Restore",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            // Check if MariaDB is running
            if (!SMariaDbPortableService.isRunning())
            {
                MessageBox.Show(
                    "Database server is not running.\nPlease restart the application.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            // Perform the restore
            using var connection = new MySqlConnection(SMariaDbPortableService.ConnectionString);
            connection.Open();
            
            SDatabaseBackup.restoreBackup(connection, SelectedBackup.FullName);

            MessageBox.Show(
                $"Backup restored successfully!\n\n" +
                $"The database has been restored from:\n{SelectedBackup.Name}\n\n" +
                "Please restart the application for changes to take effect.",
                "Restore Complete",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Close the settings window
            m_window.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to restore backup:\n\n{ex.Message}",
                "Restore Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    protected virtual void onPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
