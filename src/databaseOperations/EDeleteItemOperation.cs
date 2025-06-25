namespace Mams.src.databaseOperations;

public enum EDeleteItemOperation {
    SOFT_DELETE,
    HARD_DELETE,
    SAFE_DELETE,
    RESTORE,
    NONE // should cause an error if used
}
