using Mams_App.src.commands;
using Mams_App.src.configurations;
using Mams_App.src.databaseOperations;
using Mams_App.src.localizations;
using Mams_App.src.services;
using Mams_App.src.users;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Mams_App.src.settings;

/// <summary>
/// Controller for the Settings window. Handles backup listing, restoration, user settings, and localization.
/// </summary>
public class SettingsController : INotifyPropertyChanged
{
    private readonly Window m_window;
    private readonly UserModel m_userModel;
    private FileInfo? m_selectedBackup;
    private ObservableCollection<FileInfo> m_backups;

    // User properties
    private string m_userName = string.Empty;
    private string m_userPhone = string.Empty;
    private string m_userEmail = string.Empty;
    private string m_userCity = string.Empty;
    private string m_userAddress = string.Empty;

    // Localization properties
    private string m_selectedCulture = "fr-CH";
    private CultureItem? m_selectedCultureItem;

    public event PropertyChangedEventHandler? PropertyChanged;

    #region Properties

    /// <summary>
    /// Gets the application version.
    /// </summary>
    public static string AppVersion => SVersionService.GetVersion();

    /// <summary>
    /// Gets the backup directory path.
    /// </summary>
    public static string BackupDirectory => SDatabaseBackup.BackupDirectoryPath;

    /// <summary>
    /// Gets the settings file path.
    /// </summary>
    public static string SettingsFilePath => SAppConfigService.SettingsFilePath;

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

    // User properties for binding
    public string UserName
    {
        get => m_userName;
        set
        {
            m_userName = value;
            onPropertyChanged();
        }
    }

    public string UserPhone
    {
        get => m_userPhone;
        set
        {
            m_userPhone = value;
            onPropertyChanged();
        }
    }

    public string UserEmail
    {
        get => m_userEmail;
        set
        {
            m_userEmail = value;
            onPropertyChanged();
        }
    }

    public string UserCity
    {
        get => m_userCity;
        set
        {
            m_userCity = value;
            onPropertyChanged();
        }
    }

