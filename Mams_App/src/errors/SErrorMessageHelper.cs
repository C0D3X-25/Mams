using Mams_App.src.localizations;

namespace Mams_App.src.errors;

/// <summary>
/// Provides user-friendly French error messages based on error types.
/// </summary>
/// <remarks>
/// This static helper class translates technical error codes into messages
/// that can be displayed to end users in French.
/// </remarks>
public static class SErrorMessageHelper
{

    /// <summary>
    /// Gets a user-friendly French message for the specified error type.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <returns>A French message suitable for display to end users.</returns>
    public static string GetUserMessage(EErrors error)
    {
        return error switch
        {
            EErrors.NONE => string.Empty,
            EErrors.UNKNOWN => Loc.Get("Error.Unknown"),
            EErrors.INVALID_INPUT => Loc.Get("Error.InvalidInput"),
            EErrors.MISSING_PARAMETER => Loc.Get("Error.MissingParameter"),
            EErrors.NOT_FOUND => Loc.Get("Error.NotFound"),
            EErrors.ALREADY_EXISTS => Loc.Get("Error.AlreadyExists"),
            EErrors.NULL_VALUE => Loc.Get("Error.NullValue"),
            EErrors.DATABASE_CONNECTION => Loc.Get("Error.DatabaseConnection"),
            EErrors.DATABASE_QUERY => Loc.Get("Error.DatabaseQuery"),
            EErrors.FOREIGN_KEY_VIOLATION => Loc.Get("Error.ForeignKeyViolation"),
            EErrors.UNIQUE_CONSTRAINT_VIOLATION => Loc.Get("Error.UniqueConstraintViolation"),
            EErrors.UNAUTHORIZED => Loc.Get("Error.Unauthorized"),
            EErrors.TIMEOUT => Loc.Get("Error.Timeout"),
            EErrors.CANCELLED => Loc.Get("Error.Cancelled"),
            EErrors.INVALID_OPERATION => Loc.Get("Error.InvalidOperation"),
            EErrors.MISSING_ARCHIVE_FIELD => Loc.Get("Error.MissingArchiveField"),
            _ => Loc.Get("Error.Default")
        };
    }

    /// <summary>
    /// Gets a user-friendly French message for a save operation failure.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <param name="itemName">The name of the item being saved (optional).</param>
    /// <returns>A French message suitable for display to end users.</returns>
    public static string GetSaveErrorMessage(EErrors error, string? itemName = null)
    {
        return error switch
        {
            EErrors.ALREADY_EXISTS when !string.IsNullOrEmpty(itemName)
                => Loc.Get("Error.AlreadyExistsWithName", itemName),
            EErrors.ALREADY_EXISTS
                => Loc.Get("Error.AlreadyExists"),
            EErrors.NULL_VALUE
                => Loc.Get("Error.Save.NullValue"),
            EErrors.INVALID_INPUT
                => Loc.Get("Error.Save.InvalidInput"),
            EErrors.DATABASE_CONNECTION
                => Loc.Get("Error.Save.DatabaseConnection"),
            EErrors.DATABASE_QUERY
                => Loc.Get("Error.Save.DatabaseQuery"),
            _ => GetUserMessage(error)
        };
    }

    /// <summary>
    /// Gets a user-friendly French message for a delete operation failure.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <returns>A French message suitable for display to end users.</returns>
    public static string GetDeleteErrorMessage(EErrors error)
    {
        return error switch
        {
            EErrors.NOT_FOUND
                => Loc.Get("Error.Delete.NotFound"),
            EErrors.FOREIGN_KEY_VIOLATION
                => Loc.Get("Error.Delete.ForeignKeyViolation"),
            EErrors.INVALID_INPUT
                => Loc.Get("Error.Delete.InvalidInput"),
            EErrors.DATABASE_QUERY
                => Loc.Get("Error.Delete.DatabaseQuery"),
            _ => GetUserMessage(error)
        };
    }

    /// <summary>
    /// Gets a user-friendly French message for a get/load operation failure.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <returns>A French message suitable for display to end users.</returns>
    public static string GetLoadErrorMessage(EErrors error)
    {
        return error switch
        {
            EErrors.NOT_FOUND
                => Loc.Get("Error.Load.NotFound"),
            EErrors.INVALID_INPUT
                => Loc.Get("Error.Load.InvalidInput"),
            EErrors.DATABASE_CONNECTION
                => Loc.Get("Error.Load.DatabaseConnection"),
            EErrors.DATABASE_QUERY
                => Loc.Get("Error.Load.DatabaseQuery"),
            _ => GetUserMessage(error)
        };
    }

    /// <summary>
    /// Builds a complete error message with user message and optional technical details.
    /// </summary>
    /// <param name="userMessage">The user-friendly message.</param>
    /// <param name="errorDetail">The technical error detail (can be null).</param>
    /// <returns>A formatted message string.</returns>
    public static string BuildFullMessage(string userMessage, string? errorDetail)
    {
        if (string.IsNullOrEmpty(errorDetail))
        {
            return userMessage;
        }
        return $"{userMessage}\n\n{Loc.Get("Message.TechnicalDetails")}\n{errorDetail}";
    }
}
