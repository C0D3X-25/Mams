using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.crudOperations;

public interface ICrudOperation<T> where T : ABaseItem {
    ObservableCollection<T> getTable();
    T? getItem(string search);
    T? getItemByID(string id);
    bool saveItem(T item);
    bool deleteItem(string id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE);
    bool deleteItem(int id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE);
}
