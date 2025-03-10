using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Mams.src.interfaces;


interface ISearchItemsByID : ISearchItems {
    List<string> searchItemsByID(string search);
    List<string> searchItemsByID(int search);
}
