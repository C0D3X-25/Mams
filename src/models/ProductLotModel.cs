using Mams.src.enums;
using Mams.src.interfaces;
using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.models;

public class ProductLotModel :
    ABaseModel,
    ICRUDItem<ProductLotItem> {

    private const string _m_TBL_NAME = "product_lots";
    private const string _m_COL_ID = "product_lot_id";
    private const string _m_COL_NAME = "product_lot_name";
    private const string _m_COL_YEAR = "product_lot_year";
    private const string _m_COL_FK_BEEHIVE_ID = "fk_beehive_id";
    private const string _m_COL_FK_BEEHIVE_NAME = "beehive_name";
    private const string _m_COL_ARCHIVE = "product_lot_archive";


    public bool deleteItem(string id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return DeleteItemModel.deleteItem(this, id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    public bool deleteItem(int id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    public ProductLotItem? getItem(string search) {
        throw new NotImplementedException();
    }

    public ProductLotItem? getItemByID(string id) {
        throw new NotImplementedException();
    }

    public ObservableCollection<ProductLotItem> getTable() {
        throw new NotImplementedException();
    }

    public bool saveItem(ProductLotItem item) {
        throw new NotImplementedException();
    }
}
