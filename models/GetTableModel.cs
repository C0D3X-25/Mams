using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Mams.src.models;

public static class GetTableModel {

    /// <summary>
    /// Get all data from a table.
    /// </summary>
    /// <param name="table">The name of the table</param>
    /// <returns></returns>
    public static DataTable? getTable(ABaseModel model, string table) {

        using MySqlConnection? conn = model._m_conn.openConnection();

        DataTable data_table = new();

        try {
            using MySqlCommand cmd = new($"SELECT * FROM {table}", conn);
            using MySqlDataReader reader = cmd.ExecuteReader();
            data_table.Load(reader);

            return data_table;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.Code} - {ex.Message}");
            return null;
        }
    }
}
