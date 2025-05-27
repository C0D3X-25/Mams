using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.models;
using Mams.src.products;
using Mams.src.receipts;
using MySqlConnector;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Markup;

namespace Mams.src.fees;

public class FeeProductModel : ABaseModel,
    ICrudOperation<FeeProductItem> {


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }


    public FeeProductItem? getItem(string search) {
        throw new NotImplementedException();
    }


    public FeeProductItem? getItemByID(string id) {
        throw new NotImplementedException();
    }


    public ObservableCollection<FeeProductItem> getTable() {

        ReceiptHandlerModel receipt_operation_model = new();
        EntityModel entity_model = new();
        ProductModel product_model = new();

        ObservableCollection<FeeProductItem> items = new();

        var item_to_sort = receipt_operation_model.getTable();

        int receipt_id = item.receipt_item.receipt_id;

        foreach (var product in receipt.receipt_product_items) {

            FeeProductItem fee_product_item = new();

            fee_product_item.product_item = product_model.getItemByID(product.fk_product_id.ToString()) ?? new ProductItem();
            fee_product_item.fk_receipt_id = receipt_id;
            fee_product_item.fee_quantity = product.receipt_product_quantity;
            fee_product_item.fee_price_unity = Math.Round(product.receipt_product_unity_price, 2);
            fee_product_item.fee_price_total = Math.Round((fee_product_item.fee_quantity * fee_product_item.fee_price_unity), 2);

            items.Add(fee_product_item);
        }
        return items;
    }


    public bool saveItem(FeeProductItem item) {
        throw new NotImplementedException();
    }


    public ObservableCollection<FeeProductItem> getListItemWithReceiptID(string fk_receipt) {

        ObservableCollection<FeeProductItem> items = new();

        if (string.IsNullOrEmpty(fk_receipt)) {
            return items;
        }

        ReceiptProductModel receipt_product_model = new();
        ProductModel product_model = new();

        var fee_product = receipt_product_model.getListItemWithReceiptID(fk_receipt);

        foreach (var product in fee_product.receipt_product_items) {

            FeeProductItem fee_product_item = new();

            fee_product_item.fk_receipt_id = id;
            fee_product_item.fee_quantity = product.receipt_product_quantity;
            fee_product_item.fee_price_unity = Math.Round(product.receipt_product_unity_price, 2);
            fee_product_item.fee_price_total = Math.Round((fee_product_item.fee_quantity * fee_product_item.fee_price_unity), 2);
            fee_product_item.product_item = product_model.getItemByID(product.fk_product_id.ToString()) ?? new ProductItem();

            items.Add(fee_product_item);
        }
        return items;
    }
}
