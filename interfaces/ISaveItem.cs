using Mams.items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.interfaces;

public interface ISaveItem<T> where T : ABaseItem {
    bool saveItem(T item);
}
