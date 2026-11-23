using Mams_App.src.services;
using MySqlConnector;

namespace Mams_Test.integration;

/// <summary>
/// Shared xUnit fixture that creates and initializes a dedicated test database.
/// Used via <see cref="DatabaseCollectionDefinition"/> to share a single instance
/// across all integration test classes in the collection.
/// </summary>
public class DatabaseFixture : IDisposable
{
    private const string TEST_DATABASE_NAME = "mams_test_db";
    private const int MARIADB_PORT = 3307;
    private const int MAX_STARTUP_WAIT_SECONDS = 30;

    private static readonly string _connectionStringNoDb =
        $"server=localhost;port={MARIADB_PORT};uid=root;pwd=;charset=utf8mb4;Connection Timeout=10;";

    public static readonly string TestConnectionString =
        $"server=localhost;port={MARIADB_PORT};uid=root;pwd=;database={TEST_DATABASE_NAME};charset=utf8mb4;pooling=true;min pool size=1;max pool size=10;";

    /// <summary>
    /// Gets the list of all user tables in the test database, ordered so that
    /// child tables (with foreign keys) are truncated before parent tables.
    /// </summary>
    private static readonly string[] _tablesInDeleteOrder =
    [
        "treatments",
        "treatment_stocks",
        "receipts_products",
        "receipts_clients",
        "receipts_suppliers",
        "products_lots",
        "beehives",
        "products",
        "receipts",
        "clients",
        "suppliers",
        "entities",
        "dose_units",
        "regions",
        "products_shapes",
        "products_categories",
        "products_types",
        "users"
    ];

    public DatabaseFixture()
    {
        ensureMariaDbRunning();
        createTestDatabase();

        // Point the application's connection infrastructure to the test database
        SMariaDbPortableService.TestConnectionStringOverride = TestConnectionString;
    }

