using Mams_App.src.localizations;
using MySqlConnector;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace Mams_App.src.services;

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
    private static CancellationTokenSource? s_downloadCancellationTokenSource;

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
    public static string InitSqlPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "resources", INIT_SQL_FILENAME);

    /// <summary>
    /// Gets the connection string for the portable MariaDB instance.
    /// </summary>
    public static string ConnectionString => $"server=localhost;port={MARIADB_PORT};uid=root;pwd=;database={DATABASE_NAME};charset=utf8mb4;pooling=true;min pool size=5;max pool size=50;";

    /// <summary>
    /// Gets the connection string without database specified (for initial setup).
    /// </summary>
    public static string ConnectionStringNoDb => $"server=localhost;port={MARIADB_PORT};uid=root;pwd=;charset=utf8mb4;Connection Timeout=10;";

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
    /// Tests if our portable MariaDB is running and accepting connections on our specific port (async version).
    /// </summary>
    public static async Task<bool> isRunningAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = new MySqlConnection(ConnectionStringNoDb);
            await connection.OpenAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Cancels any ongoing download operation.
    /// </summary>
    public static void cancelDownload()
    {
        s_downloadCancellationTokenSource?.Cancel();
    }

    /// <summary>
    /// Downloads and installs MariaDB Portable with progress reporting.
    /// </summary>
    /// <param name="progressCallback">Callback for progress updates (status message, percentage 0-100 or null for indeterminate)</param>
    /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
    /// <returns>True if installation succeeded, false otherwise.</returns>
    public static async Task<bool> downloadAndInstallMariaDbAsync(Action<string, double?>? progressCallback = null, CancellationToken cancellationToken = default)
    {
        s_downloadCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        var linkedToken = s_downloadCancellationTokenSource.Token;
        var tempZipPath = Path.Combine(Path.GetTempPath(), $"mariadb-portable-{Guid.NewGuid()}.zip");

        try
        {
            // Download MariaDB
            Debug.WriteLine($"[MariaDbPortable] Downloading from: {MARIADB_DOWNLOAD_URL}");
            progressCallback?.Invoke(Loc.Get("Launcher.DownloadingMariaDb") ?? "Downloading MariaDB...", null);

            using (var response = await s_httpClient.GetAsync(MARIADB_DOWNLOAD_URL, HttpCompletionOption.ResponseHeadersRead, linkedToken))
            {
                response.EnsureSuccessStatusCode();

                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                var canReportProgress = totalBytes > 0;

                using (var contentStream = await response.Content.ReadAsStreamAsync(linkedToken))
                using (var fileStream = new FileStream(tempZipPath, FileMode.Create, FileAccess.Write, FileShare.None, 65536, true))
                {
                    var buffer = new byte[65536];
                    long downloadedBytes = 0;
                    long lastReportedBytes = 0;
                    const long progressReportInterval = 102400;
                    int bytesRead;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, linkedToken)) > 0)
                    {
                        linkedToken.ThrowIfCancellationRequested();
                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), linkedToken);
                        downloadedBytes += bytesRead;

                        if (downloadedBytes - lastReportedBytes >= progressReportInterval)
                        {
                            lastReportedBytes = downloadedBytes;

                            if (canReportProgress)
                            {
                                var percentage = (double)downloadedBytes / totalBytes * 100;
                                var statusText = $"{Loc.Get("Launcher.DownloadingMariaDb") ?? "Downloading MariaDB..."}\n{percentage:F1}% ({formatBytes(downloadedBytes)} / {formatBytes(totalBytes)})";
                                progressCallback?.Invoke(statusText, percentage);
                            }
                            else
                            {
                                progressCallback?.Invoke($"{Loc.Get("Launcher.DownloadingMariaDb") ?? "Downloading MariaDB..."}\n{formatBytes(downloadedBytes)}", null);
                            }

                            await Task.Yield();
                        }
                    }

                    if (canReportProgress)
                    {
                        progressCallback?.Invoke($"{Loc.Get("Launcher.DownloadingMariaDb") ?? "Downloading MariaDB..."}\n100%", 100);
                    }
                }

                Debug.WriteLine($"[MariaDbPortable] Download complete");
            }

            linkedToken.ThrowIfCancellationRequested();

            // Extract phase - run on background thread to keep UI responsive
            progressCallback?.Invoke(Loc.Get("Launcher.ExtractingMariaDb") ?? "Extracting MariaDB...", null);
            await Task.Delay(100, linkedToken);

            var tempExtractPath = Path.Combine(Path.GetTempPath(), $"mariadb-extract-{Guid.NewGuid()}");

            await Task.Run(() =>
            {
                linkedToken.ThrowIfCancellationRequested();
                if (Directory.Exists(tempExtractPath))
                {
                    Directory.Delete(tempExtractPath, true);
                }

                Debug.WriteLine($"[MariaDbPortable] Extracting to: {tempExtractPath}");
                ZipFile.ExtractToDirectory(tempZipPath, tempExtractPath);
            }, linkedToken);

            linkedToken.ThrowIfCancellationRequested();

            var extractedFolders = Directory.GetDirectories(tempExtractPath);
            if (extractedFolders.Length == 0)
            {
                throw new Exception("No folder found in extracted archive");
            }

            var sourcePath = extractedFolders[0];

            await Task.Run(() =>
            {
                linkedToken.ThrowIfCancellationRequested();
                if (Directory.Exists(MariaDbPath))
                {
                    Directory.Delete(MariaDbPath, true);
                }

                Debug.WriteLine($"[MariaDbPortable] Moving to: {MariaDbPath}");
                Directory.Move(sourcePath, MariaDbPath);
            }, linkedToken);

            // Cleanup
            try
            {
                File.Delete(tempZipPath);
                Directory.Delete(tempExtractPath, true);
            }
            catch (Exception cleanupEx)
            {
                Debug.WriteLine($"[MariaDbPortable] Cleanup warning: {cleanupEx.Message}");
            }

            Debug.WriteLine("[MariaDbPortable] Installation complete");
            return true;
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[MariaDbPortable] Download/install cancelled");
            try
            {
                if (File.Exists(tempZipPath))
                    File.Delete(tempZipPath);
            }
            catch { }
            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Download/extract failed: {ex.Message}");

            try
            {
                if (File.Exists(tempZipPath))
                    File.Delete(tempZipPath);
            }
            catch { }

            return false;
        }
        finally
        {
            s_downloadCancellationTokenSource?.Dispose();
            s_downloadCancellationTokenSource = null;
        }
    }

    /// <summary>
    /// Initializes the MariaDB data directory.
    /// </summary>
    public static async Task<bool> initializeDataDirectoryAsync(CancellationToken cancellationToken = default)
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
                if (process != null)
                {
                    await process.WaitForExitAsync(cancellationToken);
                    return process.ExitCode == 0;
                }
                return false;
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
            if (initProcess != null)
            {
                await initProcess.WaitForExitAsync(cancellationToken);
                var exitCode = initProcess.ExitCode;
                Debug.WriteLine($"[MariaDbPortable] mysql_install_db exit code: {exitCode}");
                return exitCode == 0 || isDataInitialized();
            }
            return false;
        }
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[MariaDbPortable] Data initialization cancelled");
            return false;
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
    public static async Task<bool> startMariaDbAsync()
    {
        try
        {
            // Check if we already have a tracked process running
            if (s_mariaDbProcess != null && !s_mariaDbProcess.HasExited)
            {
                Debug.WriteLine("[MariaDbPortable] MariaDB process is already running (tracked)");
                return true;
            }

            // Check if MariaDB is already running (from previous app instance or update)
            if (await isRunningAsync())
            {
                Debug.WriteLine("[MariaDbPortable] MariaDB is already running on our port (external/previous instance)");
                // Try to find and track the existing process
                tryAttachToExistingProcess();
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
    /// Starts the MariaDB server process (synchronous version for non-UI scenarios).
    /// </summary>
    public static bool startMariaDb()
    {
        try
        {
            // Check if we already have a tracked process running
            if (s_mariaDbProcess != null && !s_mariaDbProcess.HasExited)
            {
                Debug.WriteLine("[MariaDbPortable] MariaDB process is already running (tracked)");
                return true;
            }

            // Check if MariaDB is already running (from previous app instance or update)
            if (isRunning())
            {
                Debug.WriteLine("[MariaDbPortable] MariaDB is already running on our port (external/previous instance)");
                // Try to find and track the existing process
                tryAttachToExistingProcess();
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
    /// Tries to find and attach to an existing mysqld process running from our installation.
    /// </summary>
    private static void tryAttachToExistingProcess()
    {
        try
        {
            var mysqldPath = Path.Combine(MariaDbBinPath, "mysqld.exe");
            var processes = Process.GetProcessesByName("mysqld");

            foreach (var proc in processes)
            {
                try
                {
                    // Check if this process is from our MariaDB installation
                    if (proc.MainModule?.FileName?.Equals(mysqldPath, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        s_mariaDbProcess = proc;
                        Debug.WriteLine($"[MariaDbPortable] Attached to existing mysqld process PID: {proc.Id}");
                        return;
                    }
                }
                catch
                {
                    // Can't access MainModule for some processes, skip them
                }
            }

            Debug.WriteLine("[MariaDbPortable] Could not find matching mysqld process to attach");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Error finding existing process: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops the MariaDB server process.
    /// </summary>
    public static async Task stopMariaDbAsync()
    {
        try
        {
            // First, try to stop using mysqladmin (works even if we don't have process reference)
            if (await isRunningAsync())
            {
                Debug.WriteLine("[MariaDbPortable] Stopping MariaDB via mysqladmin...");

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
                    if (shutdownProcess != null)
                    {
                        await shutdownProcess.WaitForExitAsync();
                    }

                    // Wait a bit for shutdown to complete
                    await Task.Delay(2000);
                }
            }

            // If we have a tracked process, ensure it's stopped
            if (s_mariaDbProcess != null)
            {
                if (!s_mariaDbProcess.HasExited)
                {
                    Debug.WriteLine("[MariaDbPortable] Forcing tracked process to stop...");
                    using var cts = new CancellationTokenSource(3000);
                    try
                    {
                        await s_mariaDbProcess.WaitForExitAsync(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        s_mariaDbProcess.Kill();
                    }
                }

                s_mariaDbProcess.Dispose();
                s_mariaDbProcess = null;
            }

            Debug.WriteLine("[MariaDbPortable] MariaDB stopped");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Error stopping MariaDB: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops the MariaDB server process (synchronous version for shutdown scenarios).
    /// </summary>
    public static void stopMariaDb()
    {
        try
        {
            // First, try to stop using mysqladmin (works even if we don't have process reference)
            if (isRunning())
            {
                Debug.WriteLine("[MariaDbPortable] Stopping MariaDB via mysqladmin...");

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

                    // Wait a bit for shutdown to complete
                    Thread.Sleep(2000);
                }
            }

            // If we have a tracked process, ensure it's stopped
            if (s_mariaDbProcess != null)
            {
                if (!s_mariaDbProcess.HasExited)
                {
                    Debug.WriteLine("[MariaDbPortable] Forcing tracked process to stop...");
                    if (!s_mariaDbProcess.WaitForExit(3000))
                    {
                        s_mariaDbProcess.Kill();
                    }
                }

                s_mariaDbProcess.Dispose();
                s_mariaDbProcess = null;
            }

            Debug.WriteLine("[MariaDbPortable] MariaDB stopped");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MariaDbPortable] Error stopping MariaDB: {ex.Message}");
        }
    }

    /// <summary>
    /// Waits for MariaDB to be ready to accept connections.
    /// </summary>
    public static async Task<bool> waitForMariaDbReadyAsync(int timeoutSeconds, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        while (stopwatch.Elapsed.TotalSeconds < timeoutSeconds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (await isRunningAsync(cancellationToken))
            {
                Debug.WriteLine($"[MariaDbPortable] MariaDB ready after {stopwatch.Elapsed.TotalSeconds:F1}s");
                return true;
            }
            await Task.Delay(500, cancellationToken);
        }

        Debug.WriteLine($"[MariaDbPortable] Timeout waiting for MariaDB to be ready");
        return false;
    }

    /// <summary>
    /// Initializes the application database by running init.sql.
    /// </summary>
    public static async Task<bool> initializeDatabaseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Read init.sql
            if (!File.Exists(InitSqlPath))
            {
                Debug.WriteLine($"[MariaDbPortable] init.sql not found at: {InitSqlPath}");

                // Create database without init.sql
                using var connection = new MySqlConnection(ConnectionStringNoDb);
                await connection.OpenAsync(cancellationToken);
                using var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS {DATABASE_NAME};", connection);
                await cmd.ExecuteNonQueryAsync(cancellationToken);

                Debug.WriteLine("[MariaDbPortable] Created empty database (no init.sql found)");
                return true;
            }

            var initSql = await File.ReadAllTextAsync(InitSqlPath, cancellationToken);
            Debug.WriteLine($"[MariaDbPortable] Running init.sql ({initSql.Length} chars)");

            // Connect without database first
            using (var connection = new MySqlConnection(ConnectionStringNoDb))
            {
                await connection.OpenAsync(cancellationToken);

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
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
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
        catch (OperationCanceledException)
        {
            Debug.WriteLine("[MariaDbPortable] Database initialization cancelled");
            return false;
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
