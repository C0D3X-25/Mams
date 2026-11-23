using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.receipts;
using MySqlConnector;

namespace Mams_Test.integration.receipts;

/// <summary>
/// Integration tests for <see cref="ReceiptModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class ReceiptModelIntegrationTests : IntegrationTestBase
{
    private readonly ReceiptModel _model = new();

    public ReceiptModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new ReceiptItem
        {
            receipt_number = "FAC-2025-001",
            receipt_total_price = 150.50m,
            receipt_date_created = "15.01.2025"
        };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicateNumber_ReturnsAlreadyExists()
    {
        var item = new ReceiptItem
        {
            receipt_number = "FAC-2025-001",
            receipt_total_price = 100m,
            receipt_date_created = "15.01.2025"
        };
        _model.saveItem(item);

        var duplicate = new ReceiptItem
        {
            receipt_number = "FAC-2025-001",
            receipt_total_price = 200m,
            receipt_date_created = "16.01.2025"
        };
        var response = _model.saveItem(duplicate);

        Assert.False(response.is_success);
        Assert.Equal(EErrors.ALREADY_EXISTS, response.error);
    }

    // ─────────────────────────────────────────────
    //  saveItem – UPDATE
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Update_ModifiesExistingRow()
    {
        var item = new ReceiptItem
        {
            receipt_number = "FAC-2025-001",
            receipt_total_price = 100m,
            receipt_date_created = "15.01.2025"
        };
        var insertResponse = _model.saveItem(item);

        var updated = new ReceiptItem
        {
            receipt_id = insertResponse.returned_id,
            receipt_number = "FAC-2025-001",
            receipt_total_price = 200m,
            receipt_date_created = "20.01.2025"
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(200m, fetched.returned_item!.receipt_total_price);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new ReceiptItem
        {
            receipt_number = "FAC-2025-001",
            receipt_total_price = 100m,
            receipt_date_created = "15.01.2025"
        };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("FAC-2025-001", fetched.returned_item!.receipt_number);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new ReceiptItem
        {
            receipt_number = "FAC-2025-001",
            receipt_total_price = 100m,
            receipt_date_created = "15.01.2025"
        };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.rollbackTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  getItemByID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetItemByID_ExistingId_ReturnsItem()
    {
        var item = new ReceiptItem
        {
            receipt_number = "FAC-2025-001",
            receipt_total_price = 150.50m,
            receipt_date_created = "15.01.2025"
        };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("FAC-2025-001", response.returned_item.receipt_number);
        Assert.Equal(150.50m, response.returned_item.receipt_total_price);
        Assert.Equal(saved.returned_id, response.returned_item.receipt_id);
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
    public void DeleteItem_HardDelete_RemovesRow()
    {
        var item = new ReceiptItem
        {
            receipt_number = "FAC-2025-001",
            receipt_total_price = 100m,
            receipt_date_created = "15.01.2025"
        };
        var saved = _model.saveItem(item);

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  getRowsID
    // ─────────────────────────────────────────────

    [Fact]
    public void GetRowsID_ReturnsAllIds()
    {
        _model.saveItem(new ReceiptItem { receipt_number = "FAC-001", receipt_total_price = 100m, receipt_date_created = "15.01.2025" });
        _model.saveItem(new ReceiptItem { receipt_number = "FAC-002", receipt_total_price = 200m, receipt_date_created = "16.01.2025" });

        var ids = _model.getRowsID();

        Assert.Equal(2, ids.Count);
    }
}
