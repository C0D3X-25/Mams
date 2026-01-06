using Mams_App.src.services;
using MySqlConnector;
using System.Windows;

namespace Mams_App.src.databaseConnections;

public class SQLConnectionModel {

    /// <summary>
    /// Gets the connection string from the portable MariaDB service.
    /// </summary>
    private static string ConnectionString => SMariaDbPortableService.ConnectionString;

    public static MySqlConnection GetConnection() {
        var connection = new MySqlConnection(ConnectionString);
        try {
            connection.Open();
            return connection;
        }
        catch (MySqlException e) {
            MessageBox.Show("Failed to connect to the database.\n" +
                $"Code:   {e.ErrorCode}\n" +
                $"Number: {e.Number}\n" +
                $"Data:   {e.Data}\n" +
                $"State:  {e.SqlState}\n" +
                $"Source: {e.Source}"
            );
            connection.Dispose();
            Environment.Exit(1);
            return null;
        }
    }

    /// <summary>
    /// Determines whether the specified MySQL database connection is open.
    /// </summary>
    /// <remarks>If the connection is not open, a message box is displayed indicating the failure, and the
    /// behavior  depends on the value of <paramref name="cmd"/>: <list type="bullet"> <item><description><see
    /// cref="EDatabaseConnection.EXIT"/>: Closes the connection (if not <see langword="null"/>) and exits the
    /// application.</description></item> <item><description><see cref="EDatabaseConnection.CONTINUE"/>: No action is
    /// taken.</description></item> <item><description><see cref="EDatabaseConnection.RECONNECT"/>: Attempts to restart
    /// the MariaDB server.</description></item> <item><description>Any other value: Exits the
    /// application.</description></item> </list></remarks>
    /// <param name="connection">The <see cref="MySqlConnection"/> instance to check. Can be <see langword="null"/>.</param>
    /// <param name="cmd">Specifies the action to take if the connection is not open.  The default value is <see
    /// cref="EDatabaseConnection.EXIT"/>.</param>
    /// <returns><see langword="true"/> if the connection is open; otherwise, <see langword="false"/>.</returns>
    public static bool isConnectionOpen(MySqlConnection? connection, EDatabaseConnection cmd = EDatabaseConnection.EXIT)
    {
        if (connection != null 
            && connection.State == System.Data.ConnectionState.Open
            ) {
            return true;
        }

        MessageBox.Show("Failed to connect to the database.");

        switch (cmd)
        {
            case EDatabaseConnection.EXIT:
                connection?.Close();
                Environment.Exit(1);
                break;
            case EDatabaseConnection.CONTINUE:
                // Do nothing
                break;
            case EDatabaseConnection.RECONNECT:
                SMariaDbPortableService.stopMariaDb();
                SMariaDbPortableService.startMariaDb();
                break;
            default:
                Environment.Exit(1);
                break;
        }
        return false;
    }
}

