using Mams_App.src.databaseOperations;
using Mams_App.src.errors;
using Mams_App.src.helpers;
using Mams_App.src.models;
using MySqlConnector;

namespace Mams_App.src.users;

/// <summary>
/// Represents a model for managing the user (app owner) in the database.
/// This is used for invoice generation.
/// </summary>
public class UserModel : ABaseModel
{
    private const string _m_TBL_NAME = "users";
    private const string _m_COL_ID = "user_id";
    private const string _m_COL_NAME = "user_name";
    private const string _m_COL_PHONE = "user_phone";
    private const string _m_COL_EMAIL = "user_email";
    private const string _m_COL_CITY = "user_city";
    private const string _m_COL_ADDRESS = "user_address";

    /// <summary>
    /// Retrieves the user (app owner) from the database.
    /// There should only be one user in the database.
    /// </summary>
    /// <returns>A <see cref="ResponseGetItem{UserItem}"/> containing the user item and any error message.</returns>
    public ResponseGetItem<UserItem> getUser()
    {
        return executeWithConnection(connection =>
        {
            try
            {
                using MySqlCommand cmd = new(
                    $"SELECT {_m_COL_ID}, {_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS} " +
                    $"FROM {_m_TBL_NAME} " +
                    $"WHERE {_m_COL_ID} = 1 ",
                    connection,
                    m_transaction
                );

                using MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    return ResponseGetItem<UserItem>.Success(new UserItem
                    {
                        user_id = reader.getSafeValue<int>(_m_COL_ID),
                        user_name = reader.getSafeValue(_m_COL_NAME, string.Empty),
                        user_phone = reader.getSafeValue(_m_COL_PHONE, string.Empty),
                        user_email = reader.getSafeValue(_m_COL_EMAIL, string.Empty),
                        user_city = reader.getSafeValue(_m_COL_CITY, string.Empty),
                        user_address = reader.getSafeValue(_m_COL_ADDRESS, string.Empty)
                    });
                }
                return ResponseGetItem<UserItem>.NotFound();
            }
            catch (MySqlException ex)
            {
                return ResponseGetItem<UserItem>.MySqlFailure(ex.ErrorCode, ex.Message);
            }
        });
    }

    /// <summary>
    /// Saves the user (app owner) to the database.
    /// If the user exists (id=1), it updates the record; otherwise, it inserts a new one.
    /// </summary>
    /// <param name="item">The <see cref="UserItem"/> to save. Cannot be <c>null</c>.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved item and any error message.</returns>
    public ResponseSaveItem saveUser(UserItem item)
    {
        if (item == null)
        {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE,
                "UserModel.saveUser: Item cannot be null");
        }

        // Check if user already exists
        var existingUser = getUser();
        bool isInsert = !existingUser.is_found;

        string query;

        if (isInsert)
        {
            query = $"INSERT INTO {_m_TBL_NAME} ({_m_COL_ID}, {_m_COL_NAME}, {_m_COL_PHONE}, {_m_COL_EMAIL}, {_m_COL_CITY}, {_m_COL_ADDRESS}) " +
                $"VALUES (1, @name, @phone, @email, @city, @address); ";
        }
        else
        {
            query = $"UPDATE {_m_TBL_NAME} " +
                $"SET {_m_COL_NAME} = @name, {_m_COL_PHONE} = @phone, {_m_COL_EMAIL} = @email, " +
                $"{_m_COL_CITY} = @city, {_m_COL_ADDRESS} = @address " +
                $"WHERE {_m_COL_ID} = 1";
        }

        // Start transaction if needed
        bool need_transaction = !isTransactionActive();
        if (need_transaction)
        {
            startTransaction();
        }

        try
        {
            executeWithConnection(connection =>
            {
                using MySqlCommand cmd = new(query, connection, m_transaction);
                cmd.Parameters.AddWithValue("@name", item.user_name.Trim());
                cmd.Parameters.AddWithValue("@phone", item.user_phone.Trim());
                cmd.Parameters.AddWithValue("@email", item.user_email.Trim());
                cmd.Parameters.AddWithValue("@city", item.user_city.Trim());
                cmd.Parameters.AddWithValue("@address", item.user_address.Trim());
                cmd.ExecuteNonQuery();
            });

            if (need_transaction)
            {
                commitTransaction();
            }

            return ResponseSaveItem.Success(1);
        }
        catch (MySqlException ex)
        {
            if (need_transaction)
            {
                rollbackTransaction();
            }
            return ResponseSaveItem.MySqlFailure(ex.ErrorCode, ex.Message);
        }
    }
}
