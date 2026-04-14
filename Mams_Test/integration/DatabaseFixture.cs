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

        cmd.CommandText = "SET FOREIGN_KEY_CHECKS = 1;";
        cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
        // Reset the override so the app uses its default connection string
        SMariaDbPortableService.TestConnectionStringOverride = null;

        dropTestDatabase();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Creates the test database and runs the schema from <c>init.sql</c>.
    /// </summary>
    private static void createTestDatabase()
    {
        using var connection = new MySqlConnection(_connectionStringNoDb);
        connection.Open();

        using var cmd = connection.CreateCommand();

        // Create the test database
        cmd.CommandText = $"CREATE DATABASE IF NOT EXISTS `{TEST_DATABASE_NAME}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;";
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
    /// </summary>
    private static string removeDbCreationStatements(string sql)
    {
        var lines = sql.Split('\n');
        var filtered = lines.Where(line =>
        {
            string trimmed = line.TrimStart().ToUpperInvariant();
            return !trimmed.StartsWith("CREATE DATABASE")
                && !trimmed.StartsWith("USE ")
                && !trimmed.StartsWith("-- DROP DATABASE");
        });
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
