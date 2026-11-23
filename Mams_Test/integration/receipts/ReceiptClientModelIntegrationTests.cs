using Mams_App.src.models;
using Mams_App.src.receipts;
using MySqlConnector;

namespace Mams_Test.integration.receipts;

/// <summary>
/// Integration tests for <see cref="ReceiptClientModel"/> that verify
/// database operations for receipt-client relationships.
/// </summary>
[Collection("Database")]
public class ReceiptClientModelIntegrationTests : IntegrationTestBase
{
    private readonly ReceiptClientModel _model = new();

    public ReceiptClientModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

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
            "VALUES ('V-TEST', 0, '2025-01-15'); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static int insertClient()
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "INSERT INTO entities (entity_name) VALUES ('Client Test'); SELECT LAST_INSERT_ID();";
        int entityId = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText = $"INSERT INTO clients (fk_entity_id) VALUES ({entityId}); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    // ─────────────────────────────────────────────
    //  saveItem
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsId()
    {
        int receiptId = insertReceipt();
        int clientId = insertClient();

        var item = new ReceiptClientItem
        {
            fk_receipt_id = receiptId,
            fk_client_id = clientId
        };

        _model.saveItem(item);

        // Junction table has no auto_increment so verify data was written
        var fetched = _model.getItemByID(receiptId.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(clientId, fetched.returned_item!.fk_client_id);
    }

    [Fact]
    public void SaveItem_UpdateExisting_ChangesClient()
    {
        int receiptId = insertReceipt();
        int clientId1 = insertClient();
        int clientId2 = insertClient();

        var item = new ReceiptClientItem
        {
            fk_receipt_id = receiptId,
            fk_client_id = clientId1
        };
        _model.saveItem(item);

        var updated = new ReceiptClientItem
        {
            fk_receipt_id = receiptId,
            fk_client_id = clientId2
        };
        var response = _model.saveItem(updated);

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(receiptId.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(clientId2, fetched.returned_item!.fk_client_id);
    }

    // ─────────────────────────────────────────────
    //  getItemByID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetItemByID_ExistingId_ReturnsItem()
    {
        int receiptId = insertReceipt();
        int clientId = insertClient();

        var item = new ReceiptClientItem
        {
            fk_receipt_id = receiptId,
            fk_client_id = clientId
        };
        _model.saveItem(item);

        var response = _model.getItemByID(receiptId.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal(receiptId, response.returned_item.fk_receipt_id);
        Assert.Equal(clientId, response.returned_item.fk_client_id);
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
        int clientId = insertClient();

        var item = new ReceiptClientItem
        {
            fk_receipt_id = receiptId,
            fk_client_id = clientId
        };
        _model.saveItem(item);

        var response = _model.deleteItem(receiptId.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(receiptId.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  getAllItems
    // ─────────────────────────────────────────────

    [Fact]
    public void GetAllItems_ReturnsInsertedRelationships()
    {
        int receiptId = insertReceipt();
        int clientId = insertClient();

        var item = new ReceiptClientItem
        {
            fk_receipt_id = receiptId,
            fk_client_id = clientId
        };
        _model.saveItem(item);

        var response = _model.getAllItems();

        Assert.True(response.is_success);
        Assert.NotEmpty(response.returned_items!);
    }
}
