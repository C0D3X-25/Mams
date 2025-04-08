using Mams.src.enums;
using Mams.src.interfaces;
using Mams.src.items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mams.src.models;
internal class ProductModel :
    ABaseSearchModel,
    ISearchItemsByName,
    ICRUDItem<ProductItem> {

    private const string _m_TBL_NAME = "products";
    private const string _m_COL_ID = "product_id";
    private const string _m_COL_NAME = "product_name";
    private const string _m_COL_WEIGHT = "product_weight";
    private const string _m_COL_FK_PRODUCT_TYPE_ID = "fk_product_type_id";
    private const string _m_COL_FK_PRODUCT_TYPE_NAME = "product_type_name";
    private const string _m_COL_FK_PRODUCT_CATEGORY_ID = "fk_product_category_id";
    private const string _m_COL_FK_PRODUCT_CATEGORY_NAME = "product_category_name";
    private const string _m_COL_FK_PRODUCT_SHAPE_ID = "fk_product_shape_id";
    private const string _m_COL_FK_PRODUCT_SHAPE_NAME = "product_shape_name";
    private const string _m_COL_FK_PRODUCT_LOT_ID = "fk_product_lot_id";
    private const string _m_COL_FK_PRODUCT_LOT_NAME = "product_lot_nbr";
    private const string _m_COL_CITY = "entity_city";
    private const string _m_COL_ADDRESS = "entity_address";
    private const string _m_COL_ARCHIVE = "product_archive";


    public bool deleteItem(string id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        throw new NotImplementedException();
    }

    public bool deleteItem(int id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        throw new NotImplementedException();
    }

    public ProductItem? getItem(string search) {
        throw new NotImplementedException();
    }

    public ProductItem? getItemByID(string id) {
        throw new NotImplementedException();
    }

    public ObservableCollection<ProductItem> getTable() {
        return GetTableModel.getTableData<ProductItem>(this, _m_TBL_NAME);
    }

    public bool saveItem(ProductItem item) {
        throw new NotImplementedException();
    }

    public override List<string> searchItems(string search) {
        throw new NotImplementedException();
    }

    public List<string> searchItemsByName(string search) {
        throw new NotImplementedException();
    }
}

