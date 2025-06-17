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

        if (item == null) {
            return false;
        }

        int receipt_id = 0;

        // Insert new receipt
        if (item.receipt_item.receipt_id == 0) {
            receipt_id = _m_receipt_model.saveItem(item.receipt_item);
        }
        // Update existing receipt
        else {
            receipt_id = item.receipt_item.receipt_id;

            // TODO: Gonna need a better way to update a receipt
            _m_receipt_model.saveItem(item.receipt_item);
            _m_receipt_product_model.deleteItem(receipt_id);
            _m_receipt_client_model.deleteItem(receipt_id);
            _m_receipt_supplier_model.deleteItem(receipt_id);
        }

        // Save the receipt products
        foreach (var product in item.receipt_product_items) {
            product.fk_receipt_id = receipt_id;
            if (_m_receipt_product_model.saveItem(product) <= 0) {
                return false;
            }
        }

        // Save the client
        if (item.receipt_client_item.fk_client_id > 0) {
            item.receipt_client_item.fk_receipt_id = receipt_id;
            return _m_receipt_client_model.saveItem(item.receipt_client_item) > 0;
        }
        // Or save the supplier
        else {
            if (item.receipt_supplier_item.fk_supplier_id > 0) {
                item.receipt_supplier_item.fk_receipt_id = receipt_id;
                return _m_receipt_supplier_model.saveItem(item.receipt_supplier_item) > 0;
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
