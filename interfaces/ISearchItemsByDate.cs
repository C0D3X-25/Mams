using Mams.items;

namespace Mams.interfaces;

interface ISearchItemsByDate : ISearchItems {
    List<string> searchItemsByDate(string search);
}
