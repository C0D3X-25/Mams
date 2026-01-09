using Mams_App.src.errors;
using MySqlConnector;

namespace Mams_App.src.databaseOperations;

/// <summary>
/// Represents the response from a delete operation, containing success status and any error.
/// </summary>
/// <remarks>
/// This class provides a structured way to handle the result of delete operations,
/// including success status determination and error information from MySQL operations.
/// </remarks>
public class ResponseDeleteItem
{

    /// <summary>
    /// Gets or sets a value indicating whether the delete operation was successful.
    /// </summary>
    /// <remarks>
    /// A value of <see langword="true"/> indicates the item was successfully deleted or archived.
    /// A value of <see langword="false"/> indicates the operation failed.
    /// </remarks>
    public bool is_deleted { get; set; } = false;

    /// <summary>
    /// Gets or sets the error type if the operation failed.
    /// </summary>
    /// <remarks>
    /// This property is <see cref="EErrors.NONE"/> when the operation succeeds.
    /// When an error occurs, it contains the appropriate error type.
    /// </remarks>
    public EErrors error { get; set; } = EErrors.NONE;

    /// <summary>
    /// Gets or sets the detailed error message for developers or advanced users.
    /// </summary>
    /// <remarks>
    /// This property contains technical information about the error, such as MySQL error codes,
    /// exception details, or the class/method where the error occurred.
    /// This message is intended for debugging purposes and should be in English.
    /// </remarks>
    public string? error_message_detail { get; set; } = null;

    /// <summary>
    /// Gets a value indicating whether the delete operation was successful.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if <see cref="is_deleted"/> is <see langword="true"/> 
    /// and <see cref="error"/> is <see cref="EErrors.NONE"/>; otherwise, <see langword="false"/>.
    /// </remarks>
    public bool is_success => is_deleted && error == EErrors.NONE;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseDeleteItem"/> class with default values.
    /// </summary>
    public ResponseDeleteItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseDeleteItem"/> class with the specified success status.
    /// </summary>
    /// <param name="deleted">A value indicating whether the delete operation was successful.</param>
    public ResponseDeleteItem(bool deleted)
    {
        is_deleted = deleted;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseDeleteItem"/> class with the specified success status and error.
    /// </summary>
    /// <param name="deleted">A value indicating whether the delete operation was successful.</param>
    /// <param name="error">The error type from the delete operation.</param>
    public ResponseDeleteItem(bool deleted, EErrors error)
    {
        is_deleted = deleted;
        this.error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseDeleteItem"/> class with the specified success status, error, and detail message.
    /// </summary>
    /// <param name="deleted">A value indicating whether the delete operation was successful.</param>
    /// <param name="error">The error type from the delete operation.</param>
    /// <param name="errorMessageDetail">The detailed error message for developers.</param>
    public ResponseDeleteItem(bool deleted, EErrors error, string errorMessageDetail)
    {
        is_deleted = deleted;
        this.error = error;
        error_message_detail = errorMessageDetail;
    }

    /// <summary>
    /// Creates a successful response indicating the item was deleted.
    /// </summary>
    /// <returns>A <see cref="ResponseDeleteItem"/> indicating success.</returns>
    public static ResponseDeleteItem Success() => new(true);

    /// <summary>
    /// Creates a failed response with the specified error type.
    /// </summary>
    /// <param name="error">The error type describing the failure.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> indicating failure.</returns>
    public static ResponseDeleteItem Failure(EErrors error) => new(false, error);

    /// <summary>
    /// Creates a failed response with the specified error type and detail message.
    /// </summary>
    /// <param name="error">The error type describing the failure.</param>
    /// <param name="errorMessageDetail">The detailed error message for developers.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> indicating failure.</returns>
    public static ResponseDeleteItem Failure(EErrors error, string errorMessageDetail)
        => new(false, error, errorMessageDetail);

    /// <summary>
    /// Creates a failed response with the specified MySQL error code and message.
    /// </summary>
    /// <param name="errorCode">The MySQL error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> indicating failure with formatted MySQL error details.</returns>
    public static ResponseDeleteItem MySqlFailure(MySqlErrorCode errorCode, string message)
        => new(false, EErrors.DATABASE_QUERY, $"MySQL Error [{errorCode}]: {message}");
}
