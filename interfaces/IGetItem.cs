using Project_Mams.src.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Mams.src.interfaces;

public interface IGetItem<T> where T : ABaseItem {
    T? getItem(string search);
}
