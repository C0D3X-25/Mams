//using Mams.src.databaseOperations;
//using Mams.src.entities;
//using Mams.src.models;
//using Mams.src.products;
//using Mams.src.receipts;
//using System.Collections.ObjectModel;
//using System.Windows;

//namespace Mams.src.fees;

//public class FeeReceiptModel : ABaseModel,
//    ICrudOperation<FeeReceiptItem> {


//    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
//        throw new NotImplementedException();
//    }
//    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
//        throw new NotImplementedException();
//    }


//    public FeeReceiptItem? getItem(string search) {
//        throw new NotImplementedException();
//    }


//    public FeeReceiptItem? getItemByID(string id) {
//        throw new NotImplementedException();
//    }


//    public ObservableCollection<FeeReceiptItem> getTable() {

//        ReceiptHandlerModel receipt_operation_model = new();
//        EntityModel entity_model = new();
//        ProductModel product_model = new();

//        ObservableCollection<FeeReceiptItem> items = new();

//        var item_to_sort = receipt_operation_model.getTable();

//        foreach (var receipt in item_to_sort) {

//            // Skip the item if the supplier ID is 0, because it's a client receipt
//            if (receipt.receipt_supplier_item.fk_receipt_id == 0) {
//                continue;
//            }

//            FeeReceiptItem fee_receipt_item = new();

//            int receipt_id = receipt.receipt_item.receipt_id;

//            fee_receipt_item.receipt_id = receipt_id;
//            fee_receipt_item.fk_supplier_id = receipt.receipt_supplier_item.fk_supplier_id;
//            fee_receipt_item.fk_supplier_name = entity_model.getItemByID(receipt.receipt_supplier_item.fk_receipt_id.ToString())?.entity_name ?? "";
//            fee_receipt_item.receipt_date_created = receipt.receipt_item.receipt_date_created;
//            fee_receipt_item.receipt_total_price = 0; // Updated in the foreach loop

//            foreach (var product in receipt.receipt_product_items) {

//                FeeProductItem fee_product_item = new();

//                fee_product_item.product_item = product_model.getItemByID(product.fk_product_id.ToString()) ?? new ProductItem();
//                fee_product_item.fk_receipt_id = receipt_id;
//                //fee_product_item.fk_product_id = product.fk_product_id;
//                //fee_product_item.fk_product_name = product_model.getItemByID(product.fk_product_id.ToString())?.product_name ?? "";
//                fee_product_item.fee_quantity = product.receipt_product_quantity;
//                fee_product_item.fee_price_unity = Math.Round(product.receipt_product_unity_price, 2);
//                fee_product_item.fee_price_total = Math.Round((fee_product_item.fee_quantity * fee_product_item.fee_price_unity), 2);

//                fee_receipt_item.receipt_total_price += fee_product_item.fee_price_total;
//                fee_receipt_item.m_fee_items.Add(fee_product_item);
//            }
//            items.Add(fee_receipt_item);
//        }
//        return items;
//    }


//    public bool saveItem(FeeReceiptItem item) {
//        throw new NotImplementedException();
//    }
//}
