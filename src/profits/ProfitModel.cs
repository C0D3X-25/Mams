using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.models;
using Mams.src.products;
using Mams.src.receipts;
using System.Collections.ObjectModel;

namespace Mams.src.profits;

public class ProfitModel : ABaseModel,
    ICrudOperation<ProfitItem> {


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        throw new NotImplementedException();
    }


    public ProfitItem? getItem(string search) {
        throw new NotImplementedException();
    }

    
    public ProfitItem? getItemByID(string id) {
        throw new NotImplementedException();
    }


    public ObservableCollection<ProfitItem> getTable() {

        ReceiptCascadeOperationModel receipt_operation_model = new();
        EntityModel entity_model = new();
        ProductModel product_model = new();

        ObservableCollection<ProfitItem> items = new();

        var item_to_sort = receipt_operation_model.getTable();

        foreach (var item in item_to_sort) {

            // Skip the item if the client ID is 0, because it's a supplier receipt
            if (item.receipt_client_item.fk_client_id == 0) { 
                continue; 
            }
            
            int receipt_id = item.receipt_item.receipt_id;
            string client_name = entity_model.getItemByID(item.receipt_client_item.fk_client_id.ToString())?.entity_name ?? "";
            string profit_year = item.receipt_item.receipt_date_created;

            foreach (var product in item.receipt_product_items) {

                ProfitItem profit_item = new();

                profit_item.receipt_id = receipt_id;
                profit_item.client_name = client_name;
                profit_item.product_name = product_model.getItemByID(product.fk_product_id.ToString())?.product_name ?? "";
                profit_item.profit_date = item.receipt_item.receipt_date_created;
                profit_item.profit_quantity = product.receipt_product_quantity;
                profit_item.profit_price_unity = Math.Round(product.receipt_product_unity_price, 2);
                profit_item.profit_price_total = Math.Round((profit_item.profit_quantity * profit_item.profit_price_unity), 2);

                items.Add(profit_item);
            }
        }
        return items;
    }


    public bool saveItem(ProfitItem item) {
        throw new NotImplementedException();
    }
}
