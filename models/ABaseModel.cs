using Project_Mams.src.models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Mams.src.models;

/// <summary>
/// Base class for all models to inherit from.
/// Holds the SQL connection object for all models to use.
/// </summary>
public abstract class ABaseModel {
    public readonly SQLConnectionModel _m_conn = new();
}
