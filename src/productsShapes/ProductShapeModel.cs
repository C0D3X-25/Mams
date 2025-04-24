using Mams.src.crudOperations;
using Mams.src.models;
using System.Collections.ObjectModel;

namespace Mams.src.productsShapes;

public class ProductShapeModel :
    ABaseModel,
    ICrudOperation<ProductShapeItem> {

    private const string _m_TBL_NAME = "products_shapes";
    private const string _m_COL_ID = "product_shape_id";
    private const string _m_COL_NAME = "product_shape_name";
    private const string _m_COL_ARCHIVE = "product_shape_archive";

    public bool deleteItem(string id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return DeleteItemModel.deleteItem(this, id, _m_COL_ID, _m_COL_ARCHIVE, _m_TBL_NAME, delete_type);
    }

    public bool deleteItem(int id, EDatabaseDeleteItem delete_type = EDatabaseDeleteItem.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }

    public ProductShapeItem? getItem(string search) {
        throw new NotImplementedException();
    }

    public ProductShapeItem? getItemByID(string id) {
        throw new NotImplementedException();
    }

    public ObservableCollection<ProductShapeItem> getTable() {
        throw new NotImplementedException();
    }

    public bool saveItem(ProductShapeItem item) {
        throw new NotImplementedException();
    }
}

