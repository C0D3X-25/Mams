using Mams_App.src.models;
using Mams_App.src.receipts;
using MySqlConnector;

namespace Mams_Test.integration.receipts;

/// <summary>
/// Integration tests for <see cref="ReceiptSupplierModel"/> that verify
/// database operations for receipt-supplier relationships.
/// </summary>
[Collection("Database")]
public class ReceiptSupplierModelIntegrationTests : IntegrationTestBase
{
    private readonly ReceiptSupplierModel _model = new();

    public ReceiptSupplierModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  Helper methods
    // ─────────────────────────────────────────────

    private static int insertReceipt()
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "INSERT INTO receipts (receipt_number, receipt_total_price, receipt_date_created) " +
            "VALUES ('F-TEST', 0, '2025-01-15'); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static int insertSupplier()
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO entities (entity_name) VALUES ('Fournisseur Test'); SELECT LAST_INSERT_ID();";
        int entityId = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText = $"INSERT INTO suppliers (fk_entity_id) VALUES ({entityId}); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // ─────────────────────────────────────────────
    //  saveItem
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsId()
    {
        int receiptId = insertReceipt();
        int supplierId = insertSupplier();

        var item = new ReceiptSupplierItem
        {
            fk_receipt_id = receiptId,
            fk_supplier_id = supplierId
        };

        _model.saveItem(item);

        // Junction table has no auto_increment so verify data was written
        var fetched = _model.getItemByID(receiptId.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(supplierId, fetched.returned_item!.fk_supplier_id);
    }

    [Fact]
    public void SaveItem_UpdateExisting_ChangesSupplier()
    {
        int receiptId = insertReceipt();
        int supplierId1 = insertSupplier();
        int supplierId2 = insertSupplier();

        var item = new ReceiptSupplierItem
        {
            fk_receipt_id = receiptId,
            fk_supplier_id = supplierId1
        };
        _model.saveItem(item);

        var updated = new ReceiptSupplierItem
        {
            fk_receipt_id = receiptId,
            fk_supplier_id = supplierId2
        };
        var response = _model.saveItem(updated);

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(receiptId.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(supplierId2, fetched.returned_item!.fk_supplier_id);
    }

    // ─────────────────────────────────────────────
    //  getItemByID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetItemByID_ExistingId_ReturnsItem()
    {
        int receiptId = insertReceipt();
        int supplierId = insertSupplier();

        var item = new ReceiptSupplierItem
        {
            fk_receipt_id = receiptId,
            fk_supplier_id = supplierId
        };
        _model.saveItem(item);

        var response = _model.getItemByID(receiptId.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal(receiptId, response.returned_item.fk_receipt_id);
        Assert.Equal(supplierId, response.returned_item.fk_supplier_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_RemovesRelationship()
    {
        int receiptId = insertReceipt();
        int supplierId = insertSupplier();

        var item = new ReceiptSupplierItem
        {
            fk_receipt_id = receiptId,
            fk_supplier_id = supplierId
        };
        _model.saveItem(item);

        var response = _model.deleteItem(receiptId.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(receiptId.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  getListItemWithSupplierID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetListItemWithSupplierID_ReturnsMatchingItems()
    {
        int receiptId = insertReceipt();
        int supplierId = insertSupplier();

        var item = new ReceiptSupplierItem
        {
            fk_receipt_id = receiptId,
            fk_supplier_id = supplierId
        };
        _model.saveItem(item);

        var items = _model.getListItemWithSupplierID(supplierId.ToString());

        Assert.NotEmpty(items);
        Assert.Contains(items, i => i.fk_receipt_id == receiptId);
    }

    // ─────────────────────────────────────────────
    //  getAllItems
    // ─────────────────────────────────────────────

    [Fact]
    public void GetAllItems_ReturnsInsertedRelationships()
    {
        int receiptId = insertReceipt();
        int supplierId = insertSupplier();

        var item = new ReceiptSupplierItem
        {
            fk_receipt_id = receiptId,
            fk_supplier_id = supplierId
        };
        _model.saveItem(item);

        var response = _model.getAllItems();

        Assert.True(response.is_success);
        Assert.NotEmpty(response.returned_items!);
    }
}
