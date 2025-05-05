using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.crudOperations;

public interface ICrudOperation<T> where T : ABaseItem {
    ObservableCollection<T> getTable();
    T? getItem(string search);
    T? getItemByID(string id);
    bool saveItem(T item);
    bool deleteItem(string id, DeleteItemOperationEnum delete_type = DeleteItemOperationEnum.SOFT_DELETE);
    bool deleteItem(int id, DeleteItemOperationEnum delete_type = DeleteItemOperationEnum.SOFT_DELETE);
}
