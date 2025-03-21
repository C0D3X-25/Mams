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

public class ClientModel : 
    ABaseSearchModel,
    ISearchItemsByName,
    IGetTable<ClientItem> {

    private const string _m_TBL_NAME = "clients";
    private const string _m_COL_ID = "client_id";
    private const string _m_COL_NAME = "client_name";
    private const string _m_COL_PHONE = "client_phone";
    private const string _m_COL_EMAIL = "client_email";
    private const string _m_COL_CITY = "client_city";
    private const string _m_COL_ADDRESS = "client_address";

    public ObservableCollection<ClientItem> getTable() {
        return GetTableModel.getTableData<ClientItem>(this, _m_TBL_NAME);
    }

    public override List<string> searchItems(string search) {
        throw new NotImplementedException();
    }

    public List<string> searchItemsByName(string search) {
        throw new NotImplementedException();
    }


}
