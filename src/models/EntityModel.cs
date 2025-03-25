using Mams.src.interfaces;
using Mams.src.items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.models;

public class EntityModel : 
    ABaseSearchModel,
    ISearchItemsByName,
    IGetTable<EntityItem> {

    private const string _m_TBL_NAME = "entities";
    private const string _m_COL_ID = "entity_id";
    private const string _m_COL_NAME = "entity_name";
    private const string _m_COL_PHONE = "entity_phone";
    private const string _m_COL_EMAIL = "entity_email";
    private const string _m_COL_CITY = "entity_city";
    private const string _m_COL_ADDRESS = "entity_address";

    public ObservableCollection<EntityItem> getTable() {
        return GetTableModel.getTableData<EntityItem>(this, _m_TBL_NAME);
    }

    public override List<string> searchItems(string search) {
        throw new NotImplementedException();
    }

    public List<string> searchItemsByName(string search) {
        throw new NotImplementedException();
    }
}
