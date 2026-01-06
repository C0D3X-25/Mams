namespace Mams_App.src.errors;

/// <summary>
/// Provides user-friendly French error messages based on error types.
/// </summary>
/// <remarks>
/// This static helper class translates technical error codes into messages
/// that can be displayed to end users in French.
/// </remarks>
public static class SErrorMessageHelper {

    /// <summary>
    /// Gets a user-friendly French message for the specified error type.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <returns>A French message suitable for display to end users.</returns>
    public static string GetUserMessage(EErrors error) {
        return error switch {
            EErrors.NONE => string.Empty,
            EErrors.UNKNOWN => "Une erreur inconnue est survenue.",
            EErrors.INVALID_INPUT => "Les données saisies sont invalides.",
            EErrors.MISSING_PARAMETER => "Un paramètre requis est manquant.",
            EErrors.NOT_FOUND => "L'élément demandé n'a pas été trouvé.",
            EErrors.ALREADY_EXISTS => "Un élément avec les mêmes informations existe déjà.",
            EErrors.NULL_VALUE => "Une valeur requise n'a pas été fournie.",
            EErrors.DATABASE_CONNECTION => "Impossible de se connecter à la base de données.",
            EErrors.DATABASE_QUERY => "Une erreur est survenue lors de l'accès à la base de données.",
            EErrors.FOREIGN_KEY_VIOLATION => "Cet élément est utilisé par d'autres données et ne peut pas être modifié.",
            EErrors.UNIQUE_CONSTRAINT_VIOLATION => "Cette valeur existe déjà et doit être unique.",
            EErrors.UNAUTHORIZED => "Vous n'avez pas les droits nécessaires pour effectuer cette action.",
            EErrors.TIMEOUT => "L'opération a pris trop de temps et a été annulée.",
            EErrors.CANCELLED => "L'opération a été annulée.",
            EErrors.INVALID_OPERATION => "Cette opération n'est pas autorisée.",
            EErrors.MISSING_ARCHIVE_FIELD => "Impossible d'archiver cet élément.",
            _ => "Une erreur est survenue."
        };
    }

    /// <summary>
    /// Gets a user-friendly French message for a save operation failure.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <param name="itemName">The name of the item being saved (optional).</param>
    /// <returns>A French message suitable for display to end users.</returns>
    public static string GetSaveErrorMessage(EErrors error, string? itemName = null) {
        return error switch {
            EErrors.ALREADY_EXISTS when !string.IsNullOrEmpty(itemName) 
                => $"Un élément avec le nom '{itemName}' existe déjà.",
            EErrors.ALREADY_EXISTS 
                => "Un élément avec les mêmes informations existe déjà.",
            EErrors.NULL_VALUE 
                => "Les données à enregistrer sont invalides.",
            EErrors.INVALID_INPUT 
                => "Les informations saisies sont incorrectes.",
            EErrors.DATABASE_CONNECTION 
                => "Impossible de se connecter à la base de données. Vérifiez votre connexion.",
            EErrors.DATABASE_QUERY 
                => "Une erreur est survenue lors de l'enregistrement.",
            _ => GetUserMessage(error)
        };
    }

    /// <summary>
    /// Gets a user-friendly French message for a delete operation failure.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <returns>A French message suitable for display to end users.</returns>
    public static string GetDeleteErrorMessage(EErrors error) {
        return error switch {
            EErrors.NOT_FOUND 
                => "L'élément à supprimer n'a pas été trouvé.",
            EErrors.FOREIGN_KEY_VIOLATION 
                => "Cet élément est utilisé par d'autres données et ne peut pas être supprimé.",
            EErrors.INVALID_INPUT 
                => "Impossible de supprimer cet élément.",
            EErrors.DATABASE_QUERY 
                => "Une erreur est survenue lors de la suppression.",
            _ => GetUserMessage(error)
        };
    }

    /// <summary>
    /// Gets a user-friendly French message for a get/load operation failure.
    /// </summary>
    /// <param name="error">The error type.</param>
    /// <returns>A French message suitable for display to end users.</returns>
    public static string GetLoadErrorMessage(EErrors error) {
        return error switch {
            EErrors.NOT_FOUND 
                => "L'élément demandé n'a pas été trouvé.",
            EErrors.INVALID_INPUT 
                => "L'identifiant fourni est invalide.",
            EErrors.DATABASE_CONNECTION 
                => "Impossible de se connecter à la base de données.",
            EErrors.DATABASE_QUERY 
                => "Une erreur est survenue lors du chargement des données.",
            _ => GetUserMessage(error)
        };
    }

    /// <summary>
    /// Builds a complete error message with user message and optional technical details.
    /// </summary>
    /// <param name="userMessage">The user-friendly message.</param>
    /// <param name="errorDetail">The technical error detail (can be null).</param>
    /// <returns>A formatted message string.</returns>
    public static string BuildFullMessage(string userMessage, string? errorDetail) {
        if (string.IsNullOrEmpty(errorDetail)) {
            return userMessage;
        }
        return $"{userMessage}\n\nDétails techniques:\n{errorDetail}";
    }
}
