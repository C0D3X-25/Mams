using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.productsLots;
using MySqlConnector;

namespace Mams_Test.integration.productsLots;

/// <summary>
/// Integration tests for <see cref="ProductLotModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class ProductLotModelIntegrationTests : IntegrationTestBase
{
    private readonly ProductLotModel _model = new();

    public ProductLotModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var item = new ProductLotItem
        {
            product_lot_name = "Lot Printemps",
            product_lot_year = 2025
        };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var item = new ProductLotItem { product_lot_name = "Lot Printemps", product_lot_year = 2025 };
        _model.saveItem(item);

        var duplicate = new ProductLotItem { product_lot_name = "Lot Printemps", product_lot_year = 2025 };
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
        var item = new ProductLotItem { product_lot_name = "Lot Printemps", product_lot_year = 2025 };
        var insertResponse = _model.saveItem(item);

        var updated = new ProductLotItem
        {
            product_lot_id = insertResponse.returned_id,
            product_lot_name = "Lot Été",
            product_lot_year = 2025
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Lot Été", fetched.returned_item!.product_lot_name);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        ABaseModel.startTransaction();

        var item = new ProductLotItem { product_lot_name = "Lot Printemps", product_lot_year = 2025 };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Lot Printemps", fetched.returned_item!.product_lot_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        ABaseModel.startTransaction();

        var item = new ProductLotItem { product_lot_name = "Lot Printemps", product_lot_year = 2025 };
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
        var item = new ProductLotItem { product_lot_name = "Lot Printemps", product_lot_year = 2025 };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Lot Printemps", response.returned_item.product_lot_name);
        Assert.Equal(2025, response.returned_item.product_lot_year);
        Assert.Equal(saved.returned_id, response.returned_item.product_lot_id);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  saveItem – with beehive FK
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_WithBeehiveFK_StoresBeehiveReference()
    {
        // Insert a beehive first
        int beehiveId;
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO beehives (beehive_name, beehive_number) VALUES ('Ruche Alpha', 'R001')";
            cmd.ExecuteNonQuery();
            beehiveId = (int)cmd.LastInsertedId;
        }

        var item = new ProductLotItem
        {
            product_lot_name = "Lot Printemps",
            product_lot_year = 2025,
            fk_beehive_id = beehiveId
        };
        var saved = _model.saveItem(item);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(beehiveId, fetched.returned_item!.fk_beehive_id);
        Assert.Equal("Ruche Alpha", fetched.returned_item.beehive_name);
    }

    // ─────────────────────────────────────────────
    //  getActiveProductLots
    // ─────────────────────────────────────────────

    [Fact]
    public void GetActiveProductLots_ReturnsOnlyNonArchived()
    {
        var active = new ProductLotItem { product_lot_name = "Lot Printemps", product_lot_year = 2025 };
        _model.saveItem(active);

        var archived = new ProductLotItem { product_lot_name = "Lot Ancien", product_lot_year = 2020 };
        var archivedSave = _model.saveItem(archived);
        _model.deleteItem(archivedSave.returned_id.ToString());

        var lots = _model.getActiveProductLots();

        Assert.Contains(lots, l => l.product_lot_name == "Lot Printemps");
        Assert.DoesNotContain(lots, l => l.product_lot_name == "Lot Ancien");
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var item = new ProductLotItem { product_lot_name = "Lot Printemps", product_lot_year = 2025 };
        var saved = _model.saveItem(item);

        // Create a receipt product referencing this lot so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO products_types (product_type_name) VALUES ('TestType')";
            cmd.ExecuteNonQuery();
            long typeId = cmd.LastInsertedId;
            cmd.CommandText = "INSERT INTO products_categories (product_category_name) VALUES ('TestCat')";
            cmd.ExecuteNonQuery();
            long catId = cmd.LastInsertedId;
            cmd.CommandText = $"INSERT INTO products (product_name, fk_product_type_id, fk_product_category_id) VALUES ('TestProduct', {typeId}, {catId})";
            cmd.ExecuteNonQuery();
            long productId = cmd.LastInsertedId;
            cmd.CommandText = "INSERT INTO receipts (receipt_number, receipt_total_price, receipt_date_created) VALUES ('R001', 100.00, '2025-01-01')";
            cmd.ExecuteNonQuery();
            cmd.CommandText =
                $"INSERT INTO receipts_products (receipt_product_quantity, receipt_product_unity_price, fk_product_id, fk_receipt_id, fk_product_lot_id) " +
                $"VALUES (1, 10.00, {productId}, LAST_INSERT_ID(), {saved.returned_id})";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.product_lot_archive);
    }
}
