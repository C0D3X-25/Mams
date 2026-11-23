using Mams_App.src.entities;
using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.products;
using Mams_App.src.receipts;
using Mams_App.src.suppliers;
using MySqlConnector;

namespace Mams_Test.integration.receipts;

/// <summary>
/// Integration tests for <see cref="ReceiptHandlerModel"/> that verify
/// the composite save/delete operations across receipts, products, clients, and suppliers.
/// </summary>
[Collection("Database")]
public class ReceiptHandlerModelIntegrationTests : IntegrationTestBase
{
    private readonly ReceiptHandlerModel _model = new();
    private readonly EntityModel _entityModel = new();
    private readonly SupplierModel _supplierModel = new();

    public ReceiptHandlerModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    /// <summary>
    /// Helper: creates all prerequisites and returns a ready-to-save ReceiptHandlerItem with a supplier.
    /// </summary>
    private (ReceiptHandlerItem item, int supplierId, int productId) createSupplierReceipt()
    {
        // Entity → Supplier
        var entityResponse = _entityModel.saveItem(new EntityItem { entity_name = "Fournisseur Test" });
        var supplierResponse = _supplierModel.saveItem(new SupplierItem { fk_entity_id = entityResponse.returned_id });

        // Product prerequisites
        int productId;
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO products_types (product_type_name) VALUES ('Alimentaire')";
            cmd.ExecuteNonQuery();
            long typeId = cmd.LastInsertedId;
            cmd.CommandText = "INSERT INTO products_categories (product_category_name) VALUES ('Miel')";
            cmd.ExecuteNonQuery();
            cmd.CommandText = $"INSERT INTO products (product_name, fk_product_type_id, fk_product_category_id) VALUES ('Miel de Lavande', {typeId}, LAST_INSERT_ID())";
            cmd.ExecuteNonQuery();
            productId = (int)cmd.LastInsertedId;
        }

        var handlerItem = new ReceiptHandlerItem
        {
            receipt_item = new ReceiptItem
            {
                receipt_number = "FAC-2025-001",
                receipt_total_price = 50.00m,
                receipt_date_created = "15.01.2025"
            },
            receipt_supplier_item = new ReceiptSupplierItem
            {
                fk_supplier_id = supplierResponse.returned_id
            },
            receipt_client_item = new ReceiptClientItem(),
            receipt_product_items =
            [
                new ReceiptProductItem
                {
                    receipt_product_quantity = 5,
                    receipt_product_unity_price = 10.00m,
                    product_item = new ProductItem { product_id = productId },
                    product_lot_item = new Mams_App.src.productsLots.ProductLotItem()
                }
            ]
        };

        return (handlerItem, supplierResponse.returned_id, productId);
    }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT with supplier
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsertWithSupplier_ReturnsSuccess()
    {
        var (handlerItem, _, _) = createSupplierReceipt();

        var response = _model.saveItem(handlerItem);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    // ─────────────────────────────────────────────
    //  getItemByID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetItemByID_ExistingId_ReturnsFullReceipt()
    {
        var (handlerItem, supplierId, productId) = createSupplierReceipt();
        _model.saveItem(handlerItem);

        // Get the receipt ID from the database
        var receiptModel = new ReceiptModel();
        var ids = receiptModel.getRowsID();
        Assert.Single(ids);

        var response = _model.getItemByID(ids[0].ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("FAC-2025-001", response.returned_item.receipt_item.receipt_number);
        Assert.Equal(supplierId, response.returned_item.receipt_supplier_item.fk_supplier_id);
        Assert.Single(response.returned_item.receipt_product_items);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_RemovesReceiptAndRelatedData()
    {
        var (handlerItem, _, _) = createSupplierReceipt();
        _model.saveItem(handlerItem);

        var receiptModel = new ReceiptModel();
        var ids = receiptModel.getRowsID();
        Assert.Single(ids);
        int receiptId = ids[0];

        var response = _model.deleteItem(receiptId.ToString());

        Assert.True(response.is_success);

        // Receipt should be gone
        var fetched = receiptModel.getItemByID(receiptId.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        var (handlerItem, _, _) = createSupplierReceipt();

        ABaseModel.startTransaction();

        var response = _model.saveItem(handlerItem);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var receiptModel = new ReceiptModel();
        var ids = receiptModel.getRowsID();
        Assert.Single(ids);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        var (handlerItem, _, _) = createSupplierReceipt();

        ABaseModel.startTransaction();

        var response = _model.saveItem(handlerItem);
        Assert.True(response.is_success);

        ABaseModel.rollbackTransaction();

        var receiptModel = new ReceiptModel();
        var ids = receiptModel.getRowsID();
        Assert.Empty(ids);
    }
}
