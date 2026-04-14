using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.treatmentStocks;
using MySqlConnector;

namespace Mams_Test.integration.treatmentStocks;

/// <summary>
/// Integration tests for <see cref="TreatmentStockModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class TreatmentStockModelIntegrationTests : IntegrationTestBase
{
    private readonly TreatmentStockModel _model = new();

    public TreatmentStockModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    // ─────────────────────────────────────────────
    //  Helper methods
    // ─────────────────────────────────────────────

    /// <summary>
    /// Inserts prerequisite data and returns the product ID needed by treatment stocks.
    /// </summary>
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

    private static int insertDoseUnit(string name)
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = $"INSERT INTO dose_units (dose_unit_name) VALUES ('{name}'); SELECT LAST_INSERT_ID();";
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
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        int productId = insertProduct("Acide Oxalique");

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 500,
            fk_product_id = productId
        };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertWithAllFKs_ReturnsNewId()
    {
        int productId = insertProduct("Acide Oxalique");
        int doseUnitId = insertDoseUnit("mL");
        int supplierId = insertSupplier();

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 1000,
            fk_product_id = productId,
            fk_dose_unit_id = doseUnitId,
            fk_supplier_id = supplierId
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
        int productId = insertProduct("Acide Oxalique");

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 500,
            fk_product_id = productId
        };
        var insertResponse = _model.saveItem(item);

        var updated = new TreatmentStockItem
        {
            treatment_stock_id = insertResponse.returned_id,
            treatment_stock_purchase_date = "20.02.2025",
            treatment_stock_initial_quantity = 750,
            fk_product_id = productId
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal(750, fetched.returned_item!.treatment_stock_initial_quantity);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        int productId = insertProduct("Acide Oxalique");

        ABaseModel.startTransaction();

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 500,
            fk_product_id = productId
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
        int productId = insertProduct("Acide Oxalique");

        ABaseModel.startTransaction();

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 500,
            fk_product_id = productId
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
        int productId = insertProduct("Acide Oxalique");
        int doseUnitId = insertDoseUnit("mL");
        int supplierId = insertSupplier();

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 1000,
            fk_product_id = productId,
            fk_dose_unit_id = doseUnitId,
            fk_supplier_id = supplierId
        };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal(1000, response.returned_item.treatment_stock_initial_quantity);
        Assert.Equal("Acide Oxalique", response.returned_item.product_name);
        Assert.Equal("mL", response.returned_item.dose_unit_name);
        Assert.Equal("Fournisseur Test", response.returned_item.supplier_name);
    }

    [Fact]
    public void GetItemByID_NonExistingId_ReturnsNotFound()
    {
        var response = _model.getItemByID("99999");

        Assert.False(response.is_success);
    }

    // ─────────────────────────────────────────────
    //  getAvailableStocks
    // ─────────────────────────────────────────────

    [Fact]
    public void GetAvailableStocks_ReturnsStocksWithRemainingQuantity()
    {
        int productId = insertProduct("Acide Oxalique");

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 1000,
            fk_product_id = productId
        };
        _model.saveItem(item);

        var stocks = _model.getAvailableStocks();

        Assert.Contains(stocks, s => s.product_name == "Acide Oxalique" && s.treatment_stock_remaining_quantity > 0);
    }

    // ─────────────────────────────────────────────
    //  deleteItem
    // ─────────────────────────────────────────────

    [Fact]
    public void DeleteItem_HardDelete_RemovesRow()
    {
        int productId = insertProduct("Acide Oxalique");

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 500,
            fk_product_id = productId
        };
        var saved = _model.saveItem(item);

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.False(fetched.is_success);
    }

    // ─────────────────────────────────────────────
    //  getAllItems
    // ─────────────────────────────────────────────

    [Fact]
    public void GetAllItems_ReturnsInsertedStocks()
    {
        int productId = insertProduct("Acide Oxalique");

        var item = new TreatmentStockItem
        {
            treatment_stock_purchase_date = "15.01.2025",
            treatment_stock_initial_quantity = 500,
            fk_product_id = productId
        };
        _model.saveItem(item);

        var response = _model.getAllItems();

        Assert.True(response.is_success);
        Assert.Contains(response.returned_items!, s => s.product_name == "Acide Oxalique");
    }
}
