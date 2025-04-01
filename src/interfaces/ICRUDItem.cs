using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.interfaces;

public interface ICRUDItem<T> where T : ABaseItem {
    ObservableCollection<T> getTable();
    T? getItem(string search);
    T? getItemByID(string id);
    bool saveItem(T item);
    bool deleteItem(string id);
    bool deleteItem(int id);
}
