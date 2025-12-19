using Mams.src.errors;
using MySqlConnector;

namespace Mams.src.databaseOperations;

/// <summary>
/// Represents the response from a save operation, containing the returned ID and any error.
/// </summary>
/// <remarks>
/// This class provides a structured way to handle the result of save operations,
/// including success status determination and error information from MySQL operations.
/// </remarks>
public class ResponseSaveItem {

    /// <summary>
    /// Gets or sets the unique identifier returned from the save operation.
    /// </summary>
    /// <remarks>
    /// A value of 0 typically indicates that the operation failed or no ID was generated.
    /// For insert operations, this contains the newly generated ID.
    /// For update operations, this contains the ID of the updated item.
    /// </remarks>
    public int returned_id { get; set; } = 0;

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
    /// Gets a value indicating whether the save operation was successful.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if <see cref="returned_id"/> is greater than 0 
    /// and <see cref="error"/> is <see cref="EErrors.NONE"/>; otherwise, <see langword="false"/>.
    /// </remarks>
    public bool is_success => returned_id > 0 && error == EErrors.NONE;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseSaveItem"/> class with default values.
    /// </summary>
    public ResponseSaveItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseSaveItem"/> class with the specified ID.
    /// </summary>
    /// <param name="id">The returned ID from the save operation.</param>
    public ResponseSaveItem(int id) {
        returned_id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseSaveItem"/> class with the specified ID and error.
    /// </summary>
    /// <param name="id">The returned ID from the save operation.</param>
    /// <param name="error">The error type from the save operation.</param>
    public ResponseSaveItem(int id, EErrors error) {
        returned_id = id;
        this.error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseSaveItem"/> class with the specified ID, error, and detail message.
    /// </summary>
    /// <param name="id">The returned ID from the save operation.</param>
    /// <param name="error">The error type from the save operation.</param>
    /// <param name="errorMessageDetail">The detailed error message for developers.</param>
    public ResponseSaveItem(int id, EErrors error, string errorMessageDetail) {
        returned_id = id;
        this.error = error;
        error_message_detail = errorMessageDetail;
    }

    /// <summary>
    /// Creates a successful response with the specified ID.
    /// </summary>
    /// <param name="id">The returned ID from the save operation.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> indicating success.</returns>
    public static ResponseSaveItem Success(int id) => new(id);

    /// <summary>
    /// Creates a failed response with the specified error type.
    /// </summary>
    /// <param name="error">The error type describing the failure.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> indicating failure.</returns>
    public static ResponseSaveItem Failure(EErrors error) => new(0, error);

    /// <summary>
    /// Creates a failed response with the specified error type and detail message.
    /// </summary>
    /// <param name="error">The error type describing the failure.</param>
    /// <param name="errorMessageDetail">The detailed error message for developers.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> indicating failure.</returns>
    public static ResponseSaveItem Failure(EErrors error, string errorMessageDetail) 
        => new(0, error, errorMessageDetail);

    /// <summary>
    /// Creates a failed response with the specified MySQL error code and message.
    /// </summary>
    /// <param name="errorCode">The MySQL error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> indicating failure with formatted MySQL error details.</returns>
    public static ResponseSaveItem MySqlFailure(MySqlErrorCode errorCode, string message) 
        => new(0, EErrors.DATABASE_QUERY, $"MySQL Error [{errorCode}]: {message}");
}
