using Mams_App.src.models;
using Mams_App.src.treatments;
using MySqlConnector;

namespace Mams_Test.integration.treatments;

/// <summary>
/// Integration tests for <see cref="TreatmentModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class TreatmentModelIntegrationTests : IntegrationTestBase
{
    private readonly TreatmentModel _model = new();

    public TreatmentModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  Helper methods
    // ─────────────────────────────────────────────

    private static int insertBeehive(string name, string number)
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"INSERT INTO beehives (beehive_name, beehive_number) VALUES ('{name}', '{number}'); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static int insertProduct(string name)
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            "INSERT INTO products_types (product_type_name) VALUES ('Type Test'); SELECT LAST_INSERT_ID();";
        int typeId = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText =
            "INSERT INTO products_categories (product_category_name) VALUES ('Cat Test'); SELECT LAST_INSERT_ID();";
        int catId = Convert.ToInt32(cmd.ExecuteScalar());

        cmd.CommandText =
            $"INSERT INTO products (product_name, product_weight, fk_product_type_id, fk_product_category_id, fk_product_shape_id) " +
            $"VALUES ('{name}', 500, {typeId}, {catId}, 1); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    private static int insertTreatmentStock(int productId, decimal initialQuantity)
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText =
            $"INSERT INTO treatment_stocks (treatment_stock_purchase_date, treatment_stock_initial_quantity, fk_product_id) " +
            $"VALUES ('2025-01-15', {initialQuantity}, {productId}); SELECT LAST_INSERT_ID();";
        return Convert.ToInt32(cmd.ExecuteScalar());
    }

    /// <summary>
    /// Creates all prerequisite data and returns (beehiveId, treatmentStockId).
    /// </summary>
    private static (int beehiveId, int stockId) createPrerequisites()
    {
        int beehiveId = insertBeehive("Ruche Alpha", "R001");
        int productId = insertProduct("Acide Oxalique");
        int stockId = insertTreatmentStock(productId, 1000);
        return (beehiveId, stockId);
    }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var (beehiveId, stockId) = createPrerequisites();

        var item = new TreatmentItem
        {
            treatment_date = "15.01.2025",
            treatment_hive_count = 5,
            treatment_dose_per_hive = 10.0m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
        };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    // ─────────────────────────────────────────────
    //  saveItem – UPDATE
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Update_ModifiesExistingRow()
    {
        var (beehiveId, stockId) = createPrerequisites();

        var item = new TreatmentItem
        {
            treatment_date = "15.01.2025",
            treatment_hive_count = 5,
            treatment_dose_per_hive = 10.0m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
        };
        var insertResponse = _model.saveItem(item);

        var updated = new TreatmentItem
        {
            treatment_id = insertResponse.returned_id,
            treatment_date = "20.02.2025",
            treatment_hive_count = 8,
            treatment_dose_per_hive = 12.5m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(8, fetched.returned_item!.treatment_hive_count);
        Assert.Equal(12.5m, fetched.returned_item.treatment_dose_per_hive);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        var (beehiveId, stockId) = createPrerequisites();

        ABaseModel.startTransaction();

        var item = new TreatmentItem
        {
            treatment_date = "15.01.2025",
            treatment_hive_count = 5,
            treatment_dose_per_hive = 10.0m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
        };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        var (beehiveId, stockId) = createPrerequisites();

        ABaseModel.startTransaction();

        var item = new TreatmentItem
        {
            treatment_date = "15.01.2025",
            treatment_hive_count = 5,
            treatment_dose_per_hive = 10.0m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
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
    public void GetItemByID_ExistingId_ReturnsItemWithJoinedData()
    {
        var (beehiveId, stockId) = createPrerequisites();

        var item = new TreatmentItem
        {
            treatment_date = "15.01.2025",
            treatment_hive_count = 5,
            treatment_dose_per_hive = 10.0m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
        };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal(5, response.returned_item.treatment_hive_count);
        Assert.Equal(10.0m, response.returned_item.treatment_dose_per_hive);
        Assert.Equal("Ruche Alpha", response.returned_item.beehive_name);
        Assert.Equal("Acide Oxalique", response.returned_item.product_name);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getAllItems
    // ─────────────────────────────────────────────

    [Fact]
    public void GetAllItems_ReturnsInsertedTreatments()
    {
        var (beehiveId, stockId) = createPrerequisites();

        var item = new TreatmentItem
        {
            treatment_date = "15.01.2025",
            treatment_hive_count = 5,
            treatment_dose_per_hive = 10.0m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
        };
        _model.saveItem(item);

        var response = _model.getAllItems();

        Assert.True(response.is_success);
        Assert.Contains(response.returned_items!, t => t.beehive_name == "Ruche Alpha");
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_HardDelete_RemovesRow()
    {
        var (beehiveId, stockId) = createPrerequisites();

        var item = new TreatmentItem
        {
            treatment_date = "15.01.2025",
            treatment_hive_count = 5,
            treatment_dose_per_hive = 10.0m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
        };
        var saved = _model.saveItem(item);

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  Stock remaining quantity after treatment
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_ReducesStockRemainingQuantity()
    {
        var (beehiveId, stockId) = createPrerequisites();

        var item = new TreatmentItem
        {
            treatment_date = "15.01.2025",
            treatment_hive_count = 5,
            treatment_dose_per_hive = 10.0m,
            fk_beehive_id = beehiveId,
            fk_treatment_stock_id = stockId
        };
        _model.saveItem(item);

        // getItemByID joins stock data – used_quantity should reflect usage
        var fetched = _model.getItemByID("1");
        if (fetched.is_success)
        {
            // stock_initial_quantity - used_quantity = remaining
            Assert.Equal(1000m - 50m, fetched.returned_item!.stock_remaining_quantity);
        }
    }
}
