using Mams.src.items;
using Mams.src.models;
using System.Collections.ObjectModel;

namespace Mams.src.searchBars;

public interface ISearchItems<T> where T : ABaseModel {

    //List<string> searchItems(string search);
    ObservableCollection<T> getTable();
}
