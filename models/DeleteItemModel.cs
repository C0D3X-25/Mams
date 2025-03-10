using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Project_Mams.src.interfaces;

namespace Project_Mams.src.models;

public static class DeleteItemModel {

    // Static method
    public static bool deleteItem(ABaseModel model, string id, string table) {
        using MySqlConnection? conn = model._m_conn.openConnection();

        try {
            using MySqlCommand cmd =
                new($"DELETE FROM {table} WHERE id = @id", conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return true;
        }
        catch (MySqlException ex) {
            MessageBox.Show($"MySQL error code: {ex.Code} - {ex.Message}");
            return false;
        }
    }


    // Extension method
    //public static bool deleteItemByID(this ABaseModel model, string id, string table) {
    //    using MySqlConnection? conn = model._m_conn.openConnection();

    //    try {
    //        using MySqlCommand cmd =
    //            new($"DELETE FROM {table} WHERE id = @id", conn);
    //        cmd.Parameters.AddWithValue("@id", id);
    //        cmd.ExecuteNonQuery();
    //        return true;
    //    }
    //    catch (MySqlException ex) {
    //        MessageBox.Show($"MySQL error code: {ex.Code} - {ex.Message}");
    //        return false;
    //    }
    //}
}