    public string UserAddress
    {
        get => m_userAddress;
        set
        {
            m_userAddress = value;
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Gets the list of available cultures for currency formatting.
    /// </summary>
    public ObservableCollection<CultureItem> AvailableCultures { get; } =
    [
        new CultureItem("fr-CH", Loc.Get("Culture.frCH"), Loc.Get("Currency.CHF")),
        new CultureItem("fr-FR", Loc.Get("Culture.frFR"), Loc.Get("Currency.EUR")),
        new CultureItem("de-DE", Loc.Get("Culture.deDE"), Loc.Get("Currency.EUR")),
        new CultureItem("de-CH", Loc.Get("Culture.deCH"), Loc.Get("Currency.CHF")),
        new CultureItem("it-CH", Loc.Get("Culture.itCH"), Loc.Get("Currency.CHF")),
        new CultureItem("en-US", Loc.Get("Culture.enUS"), Loc.Get("Currency.USD")),
        new CultureItem("en-GB", Loc.Get("Culture.enGB"), Loc.Get("Currency.GBP"))
    ];

    /// <summary>
    /// Gets or sets the selected culture item.
    /// </summary>
    public CultureItem? SelectedCultureItem
    {
        get => m_selectedCultureItem;
        set
        {
            m_selectedCultureItem = value;
            if (value != null)
            {
                m_selectedCulture = value.CultureCode;
            }
            onPropertyChanged();
        }
    }

    #endregion

    #region Commands

    public ICommand RestoreBackupCommand { get; }
    public ICommand RefreshBackupsCommand { get; }
    public ICommand OpenBackupFolderCommand { get; }
    public ICommand OpenSettingsFolderCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand SaveUserCommand { get; }
    public ICommand SaveLocalizationCommand { get; }

    #endregion

    public SettingsController(Window window)
    {
        m_window = window;
        m_backups = [];
        m_userModel = new UserModel();

        RestoreBackupCommand = new RelayCommand(restoreBackup, _ => CanRestoreBackup);
        RefreshBackupsCommand = new RelayCommand(_ => refreshBackups());
        OpenBackupFolderCommand = new RelayCommand(_ => openBackupFolder());
        OpenSettingsFolderCommand = new RelayCommand(_ => openSettingsFolder());
        CloseCommand = new RelayCommand(_ => m_window.Close());
        SaveUserCommand = new RelayCommand(_ => saveUser());
        SaveLocalizationCommand = new RelayCommand(_ => saveLocalization());

        refreshBackups();
        loadUser();
        loadLocalization();
    }

    /// <summary>
    /// Loads the user data from the database.
    /// </summary>
    private void loadUser()
    {
        var result = m_userModel.getUser();
        if (result.is_found && result.returned_item != null)
        {
            UserName = result.returned_item.user_name;
            UserPhone = result.returned_item.user_phone;
            UserEmail = result.returned_item.user_email;
            UserCity = result.returned_item.user_city;
            UserAddress = result.returned_item.user_address;
        }
    }

    /// <summary>
    /// Saves the user data to the database.
    /// </summary>
    private void saveUser()
    {
        var userItem = new UserItem
        {
            user_id = 1,
            user_name = UserName,
            user_phone = UserPhone,
            user_email = UserEmail,
            user_city = UserCity,
            user_address = UserAddress
        };

        var result = m_userModel.saveUser(userItem);
        if (result.is_success)
        {
            MessageBox.Show(
                Loc.Get("Message.UserSaveSuccess"),
                Loc.Get("Message.UserSaveSuccessTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        else
        {
            MessageBox.Show(
                Loc.Get("Message.UserSaveError", result.error_message_detail ?? string.Empty),
                Loc.Get("Common.Error"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Loads the localization settings from the configuration.
    /// </summary>
    private void loadLocalization()
    {
        var config = SAppConfigService.loadConfig();
        m_selectedCulture = config.m_localization.m_culture;

        // Find the matching culture item
        SelectedCultureItem = AvailableCultures.FirstOrDefault(c => c.CultureCode == m_selectedCulture)
            ?? AvailableCultures.First();
    }

    /// <summary>
    /// Saves the localization settings to the configuration.
    /// </summary>
    private void saveLocalization()
    {
        var config = SAppConfigService.loadConfig();
        config.m_localization.m_culture = m_selectedCulture;
        SAppConfigService.saveConfig(config);

        MessageBox.Show(
            Loc.Get("Message.LocalizationSaveSuccess"),
            Loc.Get("Message.UserSaveSuccessTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);
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
                Loc.Get("Message.OpenFolderError", ex.Message),
                Loc.Get("Common.Error"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Opens the settings folder in Windows Explorer.
    /// </summary>
    private static void openSettingsFolder()
    {
        try
        {
            var settingsFolder = Path.GetDirectoryName(SettingsFilePath);
            if (string.IsNullOrEmpty(settingsFolder))
            {
                return;
            }

            // Ensure the directory exists
            if (!Directory.Exists(settingsFolder))
            {
                Directory.CreateDirectory(settingsFolder);
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = settingsFolder,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                Loc.Get("Message.OpenFolderError", ex.Message),
                Loc.Get("Common.Error"),
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
            Loc.Get("Message.BackupRestoreConfirm", SelectedBackup.Name, SelectedBackup.LastWriteTime.ToString("dd/MM/yyyy HH:mm")),
            Loc.Get("Message.BackupRestoreConfirmTitle"),
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
                    Loc.Get("Message.DatabaseNotRunning"),
                    Loc.Get("Common.Error"),
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            // Perform the restore
            using var connection = new MySqlConnection(SMariaDbPortableService.ConnectionString);
            connection.Open();

            SDatabaseBackup.restoreBackup(connection, SelectedBackup.FullName);

            MessageBox.Show(
                Loc.Get("Message.BackupRestoreSuccess", SelectedBackup.Name),
                Loc.Get("Message.BackupRestoreSuccessTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Close the settings window
            m_window.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                Loc.Get("Message.BackupRestoreError", ex.Message),
                Loc.Get("Message.BackupRestoreErrorTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    protected virtual void onPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

/// <summary>
/// Represents a culture option for the localization settings.
/// </summary>
public class CultureItem
{
    public string CultureCode { get; }
    public string DisplayName { get; }
    public string CurrencySymbol { get; }

    public CultureItem(string cultureCode, string displayName, string currencySymbol)
    {
        CultureCode = cultureCode;
        DisplayName = displayName;
        CurrencySymbol = currencySymbol;
    }

    public override string ToString() => DisplayName;
}