    /// <summary>
    /// Deletes all data from every table while respecting foreign key constraints.
    /// Called between tests to ensure isolation.
    /// </summary>
    public void cleanAllTables()
    {
        using var connection = new MySqlConnection(TestConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();

        cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 0;";
        cmd.ExecuteNonQuery();

        foreach (string table in _tablesInDeleteOrder)
        {
            cmd.CommandText = $"TRUNCATE TABLE `{table}`;";
            cmd.ExecuteNonQuery();
        }

        // Re-insert default data that models depend on (FK defaults)
        cmd.CommandText = "INSERT INTO products_shapes (product_shape_name, product_shape_archive) VALUES ('', '1901-01-01');";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "INSERT INTO beehives (beehive_name) VALUES ('');";
        cmd.ExecuteNonQuery();
        cmd.CommandText = "INSERT INTO products_lots (product_lot_name, product_lot_year, fk_beehive_id) VALUES ('', 0, 1);";
        cmd.ExecuteNonQuery();

        cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1;";
        cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        // Reset the overrides so the app uses its default paths and connection string
        SMariaDbPortableService.TestConnectionStringOverride = null;
        SMariaDbPortableService.TestBaseDirectoryOverride = null;

        dropTestDatabase();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Ensures the portable MariaDB server is running before tests execute.
    /// If it is already running, this is a no-op.
    /// </summary>
    private static void ensureMariaDbRunning()
    {
        if (SMariaDbPortableService.isRunning())
            return;

        // Point SMariaDbPortableService paths to the app's output directory
        // where the portable MariaDB is actually installed.
        SMariaDbPortableService.TestBaseDirectoryOverride = findAppOutputDirectory();

        if (!SMariaDbPortableService.isInstalled())
            throw new InvalidOperationException(
                "Portable MariaDB is not installed. Run the application once to download and install it.");

        if (!SMariaDbPortableService.isDataInitialized())
            throw new InvalidOperationException(
                "Portable MariaDB data directory is not initialized. Run the application once to initialize it.");

        if (!SMariaDbPortableService.startMariaDb())
            throw new InvalidOperationException(
                "Failed to start the portable MariaDB process.");

        // Wait until the server accepts connections
        var deadline = DateTime.UtcNow.AddSeconds(MAX_STARTUP_WAIT_SECONDS);
        while (DateTime.UtcNow < deadline)
        {
            if (SMariaDbPortableService.isRunning())
                return;

            Thread.Sleep(500);
        }

        throw new TimeoutException(
            $"Portable MariaDB did not become ready within {MAX_STARTUP_WAIT_SECONDS} seconds.");
    }

    /// <summary>
    /// Locates the Mams_App output directory by searching upward from the test
    /// output directory for the <c>mariadb</c> folder in a sibling build output.
    /// </summary>
    private static string findAppOutputDirectory()
    {
        // Walk up from the test output directory to find the repo root,
        // then look for the app's mariadb folder under Mams_App build outputs.
        string? dir = AppDomain.CurrentDomain.BaseDirectory;
        while (dir != null)
        {
            string appProjectDir = Path.Combine(dir, "Mams_App");
            if (Directory.Exists(appProjectDir))
            {
                // Search for mariadb/bin/mysqld.exe under Mams_App/bin/**/
                string binDir = Path.Combine(appProjectDir, "bin");
                if (Directory.Exists(binDir))
                {
                    foreach (string candidate in Directory.GetFiles(binDir, "mysqld.exe", SearchOption.AllDirectories))
                    {
                        // mysqld.exe is at <output>/mariadb/bin/mysqld.exe — we need the <output> dir
                        string? mariaDbBin = Path.GetDirectoryName(candidate);
                        string? mariaDbRoot = mariaDbBin != null ? Path.GetDirectoryName(mariaDbBin) : null;
                        string? outputDir = mariaDbRoot != null ? Path.GetDirectoryName(mariaDbRoot) : null;
                        if (outputDir != null)
                            return outputDir;
                    }
                }
            }

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new FileNotFoundException(
            "Could not find the Mams_App output directory containing the portable MariaDB installation. " +
            "Run the application once to download and install MariaDB.");
    }

    /// <summary>
    /// Creates the test database and runs the schema from <c>init.sql</c>.
    /// </summary>
    private static void createTestDatabase()
    {
        using var connection = new MySqlConnection(_connectionStringNoDb);
        connection.Open();

        using var cmd = connection.CreateCommand();

        // Drop any stale test database (e.g. from a previous run that crashed
        // before Dispose) so the schema is always created fresh from init.sql.
        cmd.CommandText = $"DROP DATABASE IF EXISTS `{TEST_DATABASE_NAME}`;";
        cmd.ExecuteNonQuery();

        // Create the test database
        cmd.CommandText = $"CREATE DATABASE `{TEST_DATABASE_NAME}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
        cmd.ExecuteNonQuery();

        cmd.CommandText = $"USE `{TEST_DATABASE_NAME}`;";
        cmd.ExecuteNonQuery();

        // Read and execute the init.sql schema (skip the CREATE DATABASE / USE lines
        // because we already pointed to our test database)
        string initSqlPath = findInitSql();
        string initSql = File.ReadAllText(initSqlPath);
        initSql = removeDbCreationStatements(initSql);

        // Execute each statement individually
        foreach (string statement in splitStatements(initSql))
        {
            string trimmed = statement.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            cmd.CommandText = trimmed;
            cmd.ExecuteNonQuery();
        }
    }

    private static void dropTestDatabase()
    {
        try
        {
            using var connection = new MySqlConnection(_connectionStringNoDb);
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"DROP DATABASE IF EXISTS `{TEST_DATABASE_NAME}`;";
            cmd.ExecuteNonQuery();
        }
        catch
        {
            // Best-effort cleanup
        }
    }

    /// <summary>
    /// Locates the init.sql file by searching upward from the test output directory.
    /// </summary>
    private static string findInitSql()
    {
        // Walk up from the test output directory to find the repo root
        string? dir = AppDomain.CurrentDomain.BaseDirectory;
        while (dir != null)
        {
            string candidate = Path.Combine(dir, "Mams_App", "database", "init.sql");
            if (File.Exists(candidate))
                return candidate;

            dir = Directory.GetParent(dir)?.FullName;
        }

        throw new FileNotFoundException(
            "Could not find Mams_App/database/init.sql. " +
            "Make sure the portable MariaDB is running and the repository structure is intact.");
    }

    /// <summary>
    /// Removes the CREATE DATABASE / USE statements from the init script
    /// so we can run it against our test database.
    /// Handles multi-line statements (e.g. CREATE DATABASE ... CHARACTER SET ... COLLATE ...;).
    /// </summary>
    private static string removeDbCreationStatements(string sql)
    {
        var lines = sql.Split('\n');
        var filtered = new List<string>();
        bool skipping = false;

        foreach (string line in lines)
        {
            string trimmed = line.TrimStart().ToUpperInvariant();

            if (trimmed.StartsWith("CREATE DATABASE") || trimmed.StartsWith("-- DROP DATABASE"))
            {
                // Start skipping — this may span multiple lines until a semicolon
                skipping = true;
            }

            if (skipping)
            {
                // Keep skipping until we find the semicolon ending this statement
                if (line.Contains(';'))
                    skipping = false;
                continue;
            }

            if (trimmed.StartsWith("USE "))
                continue;

            filtered.Add(line);
        }

        return string.Join('\n', filtered);
    }

    /// <summary>
    /// Splits a SQL script into individual statements on semicolons.
    /// </summary>
    private static IEnumerable<string> splitStatements(string sql)
    {
        return sql.Split(';', StringSplitOptions.RemoveEmptyEntries);
    }
}
