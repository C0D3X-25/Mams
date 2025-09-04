using MySqlConnector;
using System.Windows;

namespace Mams.src.databaseConnections;

public class SQLConnectionModel {

    // Very safe credentials !!
    private const string _m_SERVER = "localhost";
    private const string _m_USER = "root";
    private const string _m_PASSWORD = "root";
    private const string _m_DB = "mams_db";
    private const string _m_connection_string = 
        $"server={_m_SERVER};" +
        $"uid={_m_USER};" +
        $"pwd={_m_PASSWORD};" +
        $"database={_m_DB};" +
        $"pooling=true;" +
        $"min pool size=5;" +
        $"max pool size=50;";

    public MySqlConnection GetConnection() {
        var connection = new MySqlConnection(_m_connection_string);
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
    /// the MySQL service (Not implemented).</description></item> <item><description>Any other value: Exits the
    /// application.</description></item> </list></remarks>
    /// <param name="connection">The <see cref="MySqlConnection"/> instance to check. Can be <see langword="null"/>.</param>
    /// <param name="cmd">Specifies the action to take if the connection is not open.  The default value is <see
    /// cref="EDatabaseConnection.EXIT"/>.</param>
    /// <returns><see langword="true"/> if the connection is open; otherwise, <see langword="false"/>.</returns>
    public bool isConnectionOpen(MySqlConnection? connection, EDatabaseConnection cmd = EDatabaseConnection.EXIT) {
        if (connection != null 
            && connection.State == System.Data.ConnectionState.Open
            ) {
            return true;
        }

        MessageBox.Show("Failed to connect to the database.");

        switch (cmd) {
            case EDatabaseConnection.EXIT:
                connection?.Close();
                Environment.Exit(1);
                break;
            case EDatabaseConnection.CONTINUE:
                // Do nothing
                break;
            case EDatabaseConnection.RECONNECT:
                stopMysqlService();
                startMysqlService();
                break;
            default:
                Environment.Exit(1);
                break;
        }
        return false;
    }

    // TODO:
    public void startMysqlService() {
        //try {
        //    // First, try with the most common MySQL 8.0 service name
        //    string serviceName = "MySQL80";

        //    // Check if we can find the service
        //    System.ServiceProcess.ServiceController[] services = System.ServiceProcess.ServiceController.GetServices();
        //    bool serviceFound = services.Any(s => s.ServiceName == serviceName);

        //    // If not found by that name, try to find any MySQL service
        //    if (!serviceFound) {
        //        var mysqlService = services.FirstOrDefault(s =>
        //            s.ServiceName.Contains("MySQL") ||
        //            s.ServiceName.Contains("mysql"));

        //        if (mysqlService != null) {
        //            serviceName = mysqlService.ServiceName;
        //            serviceFound = true;
        //        }
        //    }

        //    if (serviceFound) {
        //        using System.ServiceProcess.ServiceController sc = new(serviceName);
        //        if (sc.Status != System.ServiceProcess.ServiceControllerStatus.Running) {
        //            sc.Start();
        //            sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
        //        }
        //    }
        //    else {
        //        // Ask user if they want to install the MySQL service
        //        MessageBoxResult result = MessageBox.Show(
        //            "MySQL service not found. Would you like to install MySQL as a Windows service?",
        //            "MySQL Service Not Found",
        //            MessageBoxButton.YesNo,
        //            MessageBoxImage.Question
        //        );

        //        if (result == MessageBoxResult.Yes) {
        //            // User chose to install the service
        //            bool installSuccessful = installMySQLService();
        //            if (installSuccessful) {
        //                MessageBox.Show("MySQL service was successfully installed and started.");
        //                // You might want to retry starting the service here
        //                startMysqlService(); // Recursive call to try again now that it's installed
        //            }
        //        }
        //        else {
        //            // User chose not to install
        //            MessageBox.Show("Application requires MySQL service to function properly.");
        //        }
        //    }
        //}
        //catch (System.ComponentModel.Win32Exception ex) when (ex.NativeErrorCode == 5) {
        //    // Error code 5 is access denied
        //    MessageBox.Show("Access denied when trying to control MySQL service. Please run as administrator.");
        //}
        //catch (Exception ex) {
        //    MessageBox.Show($"Failed to start MySQL service: {ex.Message}");
        //}
    }


    public void stopMysqlService() {
        //try {
        //    using System.ServiceProcess.ServiceController sc = new("MySQL80"); // or your MySQL service name
        //    if (sc.Status != System.ServiceProcess.ServiceControllerStatus.Stopped) {
        //        sc.Stop();
        //        sc.WaitForStatus(System.ServiceProcess.ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
        //    }
        //}
        //catch (Exception ex) {
        //    MessageBox.Show($"Failed to stop MySQL service: {ex.Message}");
        //}
    }


    private bool installMySQLService() {
        //try {
        //    // Path to your MySQL installation
        //    string mysqlPath = @"C:\Program Files\MySQL\MySQL Server 8.0\bin\mysqld.exe";
        //    string configPath = @"C:\ProgramData\MySQL\MySQL Server 8.0\my.ini";
        //    string serviceName = "MySQL80";

        //    // Check if the executable exists
        //    if (!File.Exists(mysqlPath)) {
        //        MessageBox.Show("MySQL executable not found at: " + mysqlPath);
        //        return false;
        //    }

        //    // Command to install the service
        //    ProcessStartInfo startInfo = new() {
        //        FileName = mysqlPath,
        //        Arguments = $"--install \"{serviceName}\" --defaults-file=\"{configPath}\string.Empty,
        //        UseShellExecute = true,
        //        Verb = "runas", // Request admin privileges
        //        CreateNoWindow = false
        //    };

        //    using (Process? process = Process.Start(startInfo)) {
        //        process?.WaitForExit();
        //        if (process?.ExitCode != 0) {
        //            MessageBox.Show($"Failed to install MySQL service. Exit code: {process?.ExitCode}");
        //            return false;
        //        }
        //    }

        //    // Now start the service
        //    using ServiceController sc = new(serviceName);
        //    sc.Start();
        //    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));

        //    return sc.Status == ServiceControllerStatus.Running;
        //}
        //catch (Exception ex) {
        //    MessageBox.Show($"Failed to install MySQL service: {ex.Message}");
        return false;
        //}
    }
}

