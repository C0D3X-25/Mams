namespace Mams.src.databaseOperations;

public enum EDeleteItemOperation 
{
    SOFT_DELETE, // sets the archive date to today
    HARD_DELETE, // removes the item from the database
    SAFE_DELETE, // remove the item from the database, but if there is a foreign key constraint, it will archive the item instead
    RESTORE, // sets the archive date to null
    NONE // should cause an error if used
}
