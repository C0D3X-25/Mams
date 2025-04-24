using Mams.src.enums;
using Mams.src.interfaces;
using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.models;

/// <summary>
/// Type is about the product utility, where it will be used like "Exploitation", "Production"


public class ProductTypeModel :
    ABaseModel,
    ICRUDItem<ProductTypeItem> {

    private const string _m_TBL_NAME = "product_types";
    private const string _m_COL_ID = "product_type_id";
    private const string _m_COL_NAME = "product_type_name";
    private const string _m_COL_ARCHIVE = "product_type_archive";


    public bool deleteItem(string id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return DeleteItemModel.deleteItem(this, id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    public bool deleteItem(int id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    public ProductTypeItem? getItem(string search) {
        throw new NotImplementedException();
    }

    public ProductTypeItem? getItemByID(string id) {
        throw new NotImplementedException();
    }

    public ObservableCollection<ProductTypeItem> getTable() {
        throw new NotImplementedException();
    }

    public bool saveItem(ProductTypeItem item) {
        throw new NotImplementedException();
    }
}
