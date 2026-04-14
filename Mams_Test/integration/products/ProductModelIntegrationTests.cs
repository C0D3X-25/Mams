using Mams_App.src.errors;
using Mams_App.src.models;
using Mams_App.src.products;
using MySqlConnector;

namespace Mams_Test.integration.products;

/// <summary>
/// Integration tests for <see cref="ProductModel"/> that verify
/// database operations including transactional behavior.
/// </summary>
[Collection("Database")]
public class ProductModelIntegrationTests : IntegrationTestBase
{
    private readonly ProductModel _model = new();

    public ProductModelIntegrationTests(DatabaseFixture fixture) : base(fixture) { }

    /// <summary>
    /// Helper: inserts prerequisite type and category, returns (typeId, categoryId).
    /// </summary>
    private (int typeId, int categoryId) insertPrerequisites()
    {
        using var conn = new MySqlConnection(DatabaseFixture.TestConnectionString);
        conn.Open();
        using var cmd = conn.CreateCommand();

        cmd.CommandText = "INSERT INTO products_types (product_type_name) VALUES ('Alimentaire')";
        cmd.ExecuteNonQuery();
        int typeId = (int)cmd.LastInsertedId;

        cmd.CommandText = "INSERT INTO products_categories (product_category_name) VALUES ('Miel')";
        cmd.ExecuteNonQuery();
        int categoryId = (int)cmd.LastInsertedId;

        return (typeId, categoryId);
    }

    // ─────────────────────────────────────────────
    //  saveItem – INSERT
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_Insert_ReturnsNewId()
    {
        var (typeId, categoryId) = insertPrerequisites();

        var item = new ProductItem
        {
            product_name = "Miel de Lavande",
            product_weight = 500,
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
        };

        var response = _model.saveItem(item);

        Assert.True(response.is_success);
        Assert.True(response.returned_id > 0);
    }

    [Fact]
    public void SaveItem_InsertDuplicate_ReturnsAlreadyExists()
    {
        var (typeId, categoryId) = insertPrerequisites();

        var item = new ProductItem
        {
            product_name = "Miel de Lavande",
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
        };
        _model.saveItem(item);

        var duplicate = new ProductItem
        {
            product_name = "Miel de Lavande",
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
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
        var (typeId, categoryId) = insertPrerequisites();

        var item = new ProductItem
        {
            product_name = "Miel de Lavande",
            product_weight = 500,
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
        };
        var insertResponse = _model.saveItem(item);

        var updated = new ProductItem
        {
            product_id = insertResponse.returned_id,
            product_name = "Miel de Thym",
            product_weight = 250,
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
        };
        var updateResponse = _model.saveItem(updated);

        Assert.True(updateResponse.is_success);

        var fetched = _model.getItemByID(insertResponse.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Miel de Thym", fetched.returned_item!.product_name);
        Assert.Equal(250, fetched.returned_item.product_weight);
    }

    // ─────────────────────────────────────────────
    //  saveItem – inside an existing transaction
    // ─────────────────────────────────────────────

    [Fact]
    public void SaveItem_InsideExistingTransaction_CommitsWithOuter()
    {
        var (typeId, categoryId) = insertPrerequisites();

        ABaseModel.startTransaction();

        var item = new ProductItem
        {
            product_name = "Miel de Lavande",
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
        };
        var response = _model.saveItem(item);
        Assert.True(response.is_success);

        ABaseModel.commitTransaction();

        var fetched = _model.getItemByID(response.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.Equal("Miel de Lavande", fetched.returned_item!.product_name);
    }

    [Fact]
    public void SaveItem_InsideExistingTransaction_RollbackDiscardsAll()
    {
        var (typeId, categoryId) = insertPrerequisites();

        ABaseModel.startTransaction();

        var item = new ProductItem
        {
            product_name = "Miel de Lavande",
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
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
    public void GetItemByID_ExistingId_ReturnsItemWithJoinedNames()
    {
        var (typeId, categoryId) = insertPrerequisites();

        var item = new ProductItem
        {
            product_name = "Miel de Lavande",
            product_weight = 500,
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
        };
        var saved = _model.saveItem(item);

        var response = _model.getItemByID(saved.returned_id.ToString());

        Assert.True(response.is_success);
        Assert.NotNull(response.returned_item);
        Assert.Equal("Miel de Lavande", response.returned_item.product_name);
        Assert.Equal(500, response.returned_item.product_weight);
        Assert.Equal("Alimentaire", response.returned_item.product_type_name);
        Assert.Equal("Miel", response.returned_item.product_category_name);
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
    public void DeleteItem_SafeDelete_ArchivesRow()
    {
        var (typeId, categoryId) = insertPrerequisites();

        var item = new ProductItem
        {
            product_name = "Miel de Lavande",
            fk_product_type_id = typeId,
            fk_product_category_id = categoryId
        };
        var saved = _model.saveItem(item);

        // Create a receipt product referencing this product so SAFE_DELETE triggers archive
        using (var conn = new MySqlConnection(DatabaseFixture.TestConnectionString))
        {
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO receipts (receipt_number, receipt_total_price, receipt_date_created) VALUES ('R001', 100.00, '2025-01-01')";
            cmd.ExecuteNonQuery();
            cmd.CommandText =
                $"INSERT INTO receipts_products (receipt_product_quantity, receipt_product_unity_price, fk_product_id, fk_receipt_id) " +
                $"VALUES (1, 10.00, {saved.returned_id}, LAST_INSERT_ID())";
            cmd.ExecuteNonQuery();
        }

        var response = _model.deleteItem(saved.returned_id.ToString());

        Assert.True(response.is_success);

        var fetched = _model.getItemByID(saved.returned_id.ToString());
        Assert.True(fetched.is_success);
        Assert.NotEqual(string.Empty, fetched.returned_item!.product_archive);
    }
}
