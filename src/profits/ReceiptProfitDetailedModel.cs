using Mams.src.clients;
using Mams.src.databaseOperations;
using Mams.src.entities;
using Mams.src.models;
using Mams.src.products;
using Mams.src.receipts;
using System.Collections.ObjectModel;

namespace Mams.src.profits;

public class ReceiptProfitDetailedModel : ABaseModel,
    ICrudOperation<ReceiptProfitDetailedItem> {

    private readonly ReceiptHandlerModel _m_receipt_handler_model = new();
    private readonly EntityModel _m_entity_model = new();
    private readonly ClientModel _m_client_model = new();
    //private readonly ReceiptClientModel _m_receipt_client_model = new();
    //private readonly ReceiptModel _m_receipts_model = new();
    //private readonly ReceiptProductModel _m_receipt_product_model = new();
    private readonly ProductModel _m_product_model = new();


    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return _m_receipt_handler_model.deleteReceipt(id);
    }
    public bool deleteItem(int id, EDeleteItemOperation delete_type = EDeleteItemOperation.SOFT_DELETE) {
        return deleteItem(id.ToString(), delete_type);
    }


    public ReceiptProfitDetailedItem? getItemByID(string id) {

        var receipt = _m_receipt_handler_model.getReceiptByID(id);

        if (receipt == null) {
            return null;
        }

        ReceiptProfitDetailedItem item = new() {
            receipt = receipt.receipt_item,
            receipt_products = receipt.receipt_product_items,
            receipt_client = receipt.receipt_client_item,
            client = _m_client_model.getItemByID(receipt.receipt_client_item.fk_client_id.ToString()) ?? new(),
        };

        item.entity = _m_entity_model.getItemByID(item.client.fk_entity_id.ToString()) ?? new();

        foreach (var receipt_product in item.receipt_products) {
            ProductItem? product = _m_product_model.getItemByID(receipt_product.product_item.product_id.ToString());
            if (product != null) {
                receipt_product.product_item = product;
            }
        }
        return item;
    }


    public ObservableCollection<ReceiptProfitDetailedItem> getTable() {

        ObservableCollection<ReceiptProfitDetailedItem> items = new();

        foreach (var receipt in _m_receipt_handler_model.getTable()) {

            // Skip because it's a fee receipt
            if (receipt.receipt_supplier_item.fk_supplier_id != 0) {
                continue;
            }

            ReceiptProfitDetailedItem item = new() {
                receipt = receipt.receipt_item,
                receipt_products = receipt.receipt_product_items,
                receipt_client = receipt.receipt_client_item,
                client = _m_client_model.getItemByID(receipt.receipt_client_item.fk_client_id.ToString()) ?? new(),
            };

            item.entity = _m_entity_model.getItemByID(item.client.fk_entity_id.ToString()) ?? new();

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


    public bool saveItem(ReceiptProfitDetailedItem item) {

        if (item == null) {
            return false;
        }
        if (item.receipt_products.Count() < 1) {
            return false;
        }
        if (item.entity.entity_id == 0) {
            return false;
        }

        int client_id = findClientIdOrCreateNew(item.entity.entity_id);
        item.receipt_client.fk_client_id = client_id;
        item.client.client_id = client_id;

        UpdateReceiptTotalPrice(item);

        var handlerItem = new ReceiptHandlerItem {
            receipt_item = item.receipt,
            receipt_product_items = item.receipt_products,
            receipt_client_item = item.receipt_client
        };

        return _m_receipt_handler_model.saveReceipt(handlerItem);
    }


    private int findClientIdOrCreateNew(int entity_id) {

        if (entity_id <= 0) {
            return 0;
        }

        var client = _m_client_model.getClientWithEntityFK(entity_id.ToString());
        if (client != null && client.client_id > 0) {
            return client.client_id;
        }

        var new_client = new ClientItem {
            fk_entity_id = entity_id
        };
        _m_client_model.saveItem(new_client);

        client = _m_client_model.getClientWithEntityFK(entity_id.ToString());
        return client?.client_id ?? 0;
    }


    private void UpdateReceiptTotalPrice(ReceiptProfitDetailedItem item) {
        decimal total = 0;
        foreach (var product in item.receipt_products) {
            total += product.receipt_product_quantity * product.receipt_product_unity_price;
        }
        item.receipt.receipt_total_price = total;
    }
}

