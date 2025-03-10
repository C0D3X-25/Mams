using Project_Mams.src.items;

namespace Project_Mams.src.interfaces;

interface ISearchItemsByDate : ISearchItems {
    List<string> searchItemsByDate(string search);
}
