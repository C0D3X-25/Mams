using Mams.src.enums;
using Mams.src.interfaces;
using Mams.src.items;
using System.Collections.ObjectModel;

namespace Mams.src.models;

/// <summary>
/// Product Category is about what is is, like "Honey", "Wax", "Soap"

public class ProductCategoryModel :
    ABaseModel,
    ICRUDItem<ProductCategoryItem> {

    private const string _m_TBL_NAME = "product_categories";
    private const string _m_COL_ID = "product_category_id";
    private const string _m_COL_NAME = "product_category_name";
    private const string _m_COL_ARCHIVE = "product_category_archive";

    public bool deleteItem(string id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return DeleteItemModel.deleteItem(this, id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    public bool deleteItem(int id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    public ProductCategoryItem? getItem(string search) {
        throw new NotImplementedException();
    }

    public ProductCategoryItem? getItemByID(string id) {
        throw new NotImplementedException();
    }

    public ObservableCollection<ProductCategoryItem> getTable() {
        throw new NotImplementedException();
    }

    public bool saveItem(ProductCategoryItem item) {
        throw new NotImplementedException();
    }
}
