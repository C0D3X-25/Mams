using Mams_App.src.commands;
using Mams_App.src.configurations;
using Mams_App.src.databaseOperations;
using Mams_App.src.integrities;
using Mams_App.src.localizations;
using Mams_App.src.navigations;
using Mams_App.src.regions;
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
using System.Windows.Media;

namespace Mams_App.src.settings;

/// <summary>
/// Controller for the Settings window. Handles backup listing, restoration, user settings, and localization.
/// </summary>
public class SettingsController : INotifyPropertyChanged
{
    private readonly Window m_window;
    private readonly UserModel m_userModel;
    private readonly RegionModel m_regionModel;
    private FileInfo? m_selectedBackup;
    private ObservableCollection<FileInfo> m_backups;

    // User properties
    private string m_userName = string.Empty;
    private string m_userPhone = string.Empty;
    private string m_userEmail = string.Empty;
    private string m_userCity = string.Empty;
    private string m_userAddress = string.Empty;

    // Localization properties
    private string m_selectedLanguage = "fr";
    private string m_selectedCurrency = "fr-CH";
    private LanguageItem? m_selectedLanguageItem;
    private CurrencyItem? m_selectedCurrencyItem;

    // Integrity check properties
    private bool m_isIntegrityCheckRunning;
    private double m_integrityCheckProgress;
    private string m_integrityCheckStatus = string.Empty;
    private bool m_hasIntegrityResult;
    private string m_integrityResultTitle = string.Empty;
    private string m_integrityResultMessage = string.Empty;
    private Brush m_integrityResultBackground = Brushes.Transparent;
    private ObservableCollection<string> m_problematicFiles = [];
    private CancellationTokenSource? m_integrityCheckCts;

    // Region properties
    private ObservableCollection<RegionItem> m_regionItems = [];
    private RegionItem? m_selectedRegionItem;
    private string m_regionEditName = string.Empty;

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
    /// Gets the list of available languages.
    /// </summary>
    public ObservableCollection<LanguageItem> AvailableLanguages { get; }

    /// <summary>
    /// Gets the list of available currencies.
    /// </summary>
    public ObservableCollection<CurrencyItem> AvailableCurrencies { get; }

    /// <summary>
    /// Gets or sets the selected language item.
    /// </summary>
    public LanguageItem? SelectedLanguageItem
    {
        get => m_selectedLanguageItem;
        set
        {
            m_selectedLanguageItem = value;
            if (value != null)
            {
                m_selectedLanguage = value.LanguageCode;
            }
            onPropertyChanged();
        }
    }

    /// <summary>
    /// Gets or sets the selected currency item.
    /// </summary>
    public CurrencyItem? SelectedCurrencyItem
    {
        get => m_selectedCurrencyItem;
        set
        {
            m_selectedCurrencyItem = value;
            if (value != null)
            {
                m_selectedCurrency = value.CultureCode;
            }
            onPropertyChanged();
        }
    }

    // Integrity check properties
    public bool IsIntegrityCheckRunning
    {
        get => m_isIntegrityCheckRunning;
        private set
        {
            m_isIntegrityCheckRunning = value;
            onPropertyChanged();
            onPropertyChanged(nameof(CanStartIntegrityCheck));
            onPropertyChanged(nameof(HasIntegrityStatus));
        }
    }

    public bool CanStartIntegrityCheck => !IsIntegrityCheckRunning;

    public double IntegrityCheckProgress
    {
        get => m_integrityCheckProgress;
        private set
        {
            m_integrityCheckProgress = value;
            onPropertyChanged();
        }
    }

    public string IntegrityCheckStatus
    {
        get => m_integrityCheckStatus;
        private set
        {
            m_integrityCheckStatus = value;
            onPropertyChanged();
            onPropertyChanged(nameof(HasIntegrityStatus));
        }
    }

    public bool HasIntegrityStatus => !string.IsNullOrEmpty(IntegrityCheckStatus) || IsIntegrityCheckRunning;

    public bool HasIntegrityResult
    {
        get => m_hasIntegrityResult;
        private set
        {
            m_hasIntegrityResult = value;
            onPropertyChanged();
        }
    }

