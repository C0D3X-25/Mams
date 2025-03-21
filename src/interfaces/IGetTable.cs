using Mams.src.items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.interfaces;

public interface IGetTable<T> where T : ABaseItem {
    ObservableCollection<T> getTable();
}

