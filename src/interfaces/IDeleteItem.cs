using Mams.src.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.interfaces;

public interface IDeleteItem {
    bool deleteItem(string id);
    bool deleteItem(int id);
}
