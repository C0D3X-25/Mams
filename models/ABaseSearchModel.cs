using Project_Mams.src.interfaces;
using MySql.Data.MySqlClient;
using System.Data;

namespace Project_Mams.src.models;

/// <summary>
/// Base class for all Classes who need to search items in the DB.
/// </summary>
/// 
public abstract class ABaseSearchModel : ABaseModel, ISearchItems {

    protected readonly SearchItemModel _m_search_model = new();

    // Definition is passed to derived Classes
    public abstract List<string> searchItems(string search); 
}
