using Mams_App.src.entities;
using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.suppliers;
using MySqlConnector;

namespace Mams_Test.integration.suppliers;

/// <summary>
/// Integration tests for <see cref="SupplierModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class SupplierModelIntegrationTests : IntegrationTestBase
{
    private readonly SupplierModel _model = new();
    private readonly EntityModel _entityModel = new();

    public SupplierModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    /// <summary>
    /// Helper: inserts an entity and returns its ID.
    /// </summary>
    private int insertEntity(string name = "Supplier Entity")
    {
        var entity = new EntityItem { entity_name = name };
        var response = _entityModel.saveItem(entity);
        return response.returned_id;
    }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        int entityId = insertEntity();
        var item = new SupplierItem { fk_entity_id = entityId };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        int entityId = insertEntity();
        _model.saveItem(new SupplierItem { fk_entity_id = entityId });

        var duplicate = new SupplierItem { fk_entity_id = entityId };
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
        int entityId1 = insertEntity("Entity A");
        int entityId2 = insertEntity("Entity B");

        var insertResponse = _model.saveItem(new SupplierItem { fk_entity_id = entityId1 });

        var updated = new SupplierItem
        {
            supplier_id = insertResponse.returned_id,
            fk_entity_id = entityId2
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(entityId2, fetched.returned_item!.fk_entity_id);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        int entityId = insertEntity();

        ABaseModel.startTransaction();

        var response = _model.saveItem(new SupplierItem { fk_entity_id = entityId });
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(entityId, fetched.returned_item!.fk_entity_id);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        int entityId = insertEntity();

        ABaseModel.startTransaction();

        var response = _model.saveItem(new SupplierItem { fk_entity_id = entityId });
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
        int entityId = insertEntity();
        var saved = _model.saveItem(new SupplierItem { fk_entity_id = entityId });

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal(entityId, response.returned_item.fk_entity_id);
        Assert.Equal(saved.returned_id, response.returned_item.supplier_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getSupplierWithEntityFK
    // ─────────────────────────────────────────────

    [Fact]
    public void GetSupplierWithEntityFK_ExistingEntity_ReturnsSupplier()
    {
        int entityId = insertEntity();
        var saved = _model.saveItem(new SupplierItem { fk_entity_id = entityId });

        var result = _model.getSupplierWithEntityFK(entityId.ToString());

        Assert.NotNull(result);
        Assert.Equal(saved.returned_id, result.supplier_id);
        Assert.Equal(entityId, result.fk_entity_id);
    }

    [Fact]
    public void GetSupplierWithEntityFK_NonExistingEntity_ReturnsNull()
    {
        var result = _model.getSupplierWithEntityFK("99999");

        Assert.Null(result);
    }

    // ─────────────────────────────────────────────
    //  deleteSupplierWithEntityFK
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteSupplierWithEntityFK_ExistingEntity_DeletesSupplier()
    {
        int entityId = insertEntity();
        var saved = _model.saveItem(new SupplierItem { fk_entity_id = entityId });

        // SAFE_DELETE with no references → HARD_DELETE
        var response = _model.deleteSupplierWithEntityFK(entityId.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    [Fact]
    public void DeleteSupplierWithEntityFK_ReferencedByReceipt_ArchivesSupplier()
    {
        int entityId = insertEntity();
        var saved = _model.saveItem(new SupplierItem { fk_entity_id = entityId });

        // Create a receipt that references this supplier so SAFE_DELETE triggers SOFT_DELETE
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO receipts (receipt_number, receipt_total_price, receipt_date_created) VALUES ('R-001', 100, CURDATE())";
            cmd.ExecuteNonQuery();
            cmd.CommandText =
                $"INSERT INTO receipts_suppliers (fk_receipt_id, fk_supplier_id) VALUES (LAST_INSERT_ID(), {saved.returned_id})";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteSupplierWithEntityFK(entityId.ToString());

        Assert.True(response.is_success);

        // The item should still exist but be archived
        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.supplier_archive);
    }
}
