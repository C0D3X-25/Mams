using Mams.src.models;
using System.Collections.ObjectModel;

namespace Mams.src.receipts;

public class ReceiptHandlerModel : ABaseModel {

    private readonly ReceiptModel _m_receipt_model = new();
    private readonly ReceiptProductModel _m_receipt_product_model = new();
    private readonly ReceiptClientModel _m_receipt_client_model = new();
    private readonly ReceiptSupplierModel _m_receipt_supplier_model = new();


    public ObservableCollection<ReceiptHandlerItem> getTable() {

        ObservableCollection<ReceiptHandlerItem> items = new();

        foreach (var id in _m_receipt_model.getRowsID()) {

            var receipt = getReceiptByID(id.ToString());
            if (receipt != null) { 
                items.Add(receipt);
            }
        }
        return items;
    }


    public ReceiptHandlerItem? getReceiptByID(string id) {

        if (id == null) {
            return null;
        }

        ReceiptHandlerItem item = new();

        item.receipt_item = _m_receipt_model.getItemByID(id) ?? new();
        item.receipt_client_item = _m_receipt_client_model.getItemByID(id) ?? new();
        item.receipt_supplier_item = _m_receipt_supplier_model.getItemByID(id) ?? new();
        item.receipt_product_items = _m_receipt_product_model.getListItemWithReceiptID(id) ?? new();

        return item;
    }


    public bool saveReceipt(ReceiptHandlerItem item) {

        int receipt_id = _m_receipt_model.saveAndGetLastID(item.receipt_item);
        
        // Save the receipt products
        foreach (var product in item.receipt_product_items) {
            product.fk_receipt_id = receipt_id;
            if (!_m_receipt_product_model.saveItem(product)) {
                return false;
            }
        }

        // Save the client
        if (item.receipt_client_item.fk_client_id > 0) {
            item.receipt_client_item.fk_receipt_id = receipt_id;
            return _m_receipt_client_model.saveItem(item.receipt_client_item);
        }
        // Or save the supplier
        else {
            if (item.receipt_supplier_item.fk_supplier_id > 0) {
                item.receipt_supplier_item.fk_receipt_id = receipt_id;
                return _m_receipt_supplier_model.saveItem(item.receipt_supplier_item);
            }
        }
        return false;
    }


    public bool deleteReceipt(string receipt_id) {

        if (_m_receipt_product_model.deleteItem(receipt_id)
            && _m_receipt_client_model.deleteItem(receipt_id)
            && _m_receipt_supplier_model.deleteItem(receipt_id)
            && _m_receipt_model.deleteItem(receipt_id)
        ) { 
            return true;
        }
        return false;
    }



}
