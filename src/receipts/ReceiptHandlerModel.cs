using Mams.src.beehives;
using Mams.src.databaseOperations;
using Mams.src.errors;
using Mams.src.helpers;
using Mams.src.models;
using System.Collections.ObjectModel;

namespace Mams.src.receipts;

/// <summary>
/// Provides functionality for managing receipts, including retrieval, saving, and deletion operations.
/// </summary>
public class ReceiptHandlerModel : ABaseModel,
    ICrudOperation<ReceiptHandlerItem> {

    private readonly ReceiptModel _m_receipt_model = new();
    private readonly ReceiptProductModel _m_receipt_product_model = new();
    private readonly ReceiptClientModel _m_receipt_client_model = new();
    private readonly ReceiptSupplierModel _m_receipt_supplier_model = new();

    /// <summary>
    /// Deletes an item from the database based on the specified identifier and delete operation type.
    /// </summary>
    /// <remarks>The behavior of the delete operation depends on the specified <paramref name="delete_type"/>.
    /// For <see cref="EDeleteItemOperation.SAFE_DELETE"/>, the item is archived instead of being permanently removed.</remarks>
    /// <param name="id">The unique identifier of the item to be deleted. Cannot be null or empty.</param>
    /// <param name="delete_type">The type of delete operation to perform. Defaults to <see cref="EDeleteItemOperation.SAFE_DELETE"/>.</param>
    /// <returns>A <see cref="ResponseDeleteItem"/> containing the result of the delete operation.</returns>
    public ResponseDeleteItem deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {

        if (!SDataValidation.isIdValid(id)) {
            return ResponseDeleteItem.Failure(EErrors.INVALID_INPUT);
        }

        startTransaction();

        try
        {
            var productResult = _m_receipt_product_model.deleteItem(id, delete_type);
            if (!productResult.is_success)
            {
                throw new InvalidOperationException($"Failed to delete receipt products. Error: {productResult.error}");
            }
            
            var clientResult = _m_receipt_client_model.deleteItem(id, delete_type);
            if (!clientResult.is_success)
            {
                throw new InvalidOperationException($"Failed to delete receipt client. Error: {clientResult.error}");
            }
            
            var supplierResult = _m_receipt_supplier_model.deleteItem(id, delete_type);
            if (!supplierResult.is_success)
            {
                throw new InvalidOperationException($"Failed to delete receipt supplier. Error: {supplierResult.error}");
            }
            
            var receiptResult = _m_receipt_model.deleteItem(id, delete_type);
            if (!receiptResult.is_success)
            {
                throw new InvalidOperationException($"Failed to delete receipt. Error: {receiptResult.error}");
            }

            commitTransaction();
            return ResponseDeleteItem.Success();
        }
        catch (Exception)
        {
            rollbackTransaction();
            return ResponseDeleteItem.Failure(EErrors.DATABASE_QUERY);
        }
    }

    /// <summary>
    /// Retrieves all receipt handler items based on the current receipt model's row IDs.
    /// </summary>
    /// <returns>A <see cref="ResponseGetAllItems{ReceiptHandlerItem}"/> containing all receipt handler items and any error message.</returns>
    public ResponseGetAllItems<ReceiptHandlerItem> getAllItems() {

        ObservableCollection<ReceiptHandlerItem> items = new();

        foreach (var id in _m_receipt_model.getRowsID()) {
            var result = getItemByID(id.ToString());
            if (result.is_found) { 
                items.Add(result.returned_item!);
            }
        }
        return ResponseGetAllItems<ReceiptHandlerItem>.Success(items);
    }

    /// <summary>
    /// Retrieves a receipt and its associated details by the specified receipt ID.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt. Must be a valid ID.</param>
    /// <returns>A <see cref="ResponseGetItem{ReceiptHandlerItem}"/> containing the receipt and its associated details and any error message.</returns>
    public ResponseGetItem<ReceiptHandlerItem> getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return ResponseGetItem<ReceiptHandlerItem>.Failure(EErrors.INVALID_INPUT);
        }

        ReceiptHandlerItem item = new();

        item.receipt_item = _m_receipt_model.getItemByID(id).returned_item ?? new();
        item.receipt_client_item = _m_receipt_client_model.getItemByID(id).returned_item ?? new();
        item.receipt_supplier_item = _m_receipt_supplier_model.getItemByID(id).returned_item ?? new();
        item.receipt_product_items = _m_receipt_product_model.getListItemWithReceiptID(id) ?? new();

        return ResponseGetItem<ReceiptHandlerItem>.Success(item);
    }

    /// <summary>
    /// Saves a receipt and its associated data, including products, client, or supplier information.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptHandlerItem"/> containing the receipt details, products, client, and supplier information.
    /// The <paramref name="item"/> cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved receipt client or supplier.
    /// Returns a response with ID 0 if the operation fails.</returns>
    public ResponseSaveItem saveItem(ReceiptHandlerItem item)
    {
        if (item == null)
        {
            return ResponseSaveItem.Failure(EErrors.NULL_VALUE);
        }

        int receipt_id = item.receipt_item.receipt_id;

        startTransaction();

        try
        {
            // Insert new receipt
            if (receipt_id == 0)
            {
                var receiptResult = _m_receipt_model.saveItem(item.receipt_item);
                if (!receiptResult.is_success)
                {
                    throw new InvalidOperationException($"Failed to save receipt. Error: {receiptResult.error}");
                }
                receipt_id = receiptResult.returned_id;
            }
            // Update existing receipt
            else 
            {
                // TODO: Gonna need a better way to update a receipt
                var updateResult = _m_receipt_model.saveItem(item.receipt_item);
                if (!updateResult.is_success)
                {
                    throw new InvalidOperationException($"Failed to update receipt. Error: {updateResult.error}");
                }
                
                var productDeleteResult = _m_receipt_product_model.deleteItem(receipt_id.ToString(), EDeleteItemOperation.HARD_DELETE);
                if (!productDeleteResult.is_success)
                {
                    throw new InvalidOperationException($"Failed to delete existing receipt products. Error: {productDeleteResult.error}");
                }
                
                var clientDeleteResult = _m_receipt_client_model.deleteItem(receipt_id.ToString(), EDeleteItemOperation.HARD_DELETE);
                if (!clientDeleteResult.is_success)
                {
                    throw new InvalidOperationException($"Failed to delete existing receipt client. Error: {clientDeleteResult.error}");
                }
                
                var supplierDeleteResult = _m_receipt_supplier_model.deleteItem(receipt_id.ToString(), EDeleteItemOperation.HARD_DELETE);
                if (!supplierDeleteResult.is_success)
                {
                    throw new InvalidOperationException($"Failed to delete existing receipt supplier. Error: {supplierDeleteResult.error}");
                }
            }

            // Save the receipt products
            foreach (var product in item.receipt_product_items)
            {
                product.fk_receipt_id = receipt_id;
                var productResult = _m_receipt_product_model.saveItem(product);
                if (!productResult.is_success)
                {
                    throw new InvalidOperationException($"Failed to save receipt product. Error: {productResult.error}");
                }
            }

            // Save the client
            if (item.receipt_client_item.fk_client_id > 0) 
            {
                item.receipt_client_item.fk_receipt_id = receipt_id;

                var clientResult = _m_receipt_client_model.saveItem(item.receipt_client_item);
                if (!clientResult.is_success)
                {
                    throw new InvalidOperationException($"Failed to save receipt client. Error: {clientResult.error}");
                }
                
                commitTransaction();
                return ResponseSaveItem.Success(clientResult.returned_id);
            }
            // Or save the supplier
            else if (item.receipt_supplier_item.fk_supplier_id > 0)
            {
                item.receipt_supplier_item.fk_receipt_id = receipt_id;

                var supplierResult = _m_receipt_supplier_model.saveItem(item.receipt_supplier_item);
                if (!supplierResult.is_success) 
                {
                    throw new InvalidOperationException($"Failed to save receipt supplier. Error: {supplierResult.error}");
                }
                
                commitTransaction();
                return ResponseSaveItem.Success(supplierResult.returned_id);
            }
            
            throw new InvalidOperationException("No client or supplier specified for the receipt.");
        }
        catch (Exception)
        {
            rollbackTransaction();
            return ResponseSaveItem.Failure(EErrors.DATABASE_QUERY);
        }
    }
}
