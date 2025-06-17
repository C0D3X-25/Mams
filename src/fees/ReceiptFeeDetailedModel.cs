using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.models;
using Mams.src.products;
using Mams.src.receipts;
using Mams.src.suppliers;
using System.Collections.ObjectModel;

namespace Mams.src.fees;

public class ReceiptFeeDetailedModel : ABaseModel,
    ICrudOperation<ReceiptFeeDetailedItem> {

    private readonly ReceiptHandlerModel _m_receipt_handler_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly SupplierModel _m_supplier_model = new();
    //private readonly ReceiptSupplierModel _m_receipt_supplier_model = new();
    //private readonly ReceiptModel _m_receipts_model = new();
    //private readonly ReceiptProductModel _m_receipt_product_model = new();
    private readonly ProductModel _m_product_model = new();


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return _m_receipt_handler_model.deleteReceipt(id);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ReceiptFeeDetailedItem? getItemByID(string id) {

        var receipt = _m_receipt_handler_model.getReceiptByID(id);

        if (receipt == null) {
            return null;
        }

        ReceiptFeeDetailedItem item = new() {
            receipt = receipt.receipt_item,
            receipt_products = receipt.receipt_product_items,
            receipt_supplier = receipt.receipt_supplier_item,
            supplier = _m_supplier_model.getItemByID(receipt.receipt_supplier_item.fk_supplier_id.ToString()) ?? new(),
        };

        item.entity = _m_entity_model.getItemByID(item.supplier.fk_entity_id.ToString()) ?? new();

        foreach (var receipt_product in item.receipt_products) {
            ProductItem? product = _m_product_model.getItemByID(receipt_product.product_item.product_id.ToString());
            if (product != null) {
                receipt_product.product_item = product;
            }
        }
        return item;
    }


    public ObservableCollection<ReceiptFeeDetailedItem> getTable() {

        ObservableCollection<ReceiptFeeDetailedItem> items = new();

        foreach (var receipt in _m_receipt_handler_model.getTable()) {

            // Skip because it's a profit receipt
            if (receipt.receipt_client_item.fk_client_id != 0) {
                continue;
            }

            ReceiptFeeDetailedItem item = new() {
                receipt = receipt.receipt_item,
                receipt_products = receipt.receipt_product_items,
                receipt_supplier = receipt.receipt_supplier_item,
                supplier = _m_supplier_model.getItemByID(receipt.receipt_supplier_item.fk_supplier_id.ToString()) ?? new(),
            };

            item.entity = _m_entity_model.getItemByID(item.supplier.fk_entity_id.ToString()) ?? new();

            foreach (var receipt_product in item.receipt_products) {
                ProductItem? product = _m_product_model.getItemByID(receipt_product.product_item.product_id.ToString());
                if (product != null) {
                    receipt_product.product_item = product;
                }
            }

            items.Add(item);
        }
        return items;
    }


    public int saveItem(ReceiptFeeDetailedItem item) {

        if (item == null || item.receipt_products.Count() < 1 || item.entity.entity_id == 0)
            return 0;

        int supplier_id = findSupplierIdOrCreateNew(item.entity.entity_id);
        item.receipt_supplier.fk_supplier_id = supplier_id;
        item.supplier.supplier_id = supplier_id;

        UpdateReceiptTotalPrice(item);

        var handlerItem = new ReceiptHandlerItem {
            receipt_item = item.receipt,
            receipt_product_items = item.receipt_products,
            receipt_supplier_item = item.receipt_supplier
        };

        return _m_receipt_handler_model.saveReceipt(handlerItem); 
    }


    private int findSupplierIdOrCreateNew(int entity_id) {

        if (entity_id <= 0) {
            return 0;
        }

        var supplier = _m_supplier_model.getSupplierWithEntityFK(entity_id.ToString());
        if (supplier != null && supplier.supplier_id > 0) {
            return supplier.supplier_id;
        }

        var new_supplier = new SupplierItem {
            fk_entity_id = entity_id 
        };
        _m_supplier_model.saveItem(new_supplier);

        supplier = _m_supplier_model.getSupplierWithEntityFK(entity_id.ToString());
        return supplier?.supplier_id ?? 0;
    }


    private void UpdateReceiptTotalPrice(ReceiptFeeDetailedItem item) {
        decimal total = 0;
        foreach (var product in item.receipt_products) {
            total += product.receipt_product_quantity * product.receipt_product_unity_price;
        }
        item.receipt.receipt_total_price = total;
    }
}
