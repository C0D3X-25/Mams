using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.databaseOperations;

public interface ICrudOperation<T> where T : ABaseItem {
    ObservableCollection<T> getTable();
    T? getItemByID(string id);
    int saveItem(T item); // Return the ID as int, 0 if there is an error
    bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE);
    bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE);
}
