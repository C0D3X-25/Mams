using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.interfaces;

interface IGetTable {
    DataTable? getTable();
}
