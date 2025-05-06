using Mams.src.databaseOperations;
using Mams.src.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.profits;

public class ProfitModel : ABaseModel,
    ICrudOperation<ProfitItem> {

    public const string m_TBL_NAME = "profits";
    public const string m_COL_ID = "profit_id";
    public const string m_COL_CLIENT_NAME = "client_name";
    public const string m_COL_PRODUCT_NAME = "product_name";
    public const string m_COL_YEAR = "profit_year";
    public const string m_COL_PRICE_UNITY = "profit_price_unity";
    public const string m_COL_PRICE_TOTAL = "profit_price_total";


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }


    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }


    public ProfitItem? getItem(string search) {
        throw new NotImplementedException();
    }

    
    public ProfitItem? getItemByID(string id) {
        throw new NotImplementedException();
    }


    public ObservableCollection<ProfitItem> getTable() {
        throw new NotImplementedException();
    }


    public bool saveItem(ProfitItem item) {
        throw new NotImplementedException();
    }
}
