using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.databaseOperations;

public interface ICrudOperation<T> where T : ABaseItem {
    ObservableCollection<T> getTable();
    T? getItemByID(string id);
    bool saveItem(T item);
    bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE);
    bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE);
}
