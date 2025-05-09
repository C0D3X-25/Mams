using Mams.src.models;
using System.Collections.ObjectModel;

namespace Mams.src.receipts;

public class ReceiptCascadeOperationModel : ABaseModel {

    private readonly ReceiptModel _m_receipt_model = new();
    private readonly ReceiptProductModel _m_receipt_product_model = new();
    private readonly ReceiptClientModel _m_receipt_client_model = new();
    private readonly ReceiptSupplierModel _m_receipt_supplier_model = new();


    public ObservableCollection<ReceiptCascadeOperationItem> getTable() {

        ObservableCollection<ReceiptCascadeOperationItem> items = new();

        foreach (var id in _m_receipt_model.getRowsID()) {

            var receipt = getReceiptByID(id.ToString());

            if (receipt != null) { 
                items.Add(receipt);
            }
        }
        return items;
    }


    public ReceiptCascadeOperationItem? getReceiptByID(string id) {

        if (id == null) {
            return null;
        }

        ReceiptCascadeOperationItem item = new();

        item.receipt_item = _m_receipt_model.getItemByID(id) ?? new();
        item.receipt_client_item = _m_receipt_client_model.getItemByID(id) ?? new();
        item.receipt_supplier_item = _m_receipt_supplier_model.getItemByID(id) ?? new();
        item.receipt_product_items = _m_receipt_product_model.getListItemWithReceiptID(id) ?? new();

        return item;
    }


    public bool saveReceipt(ReceiptCascadeOperationItem item) {

        _m_receipt_model.saveItem(item.receipt_item);

        int receipt_id = _m_receipt_model.getLastID();

        // Save the products
        foreach (var product in item.receipt_product_items) {
            product.fk_receipt_id = receipt_id;
            _m_receipt_product_model.saveItem(product);
        }

        // Save the client
        if (item.receipt_client_item.fk_client_id > 0) {
            item.receipt_client_item.fk_receipt_id = receipt_id;
            _m_receipt_client_model.saveItem(item.receipt_client_item);
            return true;
        }
        // Or save the supplier
        else {
            if (item.receipt_supplier_item.fk_supplier_id > 0) {
                item.receipt_supplier_item.fk_receipt_id = receipt_id;
                _m_receipt_supplier_model.saveItem(item.receipt_supplier_item);
                return true;
            }
        }
        return false;
    }


    public void deleteReceipt(int receipt_id) {

        _m_receipt_product_model.deleteItem(receipt_id);
        _m_receipt_client_model.deleteItem(receipt_id);
        _m_receipt_supplier_model.deleteItem(receipt_id);
        _m_receipt_model.deleteItem(receipt_id);
    }
}
