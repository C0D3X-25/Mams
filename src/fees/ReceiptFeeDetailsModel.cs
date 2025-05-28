using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.models;
using Mams.src.receipts;
using Mams.src.suppliers;
using System.Collections.ObjectModel;

namespace Mams.src.fees;

public class ReceiptFeeDetailsModel : ABaseModel,
    ICrudOperation<ReceiptFeeDetailsItem> {

    private readonly ReceiptHandlerModel _m_receipt_handler_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly SupplierModel _m_supplier_model = new();
    private readonly ReceiptSupplierModel _m_receipt_supplier_model = new();
    private readonly ReceiptModel _m_receipts_model = new();
    private readonly ReceiptProductModel _m_receipt_product_model = new();


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }


    public ReceiptFeeDetailsItem? getItemByID(string id) {
        throw new NotImplementedException();
    }


    public ObservableCollection<ReceiptFeeDetailsItem> getTable() {

        ObservableCollection<ReceiptFeeDetailsItem> items = new();

        foreach (var receipt in _m_receipt_handler_model.getTable()) {

            // Skip because it's a profit receipt
            if (receipt.receipt_client_item.fk_client_id != 0) {
                continue; 
            }

            ReceiptFeeDetailsItem item = new() {
                receipt = receipt.receipt_item,
                receipt_products = receipt.receipt_product_items,
                receipt_supplier = receipt.receipt_supplier_item,
                supplier = _m_supplier_model.getItemByID(receipt.receipt_supplier_item.fk_supplier_id.ToString()) ?? new(),
            };

            item.entity = _m_entity_model.getItemByID(item.supplier.fk_entity_id.ToString()) ?? new();

            items.Add(item);
        }
        return items;
    }


    public bool saveItem(ReceiptFeeDetailsItem item) {
        throw new NotImplementedException();
    }
}
