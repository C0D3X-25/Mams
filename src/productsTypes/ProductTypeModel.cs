using Mams.src.crudOperations;
using Mams.src.models;
using Mams.src.productsShapes;
using System.Collections.ObjectModel;

namespace Mams.src.productsTypes;

/// <summary>
/// Type is about the product utility, where it will be used like "Exploitation", "Production"


public class ProductTypeModel :
    ABaseModel,
    ICrudOperation<ProductTypeItem> {

    private const string _m_TBL_NAME = "products_types";
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
        return GetTableModel.getTableData<ProductTypeItem>(this, _m_TBL_NAME);
    }

    public bool saveItem(ProductTypeItem item) {
        throw new NotImplementedException();
    }
}
