using Mams.src.crudOperations;
using Mams.src.models;
using Mams.src.products;
using System.Collections.ObjectModel;

namespace Mams.src.productsLots;

public class ProductLotModel :
    ABaseModel,
    ICrudOperation<ProductLotItem> {

    private const string _m_TBL_NAME = "products_lots";
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
        return GetTableModel.getTableData<ProductLotItem>(this, _m_TBL_NAME);
    }

    public bool saveItem(ProductLotItem item) {
        throw new NotImplementedException();
    }
}
