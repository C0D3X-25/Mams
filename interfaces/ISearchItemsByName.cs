using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Mams.src.interfaces;

interface ISearchItemsByName : ISearchItems {
    List<string> searchItemsByName(string search);
}
