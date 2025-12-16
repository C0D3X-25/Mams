using Mams.src.items;

namespace Mams.src.databaseOperations;

/// <summary>
/// Defines a contract for performing basic CRUD (Create, Read, Update, Delete) operations on items of type
/// <typeparamref name="T"/>.
/// </summary>
/// <remarks>This interface provides methods for retrieving, saving, and deleting items, as well as accessing the
/// entire collection. Implementations may vary in how operations are performed, such as handling persistence or
/// applying specific deletion strategies.</remarks>
/// <typeparam name="T">The type of item managed by the CRUD operations. Must inherit from <see cref="ABaseItem"/>.</typeparam>
public interface ICrudOperation<T> where T : ABaseItem {

    /// <summary>
    /// Retrieves all items from the data store.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{T}"/> containing the collection of items and any error message.
    /// The <see cref="ResponseGetAllItems{T}.returned_items"/> is an empty collection if no items are found or if the operation fails.</returns>
    ResponseGetAllItems<T> getAllItems();

    /// <summary>
    /// Retrieves an item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the item to retrieve.</param>
    /// <returns>A <see cref="ResponseGetItem{T}"/> containing the item associated with the specified identifier and any error message.
    /// The <see cref="ResponseGetItem{T}.returned_item"/> is <see langword="null"/> if no item is found or if the operation fails.</returns>
    ResponseGetItem<T> getItemByID(string id);

    /// <summary>
    /// Saves the specified item to the data store and returns a response containing the result.
    /// Save can be used for both creating new items and updating existing ones.
    /// </summary>
    /// <param name="item">The item to be saved.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the unique identifier of the saved item and any error message.
    /// The <see cref="ResponseSaveItem.returned_id"/> is 0 if the operation fails.</returns>
    ResponseSaveItem saveItem(T item);

    /// <summary>
    /// Deletes an item identified by the specified ID, using the specified deletion operation.
    /// </summary>
    /// <param name="id">The unique identifier of the item to delete. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of deletion operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation and any error message.
    /// The <see cref="ResponseDeleteItem.is_deleted"/> is <see langword="false"/> if the operation fails.</returns>
    ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SAFE_DELETE);
}
