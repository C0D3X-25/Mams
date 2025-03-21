using Mams.src.items;

namespace Mams.src.interfaces;

interface ISearchItemsByDate : ISearchItems {
    List<string> searchItemsByDate(string search);
}
