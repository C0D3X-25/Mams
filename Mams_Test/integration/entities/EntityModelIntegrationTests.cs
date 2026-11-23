using Mams_App.src.entities;
using Mams_App.src.errors;
using Mams_App.src.models;
using MySqlConnector;

namespace Mams_Test.integration.entities;

/// <summary>
/// Integration tests for <see cref="EntityModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class EntityModelIntegrationTests : IntegrationTestBase
{
    private readonly EntityModel _model = new();

    public EntityModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new EntityItem
        {
            entity_name = "Dupont",
            entity_phone = "0123456789",
            entity_email = "dupont@test.com",
            entity_city = "Paris",
            entity_address = "1 rue de la Paix"
        };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var item = new EntityItem { entity_name = "Dupont" };
        _model.saveItem(item);

        var duplicate = new EntityItem { entity_name = "Dupont" };
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
        var item = new EntityItem
        {
            entity_name = "Dupont",
            entity_phone = "0123456789",
            entity_email = "dupont@test.com",
            entity_city = "Paris",
            entity_address = "1 rue de la Paix"
        };
        var insertResponse = _model.saveItem(item);

        var updated = new EntityItem
        {
            entity_id = insertResponse.returned_id,
            entity_name = "Durand",
            entity_phone = "0987654321",
            entity_email = "durand@test.com",
            entity_city = "Lyon",
            entity_address = "2 avenue des Champs"
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Durand", fetched.returned_item!.entity_name);
        Assert.Equal("0987654321", fetched.returned_item.entity_phone);
        Assert.Equal("durand@test.com", fetched.returned_item.entity_email);
        Assert.Equal("Lyon", fetched.returned_item.entity_city);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new EntityItem { entity_name = "Dupont" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Dupont", fetched.returned_item!.entity_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new EntityItem { entity_name = "Dupont" };
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
        var item = new EntityItem
        {
            entity_name = "Dupont",
            entity_phone = "0123456789",
            entity_email = "dupont@test.com",
            entity_city = "Paris",
            entity_address = "1 rue de la Paix"
        };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Dupont", response.returned_item.entity_name);
        Assert.Equal("0123456789", response.returned_item.entity_phone);
        Assert.Equal(saved.returned_id, response.returned_item.entity_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getActiveEntities
    // ─────────────────────────────────────────────

    [Fact]
    public void GetActiveEntities_ReturnsOnlyNonArchived()
    {
        var active = new EntityItem { entity_name = "Dupont" };
        _model.saveItem(active);

        var archived = new EntityItem { entity_name = "Durand" };
        var archivedSave = _model.saveItem(archived);

        // Archive the entity by creating an FK reference (client) and then deleting
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO clients (fk_entity_id) VALUES ({archivedSave.returned_id})";
            cmd.ExecuteNonQuery();
        }
        _model.deleteItem(archivedSave.returned_id.ToString());

        var entities = _model.getActiveEntities();

        Assert.Single(entities);
        Assert.Equal("Dupont", entities[0].entity_name);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var item = new EntityItem { entity_name = "Dupont" };
        var saved = _model.saveItem(item);

        // Create a client referencing this entity so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO clients (fk_entity_id) VALUES ({saved.returned_id})";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.entity_archive);
    }
}
