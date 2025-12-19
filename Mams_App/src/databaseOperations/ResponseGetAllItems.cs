using Mams.src.errors;
using Mams.src.items;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Mams.src.databaseOperations;

/// <summary>
/// Represents the response from a get all items operation, containing the retrieved collection and any error.
/// </summary>
/// <remarks>
/// This class provides a structured way to handle the result of get all operations,
/// including success status determination and error information from MySQL operations.
/// </remarks>
/// <typeparam name="T">The type of items retrieved. Must inherit from <see cref="ABaseItem"/>.</typeparam>
public class ResponseGetAllItems<T> where T : ABaseItem {

    /// <summary>
    /// Gets or sets the collection of items retrieved from the get all operation.
    /// </summary>
    /// <remarks>
    /// This property is an empty collection when the operation fails or no items are found.
    /// </remarks>
    public ObservableCollection<T> returned_items { get; set; } = [];

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
    /// Gets a value indicating whether the get all operation was successful.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if <see cref="error"/> is <see cref="EErrors.NONE"/>; otherwise, <see langword="false"/>.
    /// </remarks>
    public bool is_success => error == EErrors.NONE;

    /// <summary>
    /// Gets a value indicating whether any items were found.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if <see cref="returned_items"/> contains at least one item;
    /// otherwise, <see langword="false"/>.
    /// </remarks>
    public bool has_items => returned_items.Count > 0;

    /// <summary>
    /// Gets the count of items in the returned collection.
    /// </summary>
    public int count => returned_items.Count;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseGetAllItems{T}"/> class with default values.
    /// </summary>
    public ResponseGetAllItems() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseGetAllItems{T}"/> class with the specified collection.
    /// </summary>
    /// <param name="items">The collection of items retrieved from the get all operation.</param>
    public ResponseGetAllItems(ObservableCollection<T> items) {
        returned_items = items;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseGetAllItems{T}"/> class with the specified collection and error.
    /// </summary>
    /// <param name="items">The collection of items retrieved from the get all operation.</param>
    /// <param name="error">The error type from the get all operation.</param>
    public ResponseGetAllItems(ObservableCollection<T> items, EErrors error) {
        returned_items = items;
        this.error = error;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseGetAllItems{T}"/> class with the specified collection, error, and detail message.
    /// </summary>
    /// <param name="items">The collection of items retrieved from the get all operation.</param>
    /// <param name="error">The error type from the get all operation.</param>
    /// <param name="errorMessageDetail">The detailed error message for developers.</param>
    public ResponseGetAllItems(ObservableCollection<T> items, EErrors error, string errorMessageDetail) {
        returned_items = items;
        this.error = error;
        error_message_detail = errorMessageDetail;
    }

    /// <summary>
    /// Creates a successful response with the specified collection.
    /// </summary>
    /// <param name="items">The collection of items retrieved from the get all operation.</param>
    /// <returns>A <see cref="ResponseGetAllItems{T}"/> indicating success.</returns>
    public static ResponseGetAllItems<T> Success(ObservableCollection<T> items) => new(items);

    /// <summary>
    /// Creates a response indicating no items were found.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{T}"/> with an empty collection.</returns>
    public static ResponseGetAllItems<T> Empty() => new([]);

    /// <summary>
    /// Creates a failed response with the specified error type.
    /// </summary>
    /// <param name="error">The error type describing the failure.</param>
    /// <returns>A <see cref="ResponseGetAllItems{T}"/> indicating failure.</returns>
    public static ResponseGetAllItems<T> Failure(EErrors error) => new([], error);

    /// <summary>
    /// Creates a failed response with the specified error type and detail message.
    /// </summary>
    /// <param name="error">The error type describing the failure.</param>
    /// <param name="errorMessageDetail">The detailed error message for developers.</param>
    /// <returns>A <see cref="ResponseGetAllItems{T}"/> indicating failure.</returns>
    public static ResponseGetAllItems<T> Failure(EErrors error, string errorMessageDetail) 
        => new([], error, errorMessageDetail);

    /// <summary>
    /// Creates a failed response with the specified MySQL error code and message.
    /// </summary>
    /// <param name="errorCode">The MySQL error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="ResponseGetAllItems{T}"/> indicating failure with formatted MySQL error details.</returns>
    public static ResponseGetAllItems<T> MySqlFailure(MySqlErrorCode errorCode, string message) 
        => new([], EErrors.DATABASE_QUERY, $"MySQL Error [{errorCode}]: {message}");
}
