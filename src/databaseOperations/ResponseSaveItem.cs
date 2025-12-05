using MySqlConnector;

namespace Mams.src.databaseOperations;

/// <summary>
/// Represents the response from a save operation, containing the returned ID and any error message.
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
    /// Gets or sets the MySQL error message if the operation failed.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> or empty when the operation succeeds.
    /// When an error occurs, it contains the formatted error message including the MySQL error code.
    /// </remarks>
    public string? error_message { get; set; } = null;

    /// <summary>
    /// Gets a value indicating whether the save operation was successful.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if <see cref="returned_id"/> is greater than 0 
    /// and <see cref="error_message"/> is null or empty; otherwise, <see langword="false"/>.
    /// </remarks>
    public bool is_success => returned_id > 0 && string.IsNullOrEmpty(error_message);

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
    /// Initializes a new instance of the <see cref="ResponseSaveItem"/> class with the specified ID and error message.
    /// </summary>
    /// <param name="id">The returned ID from the save operation.</param>
    /// <param name="errorMessage">The error message from the save operation.</param>
    public ResponseSaveItem(int id, string? errorMessage) {
        returned_id = id;
        error_message = errorMessage;
    }

    /// <summary>
    /// Creates a successful response with the specified ID.
    /// </summary>
    /// <param name="id">The returned ID from the save operation.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> indicating success.</returns>
    public static ResponseSaveItem Success(int id) => new(id);

    /// <summary>
    /// Creates a failed response with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> indicating failure.</returns>
    public static ResponseSaveItem Failure(string errorMessage) => new(0, errorMessage);

    /// <summary>
    /// Creates a failed response with the specified MySQL error code and message.
    /// </summary>
    /// <param name="errorCode">The MySQL error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> indicating failure with formatted MySQL error details.</returns>
    public static ResponseSaveItem MySqlFailure(MySqlErrorCode errorCode, string message) 
        => new(0, $"MySQL error code: {errorCode} - {message}");
}
