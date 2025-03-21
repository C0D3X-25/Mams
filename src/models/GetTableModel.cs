using Mams.src.items;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Mams.src.models;

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
            MessageBox.Show($"MySQL error code: {ex.ErrorCode} - {ex.Message}");
            return null;
        }
    }

    public static ObservableCollection<T> getTableData<T>(ABaseModel model, string table) where T : ABaseItem, new() {
        ObservableCollection<T> items = new();
        DataTable? data_table = getTable(model, table);

        if (data_table != null) {
            try {
                foreach (DataRow row in data_table.Rows) {
                    T item = new();
                    foreach (DataColumn col in data_table.Columns) {
                        var value = row[col.ColumnName];
                        if (value != DBNull.Value) {
                            var property = typeof(T).GetProperty(col.ColumnName);
                            if (property != null) {
                                property.SetValue(item, Convert.ChangeType(value, property.PropertyType));
                            }
                        }
                    }
                    items.Add(item);
                }
            }
            catch (Exception ex) {
                MessageBox.Show($"Error converting data: {ex.Message}");
                return new ObservableCollection<T>();
            }
        }

        return items;
    }
}