    public string IntegrityResultTitle
    {
        get => m_integrityResultTitle;
        private set
        {
            m_integrityResultTitle = value;
            onPropertyChanged();
        }
    }

    public string IntegrityResultMessage
    {
        get => m_integrityResultMessage;
        private set
        {
            m_integrityResultMessage = value;
            onPropertyChanged();
        }
    }

    public Brush IntegrityResultBackground
    {
        get => m_integrityResultBackground;
        private set
        {
            m_integrityResultBackground = value;
            onPropertyChanged();
        }
    }

    public ObservableCollection<string> ProblematicFiles
    {
        get => m_problematicFiles;
        private set
        {
            m_problematicFiles = value;
            onPropertyChanged();
            onPropertyChanged(nameof(HasProblematicFiles));
        }
    }

    public bool HasProblematicFiles => ProblematicFiles.Count > 0;

    // Region properties
    public ObservableCollection<RegionItem> RegionItems
    {
        get => m_regionItems;
        private set
        {
            m_regionItems = value;
            onPropertyChanged();
        }
    }

    public RegionItem? SelectedRegionItem
    {
        get => m_selectedRegionItem;
        set
        {
            m_selectedRegionItem = value;
            onPropertyChanged();
            if (value != null)
            {
                RegionEditName = value.region_name;
            }
        }
    }

