using Mams_App.src.beehives;
using Mams_App.src.errors;
using Mams_App.src.models;
using MySqlConnector;

namespace Mams_Test.integration.beehives;

/// <summary>
/// Integration tests for <see cref="BeehiveModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class BeehiveModelIntegrationTests : IntegrationTestBase
{
    private readonly BeehiveModel _model = new();

    public BeehiveModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R001" };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var item = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R001" };
        _model.saveItem(item);

        var duplicate = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R002" };
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
        var item = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R001" };
        var insertResponse = _model.saveItem(item);

        var updated = new BeehiveItem
        {
            beehive_id = insertResponse.returned_id,
            beehive_name = "Ruche Beta",
            beehive_number = "R002"
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Ruche Beta", fetched.returned_item!.beehive_name);
        Assert.Equal("R002", fetched.returned_item.beehive_number);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R001" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Ruche Alpha", fetched.returned_item!.beehive_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R001" };
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
        var item = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R001" };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Ruche Alpha", response.returned_item.beehive_name);
        Assert.Equal("R001", response.returned_item.beehive_number);
        Assert.Equal(saved.returned_id, response.returned_item.beehive_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getActiveBeehives
    // ─────────────────────────────────────────────

    [Fact]
    public void GetActiveBeehives_ExcludesArchivedBeehives()
    {
        var active = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R001" };
        _model.saveItem(active);

        var archived = new BeehiveItem { beehive_name = "Ruche Beta", beehive_number = "R002" };
        var archivedSave = _model.saveItem(archived);

        // Create a product lot referencing this beehive so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO products_lots (product_lot_name, product_lot_year, fk_beehive_id) " +
                $"VALUES ('TestLot', 2025, {archivedSave.returned_id})";
            cmd.ExecuteNonQuery();
        }

        _model.deleteItem(archivedSave.returned_id.ToString());

        // getActiveBeehives filters out archived rows
        var beehives = _model.getActiveBeehives();

        Assert.Contains(beehives, b => b.beehive_name == "Ruche Alpha");
        Assert.DoesNotContain(beehives, b => b.beehive_name == "Ruche Beta");

        // Verify Beta is truly archived (not hard-deleted) via getItemByID
        var fetched = _model.getItemByID(archivedSave.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.beehive_archive);
    }

    // ─────────────────────────────────────────────
    //  saveItem – with region FK
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_WithRegionFK_StoresRegionReference()
    {
        // Insert a region first
        int regionId;
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO regions (region_name) VALUES ('Jura')";
            cmd.ExecuteNonQuery();
            regionId = (int)cmd.LastInsertedId;
        }

        var item = new BeehiveItem
        {
            beehive_name = "Ruche Alpha",
            beehive_number = "R001",
            fk_region_id = regionId
        };
        var saved = _model.saveItem(item);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(regionId, fetched.returned_item!.fk_region_id);
        Assert.Equal("Jura", fetched.returned_item.region_name);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var item = new BeehiveItem { beehive_name = "Ruche Alpha", beehive_number = "R001" };
        var saved = _model.saveItem(item);

        // Create a product lot referencing this beehive so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO products_lots (product_lot_name, product_lot_year, fk_beehive_id) " +
                $"VALUES ('Lot Test', 2025, {saved.returned_id})";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.beehive_archive);
    }
}
