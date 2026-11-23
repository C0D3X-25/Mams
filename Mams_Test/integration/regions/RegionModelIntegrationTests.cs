using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.regions;
using MySqlConnector;

namespace Mams_Test.integration.regions;

/// <summary>
/// Integration tests for <see cref="RegionModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class RegionModelIntegrationTests : IntegrationTestBase
{
    private readonly RegionModel _model = new();

    public RegionModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new RegionItem { region_name = "Jura" };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var item = new RegionItem { region_name = "Jura" };
        _model.saveItem(item);

        var duplicate = new RegionItem { region_name = "Jura" };
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
        var item = new RegionItem { region_name = "Jura" };
        var insertResponse = _model.saveItem(item);

        var updated = new RegionItem
        {
            region_id = insertResponse.returned_id,
            region_name = "Valais"
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Valais", fetched.returned_item!.region_name);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new RegionItem { region_name = "Jura" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Jura", fetched.returned_item!.region_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new RegionItem { region_name = "Jura" };
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
        var item = new RegionItem { region_name = "Jura" };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Jura", response.returned_item.region_name);
        Assert.Equal(saved.returned_id, response.returned_item.region_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getActiveRegions
    // ─────────────────────────────────────────────

    [Fact]
    public void GetActiveRegions_ExcludesArchivedRegions()
    {
        var active = new RegionItem { region_name = "Jura" };
        _model.saveItem(active);

        var toArchive = new RegionItem { region_name = "Valais" };
        var archivedSave = _model.saveItem(toArchive);

        // Create a beehive referencing this region so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO beehives (beehive_name, beehive_number, fk_region_id) " +
                $"VALUES ('Ruche Test', 'R001', {archivedSave.returned_id})";
            cmd.ExecuteNonQuery();
        }

        _model.deleteItem(archivedSave.returned_id.ToString());

        var regions = _model.getActiveRegions();

        Assert.Contains(regions, r => r.region_name == "Jura");
        Assert.DoesNotContain(regions, r => r.region_name == "Valais");

        // Verify Valais is archived (not hard-deleted)
        var fetched = _model.getItemByID(archivedSave.returned_id.ToString());
        Assert.True(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_Unreferenced_HardDeletesRow()
    {
        var item = new RegionItem { region_name = "Jura" };
        var saved = _model.saveItem(item);

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    [Fact]
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var item = new RegionItem { region_name = "Jura" };
        var saved = _model.saveItem(item);

        // Create a beehive referencing this region so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO beehives (beehive_name, beehive_number, fk_region_id) " +
                $"VALUES ('Ruche Test', 'R001', {saved.returned_id})";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
    }
}
