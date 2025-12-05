using Mams.src.beehives;
using Mams.src.databaseOperations;
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
    /// <returns><see langword="true"/> if the item was successfully deleted; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when any delete operation fails.</exception>
    public bool deleteItem(string id, EDeleteItemOperation delete_type = EDeleteItemOperation.HARD_DELETE) {

        if (!SDataValidation.isIdValid(id)) {
            return false;
        }

        startTransaction();

        try
        {
            if (!_m_receipt_product_model.deleteItem(id, delete_type))
            {
                throw new InvalidOperationException("Failed to delete receipt products.");
            }
            if (!_m_receipt_client_model.deleteItem(id, delete_type))
            {
                throw new InvalidOperationException("Failed to delete receipt client.");
            }
            if (!_m_receipt_supplier_model.deleteItem(id, delete_type))
            {
                throw new InvalidOperationException("Failed to delete receipt supplier.");
            }
            if (!_m_receipt_model.deleteItem(id, delete_type))
            {
                throw new InvalidOperationException("Failed to delete receipt.");
            }

            commitTransaction();
            return true;
        }
        catch
        {
            rollbackTransaction();
            throw;
        }
    }

    /// <summary>
    /// Retrieves a collection of receipt handler items based on the current receipt model's row IDs.
    /// </summary>
    /// <returns>An <see cref="ObservableCollection{T}"/> containing <see cref="ReceiptHandlerItem"/> objects corresponding to
    /// the row IDs in the receipt model. The collection will be empty if no valid receipts are found.</returns>
    public ObservableCollection<ReceiptHandlerItem> getTable() {

        ObservableCollection<ReceiptHandlerItem> items = new();

        foreach (var id in _m_receipt_model.getRowsID()) {
            var receipt = getItemByID(id.ToString());
            if (receipt != null) { 
                items.Add(receipt);
            }
        }
        return items;
    }

    /// <summary>
    /// Retrieves a receipt and its associated details by the specified receipt ID.
    /// </summary>
    /// <param name="id">The unique identifier of the receipt. Must be a valid ID.</param>
    /// <returns>A <see cref="ReceiptHandlerItem"/> containing the receipt and its associated details,  or <see langword="null"/>
    /// if the provided ID is invalid or no receipt is found.</returns>
    public ReceiptHandlerItem? getItemByID(string id) {

        if (!SDataValidation.isIdValid(id)) {
            return null;
        }

        ReceiptHandlerItem item = new();

        item.receipt_item = _m_receipt_model.getItemByID(id) ?? new();
        item.receipt_client_item = _m_receipt_client_model.getItemByID(id) ?? new();
        item.receipt_supplier_item = _m_receipt_supplier_model.getItemByID(id) ?? new();
        item.receipt_product_items = _m_receipt_product_model.getListItemWithReceiptID(id) ?? new();

        return item;
    }

    /// <summary>
    /// Saves a receipt and its associated data, including products, client, or supplier information.
    /// </summary>
    /// <param name="item">The <see cref="ReceiptHandlerItem"/> containing the receipt details, products, client, and supplier information.
    /// The <paramref name="item"/> cannot be <see langword="null"/>.</param>
    /// <returns>A <see cref="ResponseSaveItem"/> containing the ID of the saved receipt client or supplier, and any error message.
    /// Returns a response with ID 0 if the operation fails.</returns>
    public ResponseSaveItem saveItem(ReceiptHandlerItem item)
    {
        if (item == null)
        {
            return ResponseSaveItem.Failure("Item cannot be null.");
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
                    throw new InvalidOperationException(receiptResult.error_message ?? "Failed to save receipt.");
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
                    throw new InvalidOperationException(updateResult.error_message ?? "Failed to update receipt.");
                }
                if (!_m_receipt_product_model.deleteItem(receipt_id.ToString(), EDeleteItemOperation.HARD_DELETE))
                {
                    throw new InvalidOperationException("Failed to delete existing receipt products.");
                }
                if (!_m_receipt_client_model.deleteItem(receipt_id.ToString(), EDeleteItemOperation.HARD_DELETE))
                {
                    throw new InvalidOperationException("Failed to delete existing receipt client.");
                }
                if (!_m_receipt_supplier_model.deleteItem(receipt_id.ToString(), EDeleteItemOperation.HARD_DELETE))
                {
                    throw new InvalidOperationException("Failed to delete existing receipt supplier.");
                }
            }

            // Save the receipt products
            foreach (var product in item.receipt_product_items)
            {
                product.fk_receipt_id = receipt_id;
                var productResult = _m_receipt_product_model.saveItem(product);
                if (!productResult.is_success)
                {
                    throw new InvalidOperationException(productResult.error_message ?? "Failed to save receipt product.");
                }
            }

            // Save the client
            if (item.receipt_client_item.fk_client_id > 0) 
            {
                item.receipt_client_item.fk_receipt_id = receipt_id;

                var clientResult = _m_receipt_client_model.saveItem(item.receipt_client_item);
                if (!clientResult.is_success)
                {
                    throw new InvalidOperationException(clientResult.error_message ?? "Failed to save receipt client.");
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
                    throw new InvalidOperationException(supplierResult.error_message ?? "Failed to save receipt supplier.");
                }
                
                commitTransaction();
                return ResponseSaveItem.Success(supplierResult.returned_id);
            }
            
            throw new InvalidOperationException("No client or supplier specified for the receipt.");
        }
        catch (Exception ex)
        {
            rollbackTransaction();
            return ResponseSaveItem.Failure(ex.Message);
        }
    }
}
