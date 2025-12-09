using Mams.src.items;
using MySqlConnector;
using System.Collections.ObjectModel;

namespace Mams.src.databaseOperations;

/// <summary>
/// Represents the response from a get all items operation, containing the retrieved collection and any error message.
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
    /// Gets or sets the MySQL error message if the operation failed.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null"/> or empty when the operation succeeds.
    /// When an error occurs, it contains the formatted error message including the MySQL error code.
    /// </remarks>
    public string? error_message { get; set; } = null;

    /// <summary>
    /// Gets a value indicating whether the get all operation was successful.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="true"/> if <see cref="error_message"/> is null or empty; otherwise, <see langword="false"/>.
    /// </remarks>
    public bool is_success => string.IsNullOrEmpty(error_message);

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
    /// Initializes a new instance of the <see cref="ResponseGetAllItems{T}"/> class with the specified collection and error message.
    /// </summary>
    /// <param name="items">The collection of items retrieved from the get all operation.</param>
    /// <param name="errorMessage">The error message from the get all operation.</param>
    public ResponseGetAllItems(ObservableCollection<T> items, string? errorMessage) {
        returned_items = items;
        error_message = errorMessage;
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
    /// Creates a failed response with the specified error message.
    /// </summary>
    /// <param name="errorMessage">The error message describing the failure.</param>
    /// <returns>A <see cref="ResponseGetAllItems{T}"/> indicating failure.</returns>
    public static ResponseGetAllItems<T> Failure(string errorMessage) => new([], errorMessage);

    /// <summary>
    /// Creates a failed response with the specified MySQL error code and message.
    /// </summary>
    /// <param name="errorCode">The MySQL error code.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A <see cref="ResponseGetAllItems{T}"/> indicating failure with formatted MySQL error details.</returns>
    public static ResponseGetAllItems<T> MySqlFailure(MySqlErrorCode errorCode, string message) 
        => new([], $"MySQL error code: {errorCode} - {message}");
}
