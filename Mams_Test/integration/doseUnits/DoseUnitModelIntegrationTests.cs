using Mams_App.src.doseUnits;
using Mams_App.src.errors;
using Mams_App.src.models;
using MySqlConnector;

namespace Mams_Test.integration.doseUnits;

/// <summary>
/// Integration tests for <see cref="DoseUnitModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class DoseUnitModelIntegrationTests : IntegrationTestBase
{
    private readonly DoseUnitModel _model = new();

    public DoseUnitModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new DoseUnitItem { dose_unit_name = "mL" };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var item = new DoseUnitItem { dose_unit_name = "mL" };
        _model.saveItem(item);

        var duplicate = new DoseUnitItem { dose_unit_name = "mL" };
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
        var item = new DoseUnitItem { dose_unit_name = "mL" };
        var insertResponse = _model.saveItem(item);

        var updated = new DoseUnitItem
        {
            dose_unit_id = insertResponse.returned_id,
            dose_unit_name = "g"
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("g", fetched.returned_item!.dose_unit_name);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new DoseUnitItem { dose_unit_name = "mL" };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("mL", fetched.returned_item!.dose_unit_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new DoseUnitItem { dose_unit_name = "mL" };
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
        var item = new DoseUnitItem { dose_unit_name = "mL" };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("mL", response.returned_item.dose_unit_name);
        Assert.Equal(saved.returned_id, response.returned_item.dose_unit_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getActiveDoseUnits
    // ─────────────────────────────────────────────

    [Fact]
    public void GetActiveDoseUnits_ExcludesArchivedUnits()
    {
        var active = new DoseUnitItem { dose_unit_name = "mL" };
        _model.saveItem(active);

        var toArchive = new DoseUnitItem { dose_unit_name = "g" };
        var archivedSave = _model.saveItem(toArchive);

        // Create a treatment stock referencing this dose unit so SAFE_DELETE triggers archive
        int productId = insertProduct("Produit Test");
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO treatment_stocks (treatment_stock_purchase_date, treatment_stock_initial_quantity, fk_product_id, fk_dose_unit_id) " +
                $"VALUES ('2025-01-01', 100, {productId}, {archivedSave.returned_id})";
            cmd.ExecuteNonQuery();
        }

        _model.deleteItem(archivedSave.returned_id.ToString());

        var units = _model.getActiveDoseUnits();

        Assert.Contains(units, u => u.dose_unit_name == "mL");
        Assert.DoesNotContain(units, u => u.dose_unit_name == "g");

        // Verify "g" is archived (not hard-deleted)
        var fetched = _model.getItemByID(archivedSave.returned_id.ToString());
        Assert.True(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_Unreferenced_HardDeletesRow()
    {
        var item = new DoseUnitItem { dose_unit_name = "mL" };
        var saved = _model.saveItem(item);

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    [Fact]
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var item = new DoseUnitItem { dose_unit_name = "mL" };
        var saved = _model.saveItem(item);

        // Create a treatment stock referencing this dose unit so SAFE_DELETE triggers archive
        int productId = insertProduct("Produit Test");
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText =
                $"INSERT INTO treatment_stocks (treatment_stock_purchase_date, treatment_stock_initial_quantity, fk_product_id, fk_dose_unit_id) " +
                $"VALUES ('2025-01-01', 100, {productId}, {saved.returned_id})";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  Helper
    // ─────────────────────────────────────────────

    /// <summary>
    /// Inserts a minimal product row (required FK for treatment_stocks).
    /// </summary>
    private static int insertProduct(string name)
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "INSERT INTO products_types (product_type_name) VALUES ('Type Test'); SELECT LAST_INSERT_ID();";
        int typeId = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText = "INSERT INTO products_categories (product_category_name) VALUES ('Cat Test'); SELECT LAST_INSERT_ID();";
        int catId = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText = $"INSERT INTO products (product_name, product_weight, fk_product_type_id, fk_product_category_id, fk_product_shape_id) " +
                          $"VALUES ('{name}', 500, {typeId}, {catId}, 1); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }
}
