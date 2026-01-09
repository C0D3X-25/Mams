using Mams_App.src.errors;
using Mams_App.src.items;
using MySqlConnector;

namespace Mams_App.src.databaseOperations;

/// <summary>
/// Represents the response from a get operation, containing the retrieved item and any error.
/// </summary>
/// <remarks>
/// This class provides a structured way to handle the result of get operations,
/// including success status determination and error information from MySQL operations.
/// </remarks>
/// <typeparam name="T">The type of item retrieved. Must inherit from <see cref="ABaseItem"/>.</typeparam>
public class ResponseGetItem<T> where T : ABaseItem
{

    /// <summary>
    /// Gets or sets the item retrieved from the get operation.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> when the operation fails or no item is found.
    /// </remarks>
    public T? returned_item { get; set; } = default;

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
    /// Gets a value indicating whether the get operation was successful.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if <see cref="returned_item"/> is not <see langword="null"/> 
    /// and <see cref="error"/> is <see cref="EErrors.NONE"/>; otherwise, <see langword="false"/>.
    /// </remarks>
    public bool is_success => returned_item != null && error == EErrors.NONE;

    /// <summary>
    /// Gets a value indicating whether the item was found.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if <see cref="returned_item"/> is not <see langword="null"/>;
    /// otherwise, <see langword="false"/>.
    /// </remarks>
    public bool is_found => returned_item != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseGetItem{T}"/> class with default values.
    /// </summary>
    public ResponseGetItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseGetItem{T}"/> class with the specified item.
    /// </summary>
    /// <param name="item">The item retrieved from the get operation.</param>
    public ResponseGetItem(T? item)
    {
        returned_item = item;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseGetItem{T}"/> class with the specified item and error.
    /// </summary>
    /// <param name="item">The item retrieved from the get operation.</param>
    /// <param name="error">The error type from the get operation.</param>
    public ResponseGetItem(T? item, EErrors error)
    {
        returned_item = item;
        this.error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseGetItem{T}"/> class with the specified item, error, and detail message.
    /// </summary>
    /// <param name="item">The item retrieved from the get operation.</param>
    /// <param name="error">The error type from the get operation.</param>
    /// <param name="errorMessageDetail">The detailed error message for developers.</param>
    public ResponseGetItem(T? item, EErrors error, string errorMessageDetail)
    {
        returned_item = item;
        this.error = error;
        error_message_detail = errorMessageDetail;
    }

    /// <summary>
    /// Creates a successful response with the specified item.
    /// </summary>
    /// <param name="item">The item retrieved from the get operation.</param>
    /// <returns>A <see cref="ResponseGetItem{T}"/> indicating success.</returns>
    public static ResponseGetItem<T> Success(T item) => new(item);

    /// <summary>
    /// Creates a response indicating the item was not found.
    /// </summary>
    /// <returns>A <see cref="ResponseGetItem{T}"/> indicating the item was not found.</returns>
    public static ResponseGetItem<T> NotFound() => new(default);

    /// <summary>
    /// Creates a failed response with the specified error type.
    /// </summary>
    /// <param name="error">The error type describing the failure.</param>
    /// <returns>A <see cref="ResponseGetItem{T}"/> indicating failure.</returns>
    public static ResponseGetItem<T> Failure(EErrors error) => new(default, error);

    /// <summary>
    /// Creates a failed response with the specified error type and detail message.
    /// </summary>
    /// <param name="error">The error type describing the failure.</param>
    /// <param name="errorMessageDetail">The detailed error message for developers.</param>
    /// <returns>A <see cref="ResponseGetItem{T}"/> indicating failure.</returns>
    public static ResponseGetItem<T> Failure(EErrors error, string errorMessageDetail)
        => new(default, error, errorMessageDetail);

    /// <summary>
    /// Creates a failed response with the specified MySQL error code and message.
    /// </summary>
    /// <param name="errorCode">The MySQL error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="ResponseGetItem{T}"/> indicating failure with formatted MySQL error details.</returns>
    public static ResponseGetItem<T> MySqlFailure(MySqlErrorCode errorCode, string message)
        => new(default, EErrors.DATABASE_QUERY, $"MySQL Error [{errorCode}]: {message}");
}
