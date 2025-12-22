using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using MySqlConnector;

namespace Mams.src.services;

/// <summary>
/// Service to manage MariaDB Portable installation, startup, and database initialization.
/// </summary>
public static class SMariaDbPortableService
{
    private const string MARIADB_VERSION = "11.4.5";
    private const string MARIADB_DOWNLOAD_URL = $"https://archive.mariadb.org/mariadb-{MARIADB_VERSION}/winx64-packages/mariadb-{MARIADB_VERSION}-winx64.zip";
    private const string MARIADB_FOLDER_NAME = "mariadb";
    private const string DATABASE_NAME = "mams_db";
    private const string INIT_SQL_FILENAME = "init.sql";
    
    // Use a non-standard port to avoid conflicts with existing MySQL/MariaDB installations
    private const int MARIADB_PORT = 3307;
    
    private static Process? s_mariaDbProcess;
    private static readonly HttpClient s_httpClient = new() { Timeout = TimeSpan.FromMinutes(30) };

    /// <summary>
    /// Gets the path to the MariaDB portable installation folder.
    /// </summary>
    public static string MariaDbPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, MARIADB_FOLDER_NAME);

    /// <summary>
    /// Gets the path to the MariaDB bin folder.
    /// </summary>
    public static string MariaDbBinPath => Path.Combine(MariaDbPath, "bin");

    /// <summary>
    /// Gets the path to the MariaDB data folder.
    /// </summary>
    public static string MariaDbDataPath => Path.Combine(MariaDbPath, "data");

    /// <summary>
    /// Gets the path to the init.sql file in resources.
    /// </summary>
    public static string InitSqlPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ressources", INIT_SQL_FILENAME);

    /// <summary>
    /// Gets the connection string for the portable MariaDB instance.
    /// </summary>
    public static string ConnectionString => $"server=localhost;port={MARIADB_PORT};uid=root;pwd=;database={DATABASE_NAME};pooling=true;min pool size=5;max pool size=50;";

    /// <summary>
    /// Gets the connection string without database specified (for initial setup).
    /// </summary>
    public static string ConnectionStringNoDb => $"server=localhost;port={MARIADB_PORT};uid=root;pwd=;Connection Timeout=10;";

    /// <summary>
    /// Checks if MariaDB Portable is installed.
    /// </summary>
    public static bool isInstalled()
    {
        var mysqldPath = Path.Combine(MariaDbBinPath, "mysqld.exe");
        return File.Exists(mysqldPath);
    }

    /// <summary>
    /// Checks if MariaDB data directory is initialized.
    /// </summary>
    public static bool isDataInitialized()
    {
        return Directory.Exists(MariaDbDataPath) && 
               Directory.GetFiles(MariaDbDataPath, "*", SearchOption.AllDirectories).Length > 0;
    }

    /// <summary>
    /// Checks if the application database exists.
    /// </summary>
    public static bool isDatabaseCreated()
    {
        try
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Tests if our portable MariaDB is running and accepting connections on our specific port.
    /// </summary>
    public static bool isRunning()
    {
        try
        {
            using var connection = new MySqlConnection(ConnectionStringNoDb);
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Ensures MariaDB Portable is installed, running, and database is initialized.
    /// Shows progress dialogs to the user.
    /// </summary>
    /// <returns>True if MariaDB is ready to use, false if setup failed.</returns>
    public static async Task<bool> ensureMariaDbReadyAsync()
    {
        try
        {
            // Step 1: Check if our portable MariaDB is installed
            if (!isInstalled())
            {
                var installResult = MessageBox.Show(
                    "MariaDB database server is not installed.\n\n" +
                    "This application requires MariaDB to store data.\n" +
                    "MariaDB Portable will be downloaded and installed automatically.\n\n" +
                    "Download size: ~100 MB\n\n" +
                    "Would you like to proceed with the installation?",
                    "Database Setup Required",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (installResult != MessageBoxResult.Yes)
                {
                    return false;
                }

                if (!await downloadAndExtractMariaDbAsync())
                {
                    MessageBox.Show(
                        "Failed to download MariaDB.\n\n" +
                        "Please check your internet connection and try again.",
                        "Installation Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
            }

            // Step 2: Initialize data directory if needed
            if (!isDataInitialized())
            {
                Debug.WriteLine("[MariaDbPortable] Initializing data directory...");
                if (!initializeDataDirectory())
                {
                    MessageBox.Show(
                        "Failed to initialize MariaDB data directory.\n\n" +
                        "Please try restarting the application.",
                        "Initialization Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
            }

            // Step 3: Start MariaDB if not running on our port
            if (!isRunning())
            {
                Debug.WriteLine("[MariaDbPortable] Starting MariaDB...");
                if (!startMariaDb())
                {
                    MessageBox.Show(
                        "Failed to start MariaDB server.\n\n" +
                        "Please try restarting the application.",
                        "Startup Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }

                // Wait for MariaDB to be ready
                if (!await waitForMariaDbReadyAsync(30))
                {
                    MessageBox.Show(
                        "MariaDB server started but is not responding.\n\n" +
                        "Please try restarting the application.",
                        "Connection Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
            }

            // Step 4: Create database and run init.sql if needed
            if (!isDatabaseCreated())
            {
                Debug.WriteLine("[MariaDbPortable] Creating database...");
                if (!await initializeDatabaseAsync())
                {
                    MessageBox.Show(
                        "Failed to create the application database.\n\n" +
                        "Please try restarting the application.",
                        "Database Creation Failed",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return false;
                }
            }

            Debug.WriteLine("[MariaDbPortable] MariaDB is ready!");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Setup failed: {ex.Message}");
            MessageBox.Show(
                $"An error occurred during database setup:\n\n{ex.Message}",
                "Setup Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            return false;
        }
    }

    /// <summary>
    /// Downloads and extracts MariaDB Portable.
    /// </summary>
    private static async Task<bool> downloadAndExtractMariaDbAsync()
    {
        var tempZipPath = Path.Combine(Path.GetTempPath(), "mariadb-portable.zip");

        // Create progress window
        var progressWindow = new Window
        {
            Title = "Downloading MariaDB...",
            Width = 450,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.ToolWindow
        };

        var stackPanel = new StackPanel
        {
            Margin = new Thickness(20),
            VerticalAlignment = VerticalAlignment.Center
        };

        var statusText = new TextBlock
        {
            Text = "Connecting to server...",
            FontSize = 14,
            Margin = new Thickness(0, 0, 0, 10)
        };

        var progressBar = new ProgressBar
        {
            Height = 25,
            Minimum = 0,
            Maximum = 100,
            Value = 0
        };

        var progressText = new TextBlock
        {
            Text = "0%",
            FontSize = 12,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 5, 0, 0),
            Foreground = System.Windows.Media.Brushes.Gray
        };

        stackPanel.Children.Add(statusText);
        stackPanel.Children.Add(progressBar);
        stackPanel.Children.Add(progressText);
        progressWindow.Content = stackPanel;
        progressWindow.Show();

        try
        {
            // Download MariaDB
            Debug.WriteLine($"[MariaDbPortable] Downloading from: {MARIADB_DOWNLOAD_URL}");
            statusText.Text = "Downloading MariaDB...";

            using var response = await s_httpClient.GetAsync(MARIADB_DOWNLOAD_URL, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            var canReportProgress = totalBytes > 0;

            await using var contentStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true);

            var buffer = new byte[8192];
            long downloadedBytes = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer)) > 0)
            {
                await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead));
                downloadedBytes += bytesRead;

                if (canReportProgress)
                {
                    var percentage = (double)downloadedBytes / totalBytes * 100;
                    progressBar.Value = percentage;
                    progressText.Text = $"{percentage:F1}% ({formatBytes(downloadedBytes)} / {formatBytes(totalBytes)})";
                }
                else
                {
                    progressText.Text = $"Downloaded: {formatBytes(downloadedBytes)}";
                    progressBar.IsIndeterminate = true;
                }

                await Task.Delay(1); // Allow UI to update
            }

            Debug.WriteLine($"[MariaDbPortable] Download complete: {downloadedBytes} bytes");

            // Extract phase
            statusText.Text = "Extracting MariaDB...";
            progressBar.IsIndeterminate = true;
            progressText.Text = "Please wait...";
            await Task.Delay(100);

            // Close the file stream before extracting
            await fileStream.DisposeAsync();

            // Extract to temp location first
            var tempExtractPath = Path.Combine(Path.GetTempPath(), "mariadb-extract");
            if (Directory.Exists(tempExtractPath))
            {
                Directory.Delete(tempExtractPath, true);
            }

            Debug.WriteLine($"[MariaDbPortable] Extracting to: {tempExtractPath}");
            ZipFile.ExtractToDirectory(tempZipPath, tempExtractPath);

            // Find the extracted folder (it's usually named mariadb-version-winx64)
            var extractedFolders = Directory.GetDirectories(tempExtractPath);
            if (extractedFolders.Length == 0)
            {
                throw new Exception("No folder found in extracted archive");
            }

            var sourcePath = extractedFolders[0];

            // Move to final location
            if (Directory.Exists(MariaDbPath))
            {
                Directory.Delete(MariaDbPath, true);
            }

            Debug.WriteLine($"[MariaDbPortable] Moving to: {MariaDbPath}");
            Directory.Move(sourcePath, MariaDbPath);

            // Cleanup
            File.Delete(tempZipPath);
            Directory.Delete(tempExtractPath, true);

            Debug.WriteLine("[MariaDbPortable] Installation complete");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Download/extract failed: {ex.Message}");
            return false;
        }
        finally
        {
            progressWindow.Close();
        }
    }

    /// <summary>
    /// Initializes the MariaDB data directory.
    /// </summary>
    private static bool initializeDataDirectory()
    {
        try
        {
            var mysqlInstallDb = Path.Combine(MariaDbBinPath, "mysql_install_db.exe");
            if (!File.Exists(mysqlInstallDb))
            {
                // Fallback: use mysqld --initialize-insecure
                var mysqld = Path.Combine(MariaDbBinPath, "mysqld.exe");
                var startInfo = new ProcessStartInfo
                {
                    FileName = mysqld,
                    Arguments = $"--initialize-insecure --datadir=\"{MariaDbDataPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                using var process = Process.Start(startInfo);
                process?.WaitForExit(60000);
                return process?.ExitCode == 0;
            }

            // Use mysql_install_db for MariaDB
            var installStartInfo = new ProcessStartInfo
            {
                FileName = mysqlInstallDb,
                Arguments = $"--datadir=\"{MariaDbDataPath}\"",
                WorkingDirectory = MariaDbPath,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            Debug.WriteLine($"[MariaDbPortable] Running: {installStartInfo.FileName} {installStartInfo.Arguments}");

            using var initProcess = Process.Start(installStartInfo);
            initProcess?.WaitForExit(120000);

            var exitCode = initProcess?.ExitCode ?? -1;
            Debug.WriteLine($"[MariaDbPortable] mysql_install_db exit code: {exitCode}");

            return exitCode == 0 || isDataInitialized();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Data initialization failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Starts the MariaDB server process.
    /// </summary>
    public static bool startMariaDb()
    {
        try
        {
            if (s_mariaDbProcess != null && !s_mariaDbProcess.HasExited)
            {
                Debug.WriteLine("[MariaDbPortable] MariaDB process is already running");
                return true;
            }

            var mysqld = Path.Combine(MariaDbBinPath, "mysqld.exe");
            if (!File.Exists(mysqld))
            {
                Debug.WriteLine("[MariaDbPortable] mysqld.exe not found");
                return false;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = mysqld,
                Arguments = $"--datadir=\"{MariaDbDataPath}\" --port={MARIADB_PORT} --console",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = MariaDbBinPath
            };

            Debug.WriteLine($"[MariaDbPortable] Starting: {startInfo.FileName} {startInfo.Arguments}");

            s_mariaDbProcess = Process.Start(startInfo);

            if (s_mariaDbProcess == null)
            {
                Debug.WriteLine("[MariaDbPortable] Failed to start mysqld process");
                return false;
            }

            Debug.WriteLine($"[MariaDbPortable] mysqld started with PID: {s_mariaDbProcess.Id}");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Failed to start MariaDB: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Stops the MariaDB server process.
    /// </summary>
    public static void stopMariaDb()
    {
        try
        {
            if (s_mariaDbProcess != null && !s_mariaDbProcess.HasExited)
            {
                Debug.WriteLine("[MariaDbPortable] Stopping MariaDB...");

                // Try graceful shutdown first using mysqladmin
                var mysqladmin = Path.Combine(MariaDbBinPath, "mysqladmin.exe");
                if (File.Exists(mysqladmin))
                {
                    var shutdownInfo = new ProcessStartInfo
                    {
                        FileName = mysqladmin,
                        Arguments = $"-u root --port={MARIADB_PORT} shutdown",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var shutdownProcess = Process.Start(shutdownInfo);
                    shutdownProcess?.WaitForExit(10000);
                }

                // Wait for process to exit
                if (!s_mariaDbProcess.WaitForExit(5000))
                {
                    Debug.WriteLine("[MariaDbPortable] Forcing MariaDB shutdown...");
                    s_mariaDbProcess.Kill();
                }

                s_mariaDbProcess.Dispose();
                s_mariaDbProcess = null;

                Debug.WriteLine("[MariaDbPortable] MariaDB stopped");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Error stopping MariaDB: {ex.Message}");
        }
    }

    /// <summary>
    /// Waits for MariaDB to be ready to accept connections.
    /// </summary>
    private static async Task<bool> waitForMariaDbReadyAsync(int timeoutSeconds)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed.TotalSeconds < timeoutSeconds)
        {
            if (isRunning())
            {
                Debug.WriteLine($"[MariaDbPortable] MariaDB ready after {stopwatch.Elapsed.TotalSeconds:F1}s");
                return true;
            }
            await Task.Delay(500);
        }

        Debug.WriteLine($"[MariaDbPortable] Timeout waiting for MariaDB to be ready");
        return false;
    }

    /// <summary>
    /// Initializes the application database by running init.sql.
    /// </summary>
    private static async Task<bool> initializeDatabaseAsync()
    {
        try
        {
            // Read init.sql
            if (!File.Exists(InitSqlPath))
            {
                Debug.WriteLine($"[MariaDbPortable] init.sql not found at: {InitSqlPath}");
                
                // Create database without init.sql
                using var connection = new MySqlConnection(ConnectionStringNoDb);
                await connection.OpenAsync();
                using var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {DATABASE_NAME};", connection);
                await cmd.ExecuteNonQueryAsync();
                
                Debug.WriteLine("[MariaDbPortable] Created empty database (no init.sql found)");
                return true;
            }

            var initSql = await File.ReadAllTextAsync(InitSqlPath);
            Debug.WriteLine($"[MariaDbPortable] Running init.sql ({initSql.Length} chars)");

            // Connect without database first
            using (var connection = new MySqlConnection(ConnectionStringNoDb))
            {
                await connection.OpenAsync();

                // Split the SQL by semicolons and execute each statement
                var statements = initSql.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var statement in statements)
                {
                    var trimmedStatement = statement.Trim();
                    if (string.IsNullOrWhiteSpace(trimmedStatement) || trimmedStatement.StartsWith("--"))
                    {
                        continue;
                    }

                    try
                    {
                        using var cmd = new MySqlCommand(trimmedStatement, connection);
                        cmd.CommandTimeout = 30;
                        await cmd.ExecuteNonQueryAsync();
                    }
                    catch (MySqlException ex)
                    {
                        // Ignore "database already exists" or "table already exists" errors
                        // MySqlErrorCode: 1007 = DatabaseCreateExists, 1050 = TableExists
                        if (ex.Number != 1007 && ex.Number != 1050)
                        {
                            Debug.WriteLine($"[MariaDbPortable] SQL error: {ex.Message}");
                            Debug.WriteLine($"[MariaDbPortable] Statement: {trimmedStatement[..Math.Min(100, trimmedStatement.Length)]}...");
                        }
                    }
                }
            }

            Debug.WriteLine("[MariaDbPortable] Database initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Database initialization failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Formats bytes into a human-readable string.
    /// </summary>
    private static string formatBytes(long bytes)
    {
        string[] sizes = ["B", "KB", "MB", "GB"];
        int order = 0;
        double size = bytes;

        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:F2} {sizes[order]}";
    }
}