    public string RegionEditName
    {
        get => m_regionEditName;
        set
        {
            m_regionEditName = value;
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
    public ICommand StartIntegrityCheckCommand { get; }
    public ICommand AcknowledgeIntegrityResultCommand { get; }
    public ICommand AddRegionCommand { get; }
    public ICommand SaveRegionCommand { get; }
    public ICommand DeleteRegionCommand { get; }

    #endregion

    public SettingsController(Window window)
    {
        m_window = window;
        m_backups = [];
        m_userModel = new UserModel();
        m_regionModel = new RegionModel();

        // Initialize language and currency collections
        AvailableLanguages =
        [
            new LanguageItem("fr", Loc.Get("Language.French")),
            new LanguageItem("en", Loc.Get("Language.English")),
            new LanguageItem("de", Loc.Get("Language.German")),
            new LanguageItem("it", Loc.Get("Language.Italian"))
        ];

        AvailableCurrencies =
        [
            new CurrencyItem("fr-CH", Loc.Get("Currency.CHF"), "CHF"),
            new CurrencyItem("fr-FR", Loc.Get("Currency.EUR"), "€"),
            new CurrencyItem("en-US", Loc.Get("Currency.USD"), "$"),
            new CurrencyItem("en-GB", Loc.Get("Currency.GBP"), "£")
        ];

        RestoreBackupCommand = new RelayCommand(restoreBackup, _ => CanRestoreBackup);
        RefreshBackupsCommand = new RelayCommand(_ => refreshBackups());
        OpenBackupFolderCommand = new RelayCommand(_ => openBackupFolder());
        OpenSettingsFolderCommand = new RelayCommand(_ => openSettingsFolder());
        CloseCommand = new RelayCommand(_ => m_window.Close());
        SaveUserCommand = new RelayCommand(_ => saveUser());
        SaveLocalizationCommand = new RelayCommand(_ => saveLocalization());
        StartIntegrityCheckCommand = new RelayCommand(_ => startIntegrityCheck(), _ => CanStartIntegrityCheck);
        AcknowledgeIntegrityResultCommand = new RelayCommand(_ => acknowledgeIntegrityResult());
        AddRegionCommand = new RelayCommand(_ => addRegion(), _ => !string.IsNullOrWhiteSpace(RegionEditName));
        SaveRegionCommand = new RelayCommand(_ => saveRegion(), _ => SelectedRegionItem != null && !string.IsNullOrWhiteSpace(RegionEditName));
        DeleteRegionCommand = new RelayCommand(_ => deleteRegion(), _ => SelectedRegionItem != null);

        refreshBackups();
        loadUser();
        loadLocalization();
        loadRegions();
    }

    /// <summary>
    /// Loads the user data from the database.
    /// </summary>
    private void loadUser()
    {
        try
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
        catch (Exception ex)
        {
            Debug.WriteLine($"[SettingsController] Error loading user data: {ex.Message}");
            // User data will remain empty - user can still use other settings
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
        m_selectedLanguage = config.m_localization.m_language;
        m_selectedCurrency = config.m_localization.m_culture;

        // Find the matching language item
        SelectedLanguageItem = AvailableLanguages.FirstOrDefault(l => l.LanguageCode == m_selectedLanguage)
            ?? AvailableLanguages.First();

        // Find the matching currency item
        SelectedCurrencyItem = AvailableCurrencies.FirstOrDefault(c => c.CultureCode == m_selectedCurrency)
            ?? AvailableCurrencies.First();
    }

    /// <summary>
    /// Saves the localization settings to the configuration and restarts the application.
    /// </summary>
    private void saveLocalization()
    {
        var config = SAppConfigService.loadConfig();
        config.m_localization.m_language = m_selectedLanguage;
        config.m_localization.m_culture = m_selectedCurrency;
        SAppConfigService.saveConfig(config);

        MessageBox.Show(
            Loc.Get("Message.LocalizationRestartRequired"),
            Loc.Get("Message.UserSaveSuccessTitle"),
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        // Restart the application
        var exePath = Environment.ProcessPath;
        if (!string.IsNullOrEmpty(exePath))
        {
            Process.Start(exePath);
            Application.Current.Shutdown();
        }
    }

    /// <summary>
    /// Refreshes the list of available backups.
    /// </summary>
    private void refreshBackups()
    {
        try
        {
            var backups = SDatabaseBackup.getAvailableBackups();
            Backups = new ObservableCollection<FileInfo>(backups);
            SelectedBackup = null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SettingsController] Error refreshing backups: {ex.Message}");
            Backups = [];
            SelectedBackup = null;
        }
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

            // Show success message
            MessageBox.Show(
                Loc.Get("Message.BackupRestoreSuccess", SelectedBackup.Name),
                Loc.Get("Message.BackupRestoreSuccessTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // Close the settings window and refresh the current page to reload data
            m_window.Close();
            SPageNavigationController.refreshCurrentPage();
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

    /// <summary>
    /// Starts the integrity check operation asynchronously.
    /// </summary>
    private async void startIntegrityCheck()
    {
        if (IsIntegrityCheckRunning)
            return;

        // Reset state
        HasIntegrityResult = false;
        IntegrityCheckProgress = 0;
        IntegrityCheckStatus = Loc.Get("Settings.IntegrityCheckInProgress");
        ProblematicFiles = [];
        IsIntegrityCheckRunning = true;

        m_integrityCheckCts = new CancellationTokenSource();

        try
        {
            var result = await SIntegrityService.verifyAndRepairAsync(
                requiredOnly: false,
                verifyChecksums: true,
                progressCallback: (status, progress) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        IntegrityCheckStatus = status;
                        if (progress.HasValue)
                        {
                            IntegrityCheckProgress = progress.Value;
                        }
                    });
                },
                cancellationToken: m_integrityCheckCts.Token);

            // Display results
            displayIntegrityResult(result);
        }
        catch (OperationCanceledException)
        {
            IntegrityCheckStatus = string.Empty;
        }
        catch (Exception ex)
        {
            IntegrityResultTitle = Loc.Get("Settings.IntegrityCheckError");
            IntegrityResultMessage = ex.Message;
            IntegrityResultBackground = new SolidColorBrush(Color.FromRgb(254, 226, 226)); // Light red
            HasIntegrityResult = true;
        }
        finally
        {
            IsIntegrityCheckRunning = false;
            IntegrityCheckStatus = string.Empty;
            m_integrityCheckCts?.Dispose();
            m_integrityCheckCts = null;
        }
    }

    /// <summary>
    /// Displays the integrity check result to the user.
    /// </summary>
    /// <param name="result">The integrity check result.</param>
    private void displayIntegrityResult(IntegrityCheckResult result)
    {
        var problematicFiles = new List<string>();
        problematicFiles.AddRange(result.MissingFiles);
        problematicFiles.AddRange(result.CorruptedFiles);
        problematicFiles.AddRange(result.CorruptedAppFiles);
        problematicFiles.AddRange(result.FailedRepairs);

        if (result.IsValid && problematicFiles.Count == 0)
        {
            // Success
            IntegrityResultTitle = Loc.Get("Settings.IntegrityCheckSuccess");
            IntegrityResultMessage = Loc.Get("Settings.IntegrityCheckSuccessMessage");
            IntegrityResultBackground = new SolidColorBrush(Color.FromRgb(220, 252, 231)); // Light green
            ProblematicFiles = [];
        }
        else
        {
            // Failed
            IntegrityResultTitle = Loc.Get("Settings.IntegrityCheckFailed");
            IntegrityResultMessage = Loc.Get("Settings.IntegrityCheckFailedMessage");
            IntegrityResultBackground = new SolidColorBrush(Color.FromRgb(254, 226, 226)); // Light red
            ProblematicFiles = new ObservableCollection<string>(problematicFiles);
        }

        HasIntegrityResult = true;
    }

    /// <summary>
    /// Acknowledges and hides the integrity result.
    /// </summary>
    private void acknowledgeIntegrityResult()
    {
        HasIntegrityResult = false;
        IntegrityResultTitle = string.Empty;
        IntegrityResultMessage = string.Empty;
        ProblematicFiles = [];
    }

    /// <summary>
    /// Loads the list of regions from the database.
    /// </summary>
    private void loadRegions()
    {
        try
        {
            var result = m_regionModel.getAllItems();
            RegionItems = new ObservableCollection<RegionItem>(
                result.returned_items.Where(r => !string.IsNullOrEmpty(r.region_name)));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[SettingsController] Error loading regions: {ex.Message}");
            RegionItems = [];
        }
    }

    /// <summary>
    /// Adds a new region to the database.
    /// </summary>
    private void addRegion()
    {
        var item = new RegionItem { region_name = RegionEditName.Trim() };
        var result = m_regionModel.saveItem(item);
        if (result.is_success)
        {
            RegionEditName = string.Empty;
            loadRegions();
        }
        else
        {
            MessageBox.Show(
                Loc.Get("Message.SaveError"),
                Loc.Get("Common.Error"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Saves (updates) the selected region in the database.
    /// </summary>
    private void saveRegion()
    {
        if (SelectedRegionItem == null) return;

        var item = new RegionItem
        {
            region_id = SelectedRegionItem.region_id,
            region_name = RegionEditName.Trim()
        };

        var result = m_regionModel.saveItem(item);
        if (result.is_success)
        {
            loadRegions();
        }
        else
        {
            MessageBox.Show(
                Loc.Get("Message.SaveError"),
                Loc.Get("Common.Error"),
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Deletes the selected region from the database.
    /// </summary>
    private void deleteRegion()
    {
        if (SelectedRegionItem == null) return;

        var result = m_regionModel.deleteItem(SelectedRegionItem.region_id.ToString());
        if (result.is_success)
        {
            SelectedRegionItem = null;
            RegionEditName = string.Empty;
            loadRegions();
        }
        else
        {
            MessageBox.Show(
                Loc.Get("Message.DeleteErrorOccurred"),
                Loc.Get("Common.Error"),
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
/// Represents a language option for the localization settings.
/// </summary>
public class LanguageItem
{
    public string LanguageCode { get; }
    public string DisplayName { get; }

    public LanguageItem(string languageCode, string displayName)
    {
        LanguageCode = languageCode;
        DisplayName = displayName;
    }

    public override string ToString() => DisplayName;
}

/// <summary>
/// Represents a currency option for the localization settings.
/// </summary>
public class CurrencyItem
{
    public string CultureCode { get; }
    public string DisplayName { get; }
    public string Symbol { get; }

    public CurrencyItem(string cultureCode, string displayName, string symbol)
    {
        CultureCode = cultureCode;
        DisplayName = displayName;
        Symbol = symbol;
    }

    public override string ToString() => $"{DisplayName} ({Symbol})";
}

