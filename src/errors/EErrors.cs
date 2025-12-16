namespace Mams.src.errors;

/// <summary>
/// Represents generic error types for operation responses.
/// </summary>
/// <remarks>
/// This enum provides a clean and standardized way to handle errors across the application.
/// The specific error messages should be generated when displaying to the user based on the context.
/// </remarks>
public enum EErrors {
    /// <summary>
    /// No error occurred. The operation was successful.
    /// </summary>
    NONE = 0,

    /// <summary>
    /// A general or unspecified error occurred.
    /// </summary>
    UNKNOWN,

    /// <summary>
    /// The provided input data is invalid or malformed.
    /// </summary>
    INVALID_INPUT,

    /// <summary>
    /// A required parameter or value was not provided.
    /// </summary>
    MISSING_PARAMETER,

    /// <summary>
    /// The requested item was not found.
    /// </summary>
    NOT_FOUND,

    /// <summary>
    /// The item already exists (duplicate).
    /// </summary>
    ALREADY_EXISTS,

    /// <summary>
    /// A null value was provided where it is not allowed.
    /// </summary>
    NULL_VALUE,

    /// <summary>
    /// A database connection error occurred.
    /// </summary>
    DATABASE_CONNECTION,

    /// <summary>
    /// A database query execution error occurred.
    /// </summary>
    DATABASE_QUERY,

    /// <summary>
    /// A foreign key constraint violation occurred.
    /// </summary>
    FOREIGN_KEY_VIOLATION,

    /// <summary>
    /// A unique constraint violation occurred.
    /// </summary>
    UNIQUE_CONSTRAINT_VIOLATION,

    /// <summary>
    /// The operation is not authorized or permission is denied.
    /// </summary>
    UNAUTHORIZED,

    /// <summary>
    /// The operation timed out.
    /// </summary>
    TIMEOUT,

    /// <summary>
    /// The operation was cancelled.
    /// </summary>
    CANCELLED,

    /// <summary>
    /// An invalid operation was attempted.
    /// </summary>
    INVALID_OPERATION,

    /// <summary>
    /// The archive field is missing for a soft delete operation.
    /// </summary>
    MISSING_ARCHIVE_FIELD
}
