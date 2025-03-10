using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.imp;

public static class CombineLists {

    /// <summary>
    /// Combines multiple lists into one list, taking elements from each list in order.
    /// </summary>
    /// <param name="lists">The lists of string to combine</param>
    /// <param name="max_elements">The maximum number of element in the final list</param>
    /// <returns></returns>
    public static List<string> getCombinedList(List<List<string>> lists, int max_elements = 10) {
        List<string> combined_list = new();
        int index = 0;

        while (combined_list.Count < max_elements) {
            bool added = false;
            foreach (var list in lists) {
                if (index < list.Count) {
                    combined_list.Add(list[index]);
                    added = true;
                    if (combined_list.Count == max_elements) {
                        break;
                    }
                }
            }
            if (!added) {
                break; // No more elements to add
            }
            index++;
        }
        return combined_list;
    }
}
