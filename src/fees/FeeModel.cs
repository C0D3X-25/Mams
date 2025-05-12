using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.models;
using Mams.src.products;
using Mams.src.profits;
using Mams.src.receipts;
using System.Collections.ObjectModel;

namespace Mams.src.fees;

public class FeeModel : ABaseModel,
    ICrudOperation<FeeItem> {


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }


    public FeeItem? getItem(string search) {
        throw new NotImplementedException();
    }


    public FeeItem? getItemByID(string id) {
        throw new NotImplementedException();
    }


    public ObservableCollection<FeeItem> getTable() {

        ReceiptCascadeOperationModel receipt_operation_model = new();
        EntityModel entity_model = new();
        ProductModel product_model = new();

        ObservableCollection<FeeItem> items = new();

        var item_to_sort = receipt_operation_model.getTable();

        foreach (var item in item_to_sort) {

            // Skip the item if the supplier ID is 0, because it's a client receipt
            if (item.receipt_supplier_item.fk_receipt_id == 0) {
                continue;
            }

            int receipt_id = item.receipt_item.receipt_id;
            string supplier_name = entity_model.getItemByID(item.receipt_supplier_item.fk_receipt_id.ToString())?.entity_name ?? "";
            string profit_year = item.receipt_item.receipt_date_created;

            foreach (var product in item.receipt_product_items) {

                FeeItem fee_item = new();

                fee_item.receipt_id = receipt_id;
                fee_item.supplier_name = supplier_name;
                fee_item.product_name = product_model.getItemByID(product.fk_product_id.ToString())?.product_name ?? "";
                fee_item.fee_date = item.receipt_item.receipt_date_created;
                fee_item.fee_quantity = product.receipt_product_quantity;
                fee_item.fee_price_unity = Math.Round(product.receipt_product_unity_price, 2);
                fee_item.fee_price_total = Math.Round((fee_item.fee_quantity * fee_item.fee_price_unity), 2);

                items.Add(fee_item);
            }
        }
        return items;
    }


    public bool saveItem(FeeItem item) {
        throw new NotImplementedException();
    }
}
