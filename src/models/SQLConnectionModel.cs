using Mams.src.enums;
using MySqlConnector;
using System.Windows;

namespace Mams.src.models;


public class SQLConnectionModel {
    private const string _m_SERVER = "localhost";
    private const string _m_USER = "root";
    private const string _m_PASSWORD = "";
    private const string _m_DB = "test";

    public MySqlConnection? openConnection() {
        string conn_string =
            $"server={_m_SERVER};" +
            $"uid={_m_USER};" +
            $"pwd={_m_PASSWORD};" +
            $"database={_m_DB};";

        MySqlConnection? connection = null;

        try {
            connection = new(conn_string);
            connection.Open();

            if (!isConnectionOpen(connection)) {
                connection?.Close();
                return null;
            }

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

            connection?.Close();
            Environment.Exit(1);
            return null;
        }
    }

    public bool isConnectionOpen(MySqlConnection? connection, EDatabaseConnection cmd = EDatabaseConnection.EXIT) {
        if (connection != null && connection.State == System.Data.ConnectionState.Open) {
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
                // TODO: try to reconnect to DB
                break;
            default:
                Environment.Exit(1);
                break;
        }

        return false;
    }
}

